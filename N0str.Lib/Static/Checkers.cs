using NNostr.Client.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Static
{
    public static class Checkers
    {
        public static bool IsValidProfileIdentifier(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                if (input.StartsWith("npub"))
                {
                    NIP19.FromNIP19Npub(input);
                    return true;
                }

                if (input.StartsWith("nprofile"))
                {
                    NIP19.FromNIP19Note(input);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
