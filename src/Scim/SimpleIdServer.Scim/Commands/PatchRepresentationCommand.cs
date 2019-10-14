using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class PatchRepresentationCommand : ISCIMCommand<SCIMRepresentation>
    {
        public PatchRepresentationCommand(string id, JObject content)
        {
            Id = id;
            Content = content;
        }

        public string Id { get; private set; }
        public JObject Content { get; private set; }
    }
}
