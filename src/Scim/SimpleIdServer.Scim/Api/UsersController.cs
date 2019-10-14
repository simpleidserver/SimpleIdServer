using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Persistence;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMConstants.SCIMEndpoints.Users)]
    public class UsersController : BaseApiController
    {
        public UsersController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, IOptionsMonitor<SCIMHostOptions> options) : base(SCIMConstants.SCIMEndpoints.Users, options.CurrentValue.UserSchemasIds, addRepresentationCommandHandler, deleteRepresentationCommandHandler, replaceRepresentationCommandHandler, patchRepresentationCommandHandler, scimRepresentationQueryRepository, options)
        {
        }
    }
}