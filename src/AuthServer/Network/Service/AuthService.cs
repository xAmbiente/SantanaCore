using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SantanaLib.DotNetty.Handlers.MessageHandling;
using SantanaLib.Security.Cryptography;
using SantanaLib.Threading;
using Dapper.FastCrud;
using Santana.Database.Auth;
using Santana.LoginAPI;
using Santana.Network.Message.Auth;
using ProudNetSrc;
using ProudNetSrc.Handlers;
using Serilog;
using Serilog.Core;
using System.Text;

namespace Santana.Network.Service
{
    internal class AuthService : ProudMessageHandler
    {
        public static bool BorrarToken = false;

        private const int MaxLoginFailsPerWindow = 20;
        private const int MaxLoginAttemptsPerWindow = 100;
        private const long LoginWindowMs = 60_000;
        private static readonly ConcurrentDictionary<string, (int attempts, int successes, long windowStartMs)> s_login =
            new ConcurrentDictionary<string, (int, int, long)>();

        private static bool LoginIsBlocked(string ip)
        {
            if (!s_login.TryGetValue(ip, out var e))
                return false;
            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - e.windowStartMs >= LoginWindowMs)
                return false;
            var fails = e.attempts - e.successes;
            return fails > MaxLoginFailsPerWindow || e.attempts > MaxLoginAttemptsPerWindow;
        }

        private static void LoginRegisterAttempt(string ip)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            s_login.AddOrUpdate(ip,
                _ => (1, 0, now),
                (_, e) => (now - e.windowStartMs >= LoginWindowMs) ? (1, 0, now) : (e.attempts + 1, e.successes, e.windowStartMs));
        }

        private static void LoginRegisterSuccess(string ip)
        {
            s_login.AddOrUpdate(ip,
                _ => (1, 1, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
                (_, e) => (e.attempts, e.successes + 1, e.windowStartMs));
        }

        private static readonly ILogger Logger =
            Log.ForContext(Constants.SourceContextPropertyName, nameof(AuthService));


        [MessageHandler(typeof(LoginKRReqMessage))]
        public async Task KRLoginHandler(ProudSession session, LoginKRReqMessage message)
        {
            Logger.Warning("Korean client handshake is not serviced here; answering {endpoint} with a failure code", session.RemoteEndPoint);
            await session.SendAsync(new LoginKRAckMessage(AuthLoginResult.Failed2));
        }


        [MessageHandler(typeof(LoginJPReqMessage))]
        public async Task JPLoginHandler(ProudSession session, LoginJPReqMessage message)
        {
            Logger.Warning("Japanese client handshake is not serviced here; dropping the link to {endpoint}", session.RemoteEndPoint);
            await session.Channel.CloseAsync();
        }

        [MessageHandler(typeof(LoginEUReqMessage))]
        public async Task EULoginHandler(ProudSession session, LoginEUReqMessage message)
        {
            try
            {
                if (message == null)
                {
                    Console.WriteLine("[Gateway] Halting the EU handshake, the request body never materialised");
                    await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.Failed2));
                    return;
                }

                if (session?.RemoteEndPoint?.Address == null)
                {
                    Console.WriteLine("[Gateway] Halting the EU handshake, the peer address is unavailable on this session");
                    return;
                }

                var ip = session.RemoteEndPoint.Address.ToString();

                if (LoginIsBlocked(ip))
                {
                    await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.WrongIdorPw));
                    Logger.Warning("Attempt budget exhausted for {ip}; further tries are refused until the window rolls over", ip);
                    return;
                }
                LoginRegisterAttempt(ip);

                if (IPChecker.Checker(ip) == 1 && ip != "127.0.0.1")
                {
                    await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.WrongIdorPw));
                    Logger.Warning("Address {ip} sits on the deny list; closing the link without evaluating credentials", ip);
                    await session.Channel.CloseAsync();
                    return;
                }

                AccountDto account = null;
                using (var db = AuthDatabase.Open())
                {
                    if (!TryValidateLoginEuMessage(message, ip, out var username, out var password, out var token,
                            out var authToken, out var newToken))
                    {
                        await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.WrongIdorPw));
                        Logger.Error("Field extraction on the handshake body from {ip} did not pass its own checks", ip);
                        return;
                    }

                    if (!IsValidAuthPayload(username, password, token, authToken, newToken))
                    {
                        Console.WriteLine($"[Gateway] Halting the EU handshake, the field mix supplied by {ip} is not a shape this server accepts");
                        await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.WrongIdorPw));
                        Logger.Error("Handshake body from {ip} carries no workable combination of identifier, ticket or session pair", ip);
                        return;
                    }

                    if (username != "" && password != "")
                    {
                        Logger.Debug("Peer {ip} is taking the identifier-and-secret path as {username}", ip, username);

                        if (username.Length > 5 && password.Length > 5)
                        {
                            if (!Namecheck.IsNameValid(username))
                            {
                                Console.WriteLine($"[Gateway] Halting the EU handshake, the identifier [{username}] contains characters the name policy forbids");
                                await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.WrongIdorPw));
                                Logger.Error("Identifier supplied by {ip} was turned down by the name policy", ip);
                                return;
                            }

                            var result = db.Find<AccountDto>(statement => statement
                                .Where($"{nameof(AccountDto.Username):C} = @{nameof(username)}")
                                .Include<BanDto>(join => join.LeftOuterJoin())
                                .WithParameters(new { username }));

                            account = result.FirstOrDefault();
                        }
                        else
                        {
                            Console.WriteLine($"[Gateway] Halting the EU handshake, identifier or secret from {ip} does not reach the minimum length");
                            await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.WrongIdorPw));
                            Logger.Error($"Refusing {message.Username}, the supplied identifier or secret is under the length floor");
                            return;
                        }
                    }

                    else if (token != "")
                    {
                        Logger.Information($"Peer {ip} is resuming through a stored login ticket rather than credentials");

                        var result = await db.FindAsync<AccountDto>(statement => statement
                            .Where($"{nameof(AccountDto.LoginToken):C} = @token")
                            .Include<BanDto>(join => join.LeftOuterJoin())
                            .WithParameters(new { token }));

                        account = result.FirstOrDefault();

                        if (account == null)
                        {
                            await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.Failed2));
                            Logger.Error("The login ticket presented by {ip} matches no account on record", ip);
                            return;
                        }

                        var lastlogin = DateTimeOffset.ParseExact(
                            account.LastLogin,
                            "yyyyMMddHHmmss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None
                        );

                        var diff = (DateTimeOffset.UtcNow - lastlogin).TotalMinutes;

                    }

                    else if (authToken != "" && newToken != "")
                    {
                        Logger.Information("Peer {ip} is resuming through an auth/new token pair", ip);

                        var result = await db.FindAsync<AccountDto>(statement => statement
                            .Where($"{nameof(AccountDto.AuthToken):C} = @{nameof(authToken)} AND {nameof(AccountDto.newToken):C} = @{nameof(newToken)}")
                            .Include<BanDto>(join => join.LeftOuterJoin())
                            .WithParameters(new { authToken, newToken }));

                        account = result.FirstOrDefault();

                        if (account == null)
                        {
                        await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.Failed2));
                        Logger.Error("The token pair presented by {ip} matches no account on record", ip);
                        return;
                        }
                    }

                    if (account == null)
                    {
                        Console.WriteLine($"[Gateway] Halting the EU handshake, none of the lookup paths produced an account record for {ip}");
                        return;
                    }

                    var activeBan = GetActiveBan(account);
                    if (activeBan != null)
                    {
                        var bannedUntil = DateTimeOffset.FromUnixTimeSeconds(activeBan.Date + (activeBan.Duration ?? 0));
                        Console.WriteLine($"[Gateway] Halting the EU handshake, the record for {account.Username} carries a penalty that has not lapsed");
                        await session.SendAsync(new LoginEUAckMessage(bannedUntil));
                        Logger.Warning("Account {user} is serving a penalty; the client has been told when it lifts", account.Username);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(account.Hwid) &&
                        Config.Instance.AuthAPI.BlockedHWIDS?.Contains(account.Hwid) == true)
                    {
                        Console.WriteLine($"[Gateway] Halting the EU handshake, the machine fingerprint stored for {account.Username} is on the deny list");
                        await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.WrongIdorPw));
                        Logger.Warning("Account {user} is tied to a machine fingerprint that administrators have barred", account.Username);
                        return;
                    }

                    session.Authenticated = true;
                    LoginRegisterSuccess(ip);
                    Logger.Information("Session marked as trusted for {user}; the attempt counter has been cleared", account.Username);
                }

                var datetime = $"{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";
                var key = System.Text.RegularExpressions.Regex.Replace(account.Hwid, "[^a-zA-Z]", "");

                var sessionId = Hash.GetUInt32<CRC32>($"<{account.Username}+{account.Password}+{key}>");
                var authsessionId = Hash.GetString<CRC32>($"<{account.Username}+{sessionId}+{datetime}>");
                var newsessionId = Hash.GetString<CRC32>($"<{authsessionId}+{sessionId}>");

                using (var db = AuthDatabase.Open())
                {

                    account.LastLogin = $"{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";
                    if (BorrarToken)
                        account.LoginToken = "";
                    account.AuthToken = authsessionId;
                    account.newToken = newsessionId;
                    account.IsConnected = true;
                    await db.UpdateAsync(account);

                }
        
                await session.SendAsync(new LoginEUAckMessage(AuthLoginResult.OK, (ulong)account.Id, sessionId, authsessionId,
                    newsessionId, datetime));
                Console.WriteLine($"[Gateway] EU handshake complete for {account.Username} (record {account.Id}); tokens persisted and handed back");

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "The EU handshake path threw before finishing; the link is being closed");
                await session.Channel.CloseAsync();
            }
        }

        private static bool IsValidAuthPayload(string username, string password, string token, string authToken, string newToken)
        {
            if (username.Length > 15 || password.Length > 19)
                return false;

            if (token.Length > 128 || authToken.Length > 128 || newToken.Length > 128)
                return false;

            var hasPasswordLogin = username.Length > 0 || password.Length > 0;
            var hasTokenLogin = token.Length > 0;
            var hasSessionLogin = authToken.Length > 0 || newToken.Length > 0;

            return new[] { hasPasswordLogin, hasTokenLogin, hasSessionLogin }.Count(x => x) == 1;
        }

        private static bool TryValidateLoginEuMessage(
            LoginEUReqMessage message,
            string ip,
            out string username,
            out string password,
            out string token,
            out string authToken,
            out string newToken)
        {
            username = string.Empty;
            password = string.Empty;
            token = string.Empty;
            authToken = string.Empty;
            newToken = string.Empty;

            if (!ValidateStringField(nameof(message.Username), message.Username, 15, true, ip) ||
                !ValidateStringField(nameof(message.Password), message.Password, 19, true, ip) ||
                !ValidateStringField(nameof(message.token), message.token, 128, true, ip) ||
                !ValidateStringField(nameof(message.AuthToken), message.AuthToken, 128, true, ip) ||
                !ValidateStringField(nameof(message.NewToken), message.NewToken, 128, true, ip) ||
                !ValidateStringField(nameof(message.DataTime), message.DataTime, 32, true, ip) ||
                !ValidateStringField(nameof(message.Unk1), message.Unk1, 128, true, ip) ||
                !ValidateStringField(nameof(message.Unk2), message.Unk2, 128, true, ip) ||
                !ValidateStringField(nameof(message.Unk6), message.Unk6, 128, true, ip) ||
                !ValidateStringField(nameof(message.Unk8), message.Unk8, 128, true, ip) ||
                !ValidateStringField(nameof(message.Unk9), message.Unk9, 128, true, ip))
            {
                return false;
            }

            username = message.Username?.Trim() ?? string.Empty;
            password = message.Password ?? string.Empty;
            token = CleanToken(message.token);
            authToken = CleanToken(message.AuthToken);
            newToken = CleanToken(message.NewToken);

            Console.WriteLine($"[Gateway] Field extraction done for {ip}; character counts are identifier={username.Length}, secret={password.Length}, ticket={token.Length}, auth={authToken.Length}, renewal={newToken.Length}");
            return true;
        }

        private static bool ValidateStringField(string fieldName, string value, int maxLength, bool allowNull, string ip)
        {
            if (value == null)
            {
                if (allowNull)
                {
                    Console.WriteLine($"[Gateway] Field {fieldName} arrived unset from {ip}; this member is optional, reading it as an empty value");
                    return true;
                }

                Console.WriteLine($"[Gateway] Field {fieldName} arrived unset from {ip} but this member is mandatory");
                return false;
            }

            if (value.Length > maxLength)
            {
                Console.WriteLine($"[Gateway] Field {fieldName} from {ip} spans {value.Length} characters, over the ceiling of {maxLength}");
                return false;
            }

            if (value.Any(char.IsControl))
            {
                Console.WriteLine($"[Gateway] Field {fieldName} from {ip} embeds non-printable characters, which are never legitimate here");
                return false;
            }

            return true;
        }

        private static string CleanToken(string value)
        {
            return (value ?? string.Empty)
                .Trim()
                .Replace("\0", "")
                .Replace("\r", "")
                .Replace("\n", "");
        }

        private static BanDto GetActiveBan(AccountDto account)
        {
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            return account.Bans?.FirstOrDefault(b => b.Date + (b.Duration ?? 0) > now);
        }


        [MessageHandler(typeof(ServerListReqMessage))]
        public async Task ServerListHandler(AuthServer server, ProudSession session)
        {
            if (server == null || session == null)
            {
                Logger.Warning("Cannot answer a node-list query, either the service handle or the session came through unset");
                return;
            }

            await session.SendAsync(new ServerListAckMessage(server.ServerManager.ToArray()));
            Console.WriteLine($"[Gateway] Published the current node roster to {session.RemoteEndPoint}");
        }

        private static byte[] HexStringToByteArray(string hexString)
        {
            hexString = hexString.Replace("-", "");

            var result = new byte[hexString.Length / 2];

            for (var i = 0; i < hexString.Length; i += 2)
                result[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);

            return result;
        }

 
        [MessageHandler(typeof(GameDataReqMessage))]
        public async Task DataHandler(AuthServer server, ProudSession session)
        {
            if (server == null || session == null)
            {
                Logger.Warning("Cannot answer a resource query, either the service handle or the session came through unset");
                return;
            }

            if (session.XbnSent)
            {
                Console.WriteLine($"[Gateway] Skipping a repeat resource query, {session.RemoteEndPoint} already received the full set once");
                return;
            }

            session.XbnSent = true;

            foreach (var xbn in Enum.GetValues(typeof(XBNType)).Cast<XBNType>().ToList())
            {
                if (Program.XBNdata.TryGetValue(xbn, out var xbninfo))
                {
                    var readoffset = 0;
                    while (readoffset != xbninfo.Length)
                    {
                        var size = xbninfo.Length - readoffset;

                        if (size > 40000)
                            size = 40000;

                        var data = new byte[size];
                        Array.Copy(xbninfo, readoffset, data, 0, size);

                        await session.SendAsync(new GameDataAckMessage((uint)xbn, data, (uint)xbninfo.Length), SendOptions.ReliableSecureCompress);
                     
                        readoffset += size;
                    }
                }
            }

            Console.WriteLine($"[Gateway] Every resource bundle has been chunked out to {session.RemoteEndPoint}");
        }

        [MessageHandler(typeof(OptionVersionCheckReqMessage))]
        public async Task OptionVersionCheckHandler(ProudSession session, OptionVersionCheckReqMessage message)
        {
            if (message == null)
            {
                Logger.Warning("Settings-revision query arrived with no body; replying with the stock answer anyway");
                await session.SendAsync(new OptionVersionCheckAckMessage());
                return;
            }

            if (message.AccountId == 0)
                Console.WriteLine($"[Gateway] Settings-revision query from {session.RemoteEndPoint} names record zero, which no real account uses");

            await session.SendAsync(new OptionVersionCheckAckMessage());
            Console.WriteLine($"[Gateway] Settings revision reported back to {session.RemoteEndPoint}");
        }

    }
}
