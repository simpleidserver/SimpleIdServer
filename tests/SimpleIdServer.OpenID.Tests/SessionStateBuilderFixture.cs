using SimpleIdServer.OpenID.Api.Authorization;
using System.Linq;
using Xunit;

namespace SimpleIdServer.OpenID.Tests
{
    public class SessionStateBuilderFixture
    {
        [Fact]
        public void Check_Session_State_Is_Correct()
        {
            var sessionState = OpenIDAuthorizationRequestHandler.BuildSessionState("http://localhost:4200", "spa", "00b367a0-350b-4e40-852c-cdeaa3f38303", "f863daeb-61db-4afa-8955-52aaf95df38a");
            Assert.Equal("8c_w5lKq7W_6KrbYd7ZcBxQTzIxGKAJxWKSVF7e2-Zo", sessionState.Split('.').First());
        }
    }
}