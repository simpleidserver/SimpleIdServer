using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Persistence;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMConstants.SCIMEndpoints.Groups)]
    public class GroupsController : BaseApiController
    {
        public GroupsController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, IOptionsMonitor<SCIMHostOptions> options) : base(SCIMConstants.SCIMEndpoints.Users, options.CurrentValue.GroupSchemaIds, addRepresentationCommandHandler, deleteRepresentationCommandHandler, replaceRepresentationCommandHandler, patchRepresentationCommandHandler, scimRepresentationQueryRepository, options)
        {
        }
    }
}
