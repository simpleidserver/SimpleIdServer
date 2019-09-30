using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public interface IAddRepresentationCommandHandler : ISCIMCommandHandler<AddRepresentationCommand, SCIMRepresentation>
    {
    }
}
