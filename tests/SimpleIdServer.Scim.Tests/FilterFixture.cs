using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using System.Linq;
using Xunit;

namespace SimpleIdServer.Scim.Tests
{
    public class FilterFixture
    {
        [Fact]
        public void When_Parse_And_Execute_Filter()
        {
            var representations = DataSet.GetSCIMRepresentations();

            var firstResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName eq jsmith");
            var secondResult = ParseAndExecuteFilter(representations.AsQueryable(), "age le 25");
            var thirdResult = ParseAndExecuteFilter(representations.AsQueryable(), "age ge 25");
            var fourthResult = ParseAndExecuteFilter(representations.AsQueryable(), "age lt 25");
            var fifthResult = ParseAndExecuteFilter(representations.AsQueryable(), "age gt 25");
            var sixResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName sw js");
            var sevenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName ew th");
            var eightResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName co sm");
            var nineResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName pr");
            var tenResult = ParseAndExecuteFilter(representations.AsQueryable(), "name.formatted eq formatted");
            var elevenResult = ParseAndExecuteFilter(representations.AsQueryable(), "name.translations.languages.FR eq john");
            var twelveResult = ParseAndExecuteFilter(representations.AsQueryable(), "name.translations.languages.FR eq john and age le 25");
            var thirteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "phone[mobile.value eq 01].brand eq samsung");
            var fourteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "not(userName eq st)");
            var fifteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "not(userName pr)");

            Assert.Equal(1, firstResult.Count());
            Assert.Equal(1, secondResult.Count());
            Assert.Equal(1, thirdResult.Count());
            Assert.Equal(0, fourthResult.Count());
            Assert.Equal(0, fifthResult.Count());
            Assert.Equal(1, sixResult.Count());
            Assert.Equal(1, sevenResult.Count());
            Assert.Equal(1, eightResult.Count());
            Assert.Equal(1, nineResult.Count());
            Assert.Equal(1, tenResult.Count());
            Assert.Equal(1, elevenResult.Count());
            Assert.Equal(1, twelveResult.Count());
            Assert.Equal(1, thirteenResult.Count());
            Assert.Equal(1, fourteenResult.Count());
            Assert.Equal(0, fifteenResult.Count());
        }

        private IQueryable<SCIMRepresentation> ParseAndExecuteFilter(IQueryable<SCIMRepresentation> representations, string filter)
        {
            var parsed = SCIMFilterParser.Parse(filter);
            var evaluatedExpression = parsed.Evaluate(representations);
            return (IQueryable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(representations);
        }
    }
}
