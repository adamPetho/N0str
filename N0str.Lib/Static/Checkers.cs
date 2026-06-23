using NNostr.Client;
using NNostr.Client.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NNostr.Client.Protocols.NIP19;

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

        public static bool TryExtractIdentifier(string input, [NotNullWhen(true)] out string? pubkeyHex)
        {
            pubkeyHex = "";
            if (input.StartsWith("npub"))
            {
                pubkeyHex = NIP19.FromNIP19Npub(input).ToHex();
                return true;
            }

            if (input.StartsWith("nprofile"))
            {
                if (NIP19.FromNIP19Note(input) is NosteProfileNote profile)
                {
                    pubkeyHex = profile.PubKey;
                }
                return true;
            }

            return false;
        }
    }
}
