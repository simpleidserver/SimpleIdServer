using SimpleIdServer.Scim.Infrastructure;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public interface IDeleteRepresentationCommandHandler : ISCIMCommandHandler<DeleteRepresentationCommand, bool>
    {
    }
}
