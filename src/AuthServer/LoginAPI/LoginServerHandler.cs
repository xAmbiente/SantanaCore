using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper.FastCrud;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Ionic.Zlib;
using Santana.Database.Auth;
using Serilog;
namespace Santana.LoginAPI
{
    public class LoginServerHandler : ChannelHandlerAdapter
    {
        private const short FrameMagic = 0x5713;
        private const int PacketSizeCap = 4096;
        private const int UsernameCap = 15;
        private const int PasswordCap = 19;
        private static readonly SemaphoreSlim _authGate = new SemaphoreSlim(1, 1);
        private static readonly ILogger Log = Serilog.Log.ForContext("SourceContext", "LoginServer");
        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            var greeting = new DMessage();
            greeting.Write(DMessage.MessageType.Notify);
            greeting.Write($"<region>{Config.Instance.AuthAPI.Region}</region>");
            SendA(context, greeting);
        }
        public override void ChannelRead(IChannelHandlerContext context, object messageData)
        {
            IByteBuffer incoming = null;
            try
            {
                incoming = messageData as IByteBuffer;
                var raw = new byte[0];
                if (incoming != null) raw = incoming.GetIoBuffer().ToArray();
                if (raw.Length == 0 || raw.Length > PacketSizeCap)
                {
                    context.CloseAsync();
                    return;
                }
                var frame = new DMessage(raw, raw.Length);
                short magic = 0;
                var payload = new ByteArray();
                if (frame.Read(ref magic) && magic == FrameMagic && frame.Read(ref payload))
                {
                    var inner = new DMessage(payload);
                    DMessage.MessageType coreId = 0;
                    if (!inner.Read(ref coreId))
                    {
                        return;
                    }
                    switch (coreId)
                    {
                        case DMessage.MessageType.Rmi:
                            short rmiId = 0;
                            if (inner.Read(ref rmiId))
                            {
                                switch (rmiId)
                                {
                                    case 15:
                                        {
                                            var user = string.Empty;
                                            var pass = string.Empty;
                                            var machineId = string.Empty;
                                            var secret = "";
                                            if (inner.Read(ref user)
                                                && inner.Read(ref pass)
                                                && inner.Read(ref machineId)
                                                && inner.Read(ref secret))
                                            {
                                                _ = LoginAsync(context, user, pass, machineId, secret)
                                                    .ContinueWith(t => Log.Error(t.Exception, "The background credential-check task ended in a faulted state"),
                                                        TaskContinuationOptions.OnlyOnFaulted);
                                            }
                                            else
                                            {
                                                Log.Error("Could not decode the four credential fields sent by {endpoint}", context.Channel.RemoteAddress.ToString());
                                                SendLoginError(context, "Invalid login.");
                                            }
                                            break;
                                        }
                                    case 17:
                                        context.CloseAsync();
                                        break;
                                    default:
                                        Log.Error("No handler is bound to remote call {rmi}, sent by {endpoint}", rmiId, context.Channel.RemoteAddress.ToString());
                                        break;
                                }
                            }
                            break;
                        case DMessage.MessageType.Notify:
                            context.CloseAsync();
                            break;
                        default:
                            Log.Error("Frame carries an unrecognised core identifier {coreid}, sent by {endpoint}", coreId, context.Channel.RemoteAddress.ToString());
                            break;
                    }
                }
                else
                {
                    Log.Error("Frame layout from {endpoint} does not match the expected envelope; dropping the link", context.Channel.RemoteAddress.ToString());
                    context.CloseAsync();
                }
            }
            finally
            {
                incoming?.Release();
            }
        }
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            if (exception is System.Net.Sockets.SocketException sockEx && sockEx.ErrorCode == 10054)
            {
                return;
            }
            base.ExceptionCaught(context, exception);
            Log.Error(exception, "Fault bubbled up from the credential pipeline");
        }
        private static async Task LoginAsync(IChannelHandlerContext context, string username, string password, string hwid, string seckey)
        {
            await _authGate.WaitAsync();
            try
            {
                using (var db = AuthDatabase.Open())
                {
                    Log.Information("Beginning credential verification requested by {endpoint}", context.Channel.RemoteAddress.ToString());
                    username = username?.Trim();
                    password = password ?? string.Empty;
                    hwid = hwid?.Trim();
                    seckey = seckey?.Trim();
                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                        string.IsNullOrWhiteSpace(hwid) || string.IsNullOrEmpty(seckey))
                    {
                        Log.Error("One or more of the four required credential fields arrived blank from {endpoint}", context.Channel.RemoteAddress.ToString());
                        SendLoginError(context, "Failed to login xxx ");
                        return;
                    }
                    if (hwid.Length < 14 || hwid.Length > 20 ||
                        Config.Instance.AuthAPI.BlockedHWIDS?.Contains(hwid) == true)
                    {
                        Log.Error("Machine fingerprint from {0} is outside the 14-20 char range or sits on the deny list", context.Channel.RemoteAddress.ToString());
                        SendLoginError(context, "Failed to login error 4.");
                        return;
                    }
                    if (seckey != "evolved")
                    {
                        Log.Error("Shared handshake secret presented by {0} does not match the expected value", context.Channel.RemoteAddress.ToString());
                        SendLoginError(context, "Failed to login error 5.");
                        return;
                    }
                    if (username.Length > UsernameCap || password.Length > PasswordCap)
                    {
                        Log.Error("Identifier or secret from {0} runs past the permitted character budget", context.Channel.RemoteAddress.ToString());
                        SendLoginError(context, "Invalid length of Username/Password.");
                        return;
                    }
                    if (username.Length > 4 && password.Length > 4 && Namecheck.IsNameValid(username))
                    {
                        var matches = db.Find<AccountDto>(statement => statement
                            .Where($"{nameof(AccountDto.Username):C} = @{nameof(username)}")
                            .Include<BanDto>(join => join.LeftOuterJoin())
                            .WithParameters(new { Username = username }))
                            .ToList();
                        var account = matches.FirstOrDefault();
                        if (account == null && (Config.Instance.NoobMode || Config.Instance.AutoRegister))
                        {
                            account = new AccountDto { Username = username };
                            var saltBytes = new byte[24];
                            RandomNumberGenerator.Fill(saltBytes);
                            byte[] derived;
                            using (var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 24000))
                                derived = deriveBytes.GetBytes(24);
                            account.Password = Convert.ToBase64String(derived);
                            account.Salt = Convert.ToBase64String(saltBytes);
                            account.Hwid = hwid;
                            await db.InsertAsync(account);
                        }
                        if (account == null)
                        {
                            Log.Error("No stored account carries the identifier {username}, as requested by {endpoint}", username,
                                context.Channel.RemoteAddress.ToString());
                            SendLoginError(context, "Invalid Username or Password.");
                            return;
                        }
                        var activeBan = GetActiveBan(account);
                        if (activeBan != null)
                        {
                            SendLoginError(context, "Account banned.");
                            return;
                        }
                        byte[] storedSalt;
                        byte[] storedHash;
                        try
                        {
                            storedSalt = Convert.FromBase64String(account.Salt ?? "");
                            storedHash = Convert.FromBase64String(account.Password ?? "");
                        }
                        catch (FormatException)
                        {
                            Log.Error("Stored secret for {username} is not decodable base64; the record looks corrupt", username);
                            SendLoginError(context, "Invalid Username or Password.");
                            return;
                        }
                        byte[] candidateHash;
                        try
                        {
                            using (var deriveBytes = new Rfc2898DeriveBytes(password, storedSalt, 24000))
                                candidateHash = deriveBytes.GetBytes(24);
                        }
                        catch (ArgumentException)
                        {
                            Log.Error("Stored salt for {username} was refused by the key derivation routine; the record looks corrupt", username);
                            SendLoginError(context, "Invalid Username or Password.");
                            return;
                        }
                        var mismatch = (uint)candidateHash.Length ^ (uint)storedHash.Length;
                        for (var i = 0; i < candidateHash.Length && i < storedHash.Length; i++)
                            mismatch |= (uint)(candidateHash[i] ^ storedHash[i]);
                        if ((mismatch != 0 || string.IsNullOrWhiteSpace(account?.Password ?? "")) && !Config.Instance.NoobMode)
                        {
                            Log.Error("Derived secret for {username} does not reproduce the stored digest, from {endpoint}", username, context.Channel.RemoteAddress.ToString());
                            SendLoginError(context, "Invalid Username or Password.");
                            return;
                        }
                        else
                        {
                            if (account != null)
                            {
                                var lettersOnly = Regex.Replace(hwid, "[^a-zA-Z]", "");
                                var Token = $"{context.Channel.RemoteAddress}-{account.Username}-{account.Password}-{lettersOnly}";
                                var LoginToken = string.Format("{0:X}", Convert.ToBase64String(test.CompressZLib(Encoding.UTF8.GetBytes(Token))).GetHashCode()).ToLower();
                                account.LoginToken = LoginToken;
                                AuthHash.GetHash256($"{context.Channel.RemoteAddress}-{account.Username}-{account.Password}").ToLower();
                                account.LastLogin = $"{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";
                                account.AuthToken = "";
                                account.newToken = "";
                                account.Hwid = hwid;
                                await db.UpdateAsync(account);
                                var ack = new DMessage();
                                ack.Write(true);
                                ack.Write(account.LoginToken);
                                RmiSend(context, 16, ack);
                            }
                            var authhistory = new AuthHistoryDto()
                            {
                                AccountId = account.Id,
                                Date = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                Hwid = hwid
                            };
                            await db.InsertAsync(authhistory);
                            Log.Information("Credentials verified for {username}; issuing a fresh session ticket", username);
                            if (account.LoginToken != "" && account.LoginToken.Length >= 8)
                            {
                                account.IsConnected = true;
                                await db.UpdateAsync(account);
                            }
                        }
                    }
                    else
                    {
                        Log.Error("Identifier {username} is too short or carries disallowed characters, from {endpoint}", username, context.Channel.RemoteAddress.ToString());
                        SendLoginError(context, "Invalid length of Username/Password.");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Credential verification stopped early because of an unexpected fault");
                SendLoginError(context, "Login Error");
            }
            finally
            {
                _authGate.Release();
            }
        }
        private static BanDto GetActiveBan(AccountDto account)
        {
            var nowUnix = DateTimeOffset.Now.ToUnixTimeSeconds();
            return account.Bans?.FirstOrDefault(b => b.Date + (b.Duration ?? 0) > nowUnix);
        }
        private static void SendLoginError(IChannelHandlerContext ctx, string message)
        {
            var failure = new DMessage();
            failure.Write(false);
            failure.Write(message);
            RmiSend(ctx, 16, failure);
        }
        private static void RmiSend(IChannelHandlerContext ctx, short rmiId, DMessage message)
        {
            var rmiframe = new DMessage();
            rmiframe.Write(DMessage.MessageType.Rmi);
            rmiframe.Write(rmiId);
            rmiframe.Write(message);
            SendA(ctx, rmiframe);
        }
        private static Task SendA(IChannelHandlerContext ctx, DMessage data)
        {
            var coreframe = new DMessage();
            coreframe.Write(FrameMagic);
            coreframe.WriteScalar(data.Length);
            coreframe.Write(data);
            var outBuffer = Unpooled.Buffer(coreframe.Length);
            outBuffer.WriteBytes(coreframe.Buffer);
            try
            {
                ctx.WriteAndFlushAsync(outBuffer).Wait();
            }
            catch (Exception e)
            {
                ctx.FireExceptionCaught(e);
            }
            return Task.CompletedTask;
        }
    }
}
