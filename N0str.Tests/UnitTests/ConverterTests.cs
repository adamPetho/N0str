using N0str.Converter;
using System.Globalization;

namespace N0str.Tests.UnitTests
{
    public class ConverterTests
    {
        [Fact]
        public void BoolToChevronConverter_True_Test()
        {
            var value = true;
            var converter = new BoolToChevronConverter();

            Assert.Equal("▾", converter.Convert(value, typeof(bool), parameter: null, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void BoolToChevronConverter_False_Test()
        {
            var value = false;
            var converter = new BoolToChevronConverter();

            Assert.Equal("▸", converter.Convert(value, typeof(bool), parameter: null, CultureInfo.InvariantCulture));
        }
    }
}
