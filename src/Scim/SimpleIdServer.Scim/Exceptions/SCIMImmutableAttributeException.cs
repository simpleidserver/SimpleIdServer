namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMImmutableAttributeException : BaseScimException
    {
        public SCIMImmutableAttributeException(string message) : base("mutability", message)
        {
        }
    }
}
