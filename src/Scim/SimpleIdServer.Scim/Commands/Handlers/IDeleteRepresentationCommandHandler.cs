using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public interface IDeleteRepresentationCommandHandler
    {
        Task Handle(DeleteRepresentationCommand request);
    }
}
