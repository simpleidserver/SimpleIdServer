using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Serialization;
using Xunit;

namespace SimpleIdServer.Scim.Host.Acceptance.Tests
{
    public class SCIMSerializerFixture
    {
        [Fact]
        public void When_Serialize_Scim()
        {
            var serializer = new SCIMSerializer();
            var json = serializer.Serialize(new SCIMErrorRepresentation("status", "scimType", "detail"));
            string s = "";
        }
    }
}