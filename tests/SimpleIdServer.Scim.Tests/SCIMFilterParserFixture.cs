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
            var secondFilter = SCIMFilterParser.Parse("emails[type eq \"work\" and value co \"@example.com\"]");
            var thirdFilter = SCIMFilterParser.Parse("phones[phoneNumber eq \"02\"]");
            var fourthFilter = SCIMFilterParser.Parse("phones[phoneNumber eq 02]");
            var fifthFilter = SCIMFilterParser.Parse("emails[type co \"O'Malley\"]");
            Assert.NotNull(firstFilter);
            Assert.NotNull(secondFilter);
            Assert.NotNull(thirdFilter);
            Assert.NotNull(fourthFilter);
            Assert.NotNull(fifthFilter);
        }
    }
}
