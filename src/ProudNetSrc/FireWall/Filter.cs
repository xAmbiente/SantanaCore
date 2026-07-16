using DotNetty.Transport.Channels;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ProudNetSrc
{
    public class PacketsInfo
    {
        public object message { get; set; }
        public IChannelHandlerContext context { get; set; }
    }

    internal static class PacketFilter
    {

        private static readonly ILogger Logger = Log.ForContext("SourceContext", "Packet Filter");
        private static List<Tuple<PacketsInfo, DateTimeOffset>> packetFillters = new List<Tuple<PacketsInfo, DateTimeOffset>>();
        private static List<(PacketsInfo packetsInfo, DateTimeOffset dateTimeOffset)> PacketFillters = new List<(PacketsInfo packetsInfo, DateTimeOffset dateTimeOffset)>();

        public static void GetPacketFillters(IChannelHandlerContext context, object message)
        {

            if (context == null || message == null)
                return;
            var IP = context.Channel.RemoteAddress.ToString().Substring(0, context.Channel.RemoteAddress.ToString().LastIndexOf(":")).Replace("[::ffff:", "").Replace("]", "");

            var loginTime = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(30);
            packetFillters = packetFillters.Where(x => x.Item2 > loginTime).ToList();

            var countOfRecentLogin = packetFillters.Count(login => login.Item1.message == message && login.Item1.context == context);

            if (countOfRecentLogin > 8)
            {

                return;
            }

            packetFillters.Add(new Tuple<PacketsInfo, DateTimeOffset>(new PacketsInfo() { message = message, context = context }, DateTimeOffset.UtcNow));

        }
    }

    public static class ConnectFilter
    {
        private static readonly ILogger Logger = Log.ForContext("SourceContext", "Connect Filter");
        private static List<Tuple<string, DateTimeOffset>> connectFillters = new List<Tuple<string, DateTimeOffset>>();
        private static List<Tuple<string, DateTimeOffset>> ErrorFillters = new List<Tuple<string, DateTimeOffset>>();
        private static string LastBannedIP;

        public static int GetConnectFillters(string ip)
        {
            if (ip != "127.0.0.1")
            {
                var loginTime = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(40);

                if (connectFillters != null)
                {
                    var connect = connectFillters.Where(x => x?.Item2 > loginTime).ToList();

                    var countOfRecentLogin = connect.Count(login => login?.Item1 == ip);

                    if (countOfRecentLogin > 10)
                    {
                        Logger?.Warning($"Address {ip} exceeded the connection rate threshold and was blacklisted");

                        var ReadIps = File.ReadAllText("IPs.txt");

                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();

                        if (ReadIps.Length == 0 && LastBannedIP != ip)
                        {
                            File.AppendAllText("IPs.txt", ip);

                            startInfo.WorkingDirectory = @"C:\Windows\System32";
                            startInfo.FileName = "cmd.exe";
                            startInfo.Arguments = "/user:Administrator \"cmd /K " + $"netsh advfirewall firewall add rule name=BannedIP's interface=any dir=in action=block remoteip={ip}" + "\"";
                            startInfo.RedirectStandardInput = true;
                            startInfo.RedirectStandardOutput = true;
                            process.StartInfo = startInfo;
                            process.Start();
                            LastBannedIP = ip;
                            process.Close();
                        }
                        else
                        {
                            if (LastBannedIP != ip)
                            {
                                File.AppendAllText("IPs.txt", $",{ip}");

                                startInfo.WorkingDirectory = @"C:\Windows\System32";
                                startInfo.FileName = "cmd.exe";
                                startInfo.Arguments = "/user:Administrator \"cmd /K " + $"netsh advfirewall firewall set rule name=BannedIP's new remoteip={ip}" + "\"";
                                startInfo.RedirectStandardInput = true;
                                startInfo.RedirectStandardOutput = true;
                                process.StartInfo = startInfo;
                                process.Start();
                                LastBannedIP = ip;
                                process.Close();
                            }
                        }
                        return 1;
                    }
                }
                connectFillters.Add(new Tuple<string, DateTimeOffset>(ip, DateTimeOffset.UtcNow));
            }
            return 0;
        }

        public static int GErrorFillters(String ip)
        {
            var LoginTime = DateTimeOffset.UtcNow - TimeSpan.FromSeconds(30);
            var Errorrs = ErrorFillters.Where(x => x?.Item2 > LoginTime).ToList();

            if (Errorrs != null)
            {
                var CountOfRecentLogin = Errorrs.Count(Login => Login?.Item1 == ip);

                if (CountOfRecentLogin > 5)
                {

                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();

                    startInfo.WorkingDirectory = @"C:\Windows\System32";
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/user:Administrator \"cmd /K " + $"netsh advfirewall firewall add rule name=test interface=any dir=in action=block remoteip={ip}" + "\"";
                    process.StartInfo = startInfo;
                    process.Start();
                    process.Close();

                    return 1;
                }
            }
            ErrorFillters.Add(new Tuple<string, DateTimeOffset>(ip, DateTimeOffset.UtcNow));

            return 0;
        }

    }
}
