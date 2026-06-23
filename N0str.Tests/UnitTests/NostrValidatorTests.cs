using N0str.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N0str.Tests.UnitTests
{
    public class NostrValidatorTests
    {
        [Theory]
        [InlineData("npub1jw7scmeuewhywwytqxkxec9jcqf3znw2fsyddcn3948lw9q950ps9y35fg")]
        [InlineData("nprofile1qqsf80gvdu7vhtj88z9srtrvuzevqyc3fh9yczxkufcj6nlhzsz68scmupga8")]
        public void CanParseNpub(string value)
        {
            var result = Checkers.IsValidProfileIdentifier(value);

            Assert.True(result);
        }

        [Theory]
        [InlineData("npub1jw7scmeuewhywwytqxkxec9jcqf3znw2fsyddcn3948lw9q950ps9y35fg")]
        [InlineData("nprofile1qqsf80gvdu7vhtj88z9srtrvuzevqyc3fh9yczxkufcj6nlhzsz68scmupga8")]
        public void CanGetSamePubKeyFromNpubAndNprofile(string value)
        {
            var expectedPubkey = "93bd0c6f3ccbae47388b01ac6ce0b2c013114dca4c08d6e2712d4ff71405a3c3";
            var res = Checkers.TryExtractIdentifier(value, out string? hex);

            Assert.True(res);
            Assert.NotNull(hex);
            Assert.Equal(expectedPubkey, hex);
        }
    }
}
