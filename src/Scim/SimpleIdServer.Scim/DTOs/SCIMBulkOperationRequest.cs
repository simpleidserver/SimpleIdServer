namespace SimpleIdServer.Scim.DTOs
{
    public class SCIMBulkOperationRequest
    {
        public SCIMBulkOperationRequest()
        {

        }

        public string HttpMethod { get; set; }
        public string BulkIdentifier { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
    }
}
