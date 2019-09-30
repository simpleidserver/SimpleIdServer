namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMBadRequestException : BaseScimException
    {
        public SCIMBadRequestException(string code, string message) : base(code, message) { }
    }
}
