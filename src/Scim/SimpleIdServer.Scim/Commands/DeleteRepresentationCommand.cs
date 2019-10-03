namespace SimpleIdServer.Scim.Commands
{
    public class DeleteRepresentationCommand
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
