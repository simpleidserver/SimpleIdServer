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
            var secondFilter = SCIMFilterParser.Parse("(id eq user1 or id eq user2) or (id eq user3 or id eq user4) or (id eq user5 or id eq user6)");
            var thirdFilter = SCIMFilterParser.Parse("(id eq user1 or id eq user2) or ((id eq user3 or id eq user4) or (id eq user5 or id eq user6))");
            var fourthFilter = SCIMFilterParser.Parse("(id eq user1 or id eq user2) or ((id eq user3 or id eq user4) or ((id eq user5 or id eq user6) and (id eq user5 or id eq user6)))");
            Assert.NotNull(firstFilter);
            Assert.NotNull(secondFilter);
            Assert.NotNull(thirdFilter);
            Assert.NotNull(fourthFilter);
        }
    }
}
