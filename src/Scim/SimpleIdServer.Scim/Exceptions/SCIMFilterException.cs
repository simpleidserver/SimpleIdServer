namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMFilterException : BaseScimException
    {
        public SCIMFilterException(string code, string message) : base(code, message) { }
    }
}
