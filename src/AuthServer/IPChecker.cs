using Dapper.FastCrud;
using Santana.Database.Auth;
using System;
using System.Linq;
using System.Net;

namespace Santana
{
    internal class IPChecker
    {
        internal static int Checker(string context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(context) || !IPAddress.TryParse(context, out var parsed))
                {
                    Console.WriteLine($"[AddressFilter] denying request, the value [{context}] is blank or not a parseable address");
                    return 1;
                }

                if (IPAddress.IsLoopback(parsed))
                    return 0;

                using (var auth = AuthDatabase.Open())
                {
                    var bannedRow = auth.Find<IPBanDto>(statement => statement
                        .Where($"{nameof(IPBanDto.IP):C} = @{nameof(context)}")
                        .WithParameters(new { context })).FirstOrDefault();

                    if (bannedRow != null && bannedRow.IP == context)
                    {
                        Console.WriteLine($"[AddressFilter] denying request, address {context} matches an entry in the ban table");
                        return 1;
                    }
                }

                return 0;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return 0;
            }
        }
    }
}
