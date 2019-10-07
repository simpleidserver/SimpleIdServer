namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMAttributeException : BaseScimException
    {
        public SCIMAttributeException(string message) : base("badAttribute", message) { }
    }
}
