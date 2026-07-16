using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ProudNetSrc
{
    public static class IPChecker
    {

        public static List<string> AllowedIPs = new List<string>();
        public static bool IPCheck(string ip)
        {
            try
            {
                if (ip == "127.0.0.1" || AllowedIPs.Contains(ip))
                    return false;
                Int32 status = 0;
                var rnd = new Random();
                var ReadIps = File.ReadAllText("BlockIPs.txt");
                var API = new string[] { "NTUwOTphNUF1Tkh0ZXNqUzVmODZmTkNmN2lGeTZySmZVeUZjdA==",
     "NDk5MDozanZFcDhucjd4UFJzTm8zTkZVWXExR0h1TjBzVzhBSg==",
     "NDk4OTo0cHZGSFhFeWFtY2dCbTFUSHV0alRhcDliY3l6WlA3NQ==",
     "NDk4NjpnajVxNkdIZXFTZks0R0NiZUtJb3lJeU44a1BTNXFYUg==",
     "NDk4MjpGVnFTVUhnWXVYdXR5ZkxRRHJSa05uVGlpMXpoMzBsSQ==",
     "NDk4Mzo1VGdTMTZsbjNRNWhKY3pJVTBTbjdHNnphUTh1cHRlaQ==",
     "NDk4NDpoSGFxQ2hpcWxaY3J1V3g5SGxKUkF4aVpQTDBscmV1eA==",
     "NDk4MTpoSDhJa1k2dGxRQ1hmajNEVHBTZmRlYjZiSFdhN21PUQ==",
     "NDY0NTp3enBLSnI5Ulh0NjRLNlVOTHVJeVBRVTJwbFdIU3NuVw==",
     "NDY0NjpKN056R1VuQ2gyUENvVWtqR3p0MVFaaEFUcUdNeDBrNg==",
     "NDY2MDp4QUN0a1BLZFhvanY3bmNKQTdZYkRhbFZ6RUZUdjRjRw==",
     "NDU4NDpTZnU3SEVVUEhISE80RzRIZTZEQXlxY2l6cTBqcklySQ==",
     "NTUxMDpLNkF6ZzB4YnZsZ1VSY0dRZnVUU0hwVjVaR0JTS0I5aA==",
     "NTUxMTpWZTIweHhlTXIzM0M1dTZWNVN5QmNmZVo5bTJHcHZBRA==",
     "NTUxMjoxZmJzY284Q2VHRlBMS3FYUjE4WjJjczlYcndJd0Q0ZA==",
     "NTUxMzpWUmg1RkxZSmp4OXNuaFREOE9BU2xWUlB4RDBqTzgyag==",
     "NTUxNDo4MVoyd2JMOXZXQUhnR3VTRHo1SFUwTzNLd1lrZmpDSg==",
     "NTUxNTpBOEU2MXBQUWh0M0dOZWJiSjB6T1hmSFlVdGdoRVJRbw==",
     "NTUxNjp2M2VwVGFqTU9rMFRQRnl1R2dVZHZTMWlweGpiOEw2Ug==",
     "NTUxNzpUU2xsa0E2MnBNN1UwRUF5OFR5dWxCTVM4ekJJenpNWQ==" };

                var request = (HttpWebRequest)WebRequest.Create($"http://v2.api.iphub.info/ip/" + ip);

                request.Method = "Get";
                request.Timeout = 12000;
                request.ContentType = "application/vnd.twitchtv.v5+json";
                request.Headers.Add("X-key", API[rnd.Next(0, API.Count())]);

                using (var s = request.GetResponse().GetResponseStream())
                using (var sr = new System.IO.StreamReader(s))
                {
                    string info = sr.ReadToEnd();

                    status = Convert.ToInt32(info.Split(new string[] { "block\":" }, StringSplitOptions.None)[1].Split(',')[0].Trim());

                    if (status > 0)
                    {
                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();

                        if (ReadIps.Length == 0)
                        {
                            File.WriteAllText("BlockIPs.txt", ip);

                            startInfo.UseShellExecute = false;
                            startInfo.WorkingDirectory = @"C:\Windows\System32";
                            startInfo.FileName = "cmd.exe";
                            startInfo.Arguments = "/user:Administrator \"cmd /K " + $"netsh advfirewall firewall add rule name=BannedIPs interface=any dir=in action=block remoteip={ip}" + "\"";
                            startInfo.RedirectStandardInput = true;
                            startInfo.RedirectStandardOutput = true;
                            startInfo.CreateNoWindow = true;
                            process.StartInfo = startInfo;
                            process.Start();

                            startInfo.WorkingDirectory = @"C:\Windows\System32";
                            startInfo.FileName = "cmd.exe";
                            startInfo.Arguments = "/user:Administrator \"cmd /K " + $"netsh advfirewall firewall add rule name=BannedIPs interface=any dir=in action=block remoteip={ip}" + "\"";
                            startInfo.RedirectStandardInput = true;
                            startInfo.RedirectStandardOutput = true;
                            process.StartInfo = startInfo;
                            process.Start();
                            
                            process.Close();
                            return true;
                        }
                        else
                        {
                            
                            
                            File.AppendAllText("BlockIPs.txt", $",{ip}");

                            startInfo.UseShellExecute = false;

                            startInfo.WorkingDirectory = @"C:\Windows\System32";
                            startInfo.FileName = "cmd.exe";
                            startInfo.Arguments = "/user:Administrator \"cmd /K " + $"netsh advfirewall firewall set rule name=BannedIPs new remoteip={ReadIps}" + "\"";
                            startInfo.RedirectStandardInput = true;
                            startInfo.RedirectStandardOutput = true;
                            startInfo.CreateNoWindow = true;
                            process.StartInfo = startInfo;
                            process.Start(); 
                            process.Close();
                            return true;
                        }
                    }
                    AllowedIPs.Add(ip);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }
    }
}