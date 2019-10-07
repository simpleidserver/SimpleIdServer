using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Persistence;

namespace SimpleIdServer.Scim.Api.User
{
    [Route(SCIMConstants.SCIMEndpoints.Users)]
    public class UsersController : BaseApiController
    {
        public UsersController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, IOptionsMonitor<SCIMHostOptions> options) : base(SCIMConstants.SCIMEndpoints.Users, addRepresentationCommandHandler, deleteRepresentationCommandHandler, scimRepresentationQueryRepository, options)
        {
        }
    }
}