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
    }
}
