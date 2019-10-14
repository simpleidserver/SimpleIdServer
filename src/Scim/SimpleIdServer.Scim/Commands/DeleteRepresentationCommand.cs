using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class DeleteRepresentationCommand : ISCIMCommand<bool>
    {
        public DeleteRepresentationCommand(string id, string resourceType)
        {
            Id = id;
            ResourceType = resourceType;
        }

        public string Id { get; set; }
        public string ResourceType { get; set; }
    }
}
