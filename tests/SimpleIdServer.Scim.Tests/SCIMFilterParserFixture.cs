using SimpleIdServer.Scim.Parser;
using Xunit;

namespace SimpleIdServer.Scim.Tests
{
    public class SCIMFilterParserFixture
    {
        [Fact]
        public void When_Parse_Filters()
        {
            var firstFilter = SCIMFilterParser.Parse("members[display eq \"Babs Jensen\" and value co \"2819\"]");
            Assert.NotNull(firstFilter);
        }
    }
}
