using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api.Token.Handlers
{
    public class UmaTicketHandler : IGrantTypeHandler
    {
        public const string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:uma-ticket";
        public string GrantType => GRANT_TYPE;

        public Task<JObject> Handle(HandlerContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
