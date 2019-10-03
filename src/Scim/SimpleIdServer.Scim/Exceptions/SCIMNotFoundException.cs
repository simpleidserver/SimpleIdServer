namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMNotFoundException : BaseScimException
    {
        public SCIMNotFoundException(string code, string message) : base(code, message)
        {
        }
    }
}
