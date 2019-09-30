namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMUniquenessAttributeException : BaseScimException
    {
        public SCIMUniquenessAttributeException(string code, string message) : base(code, message)
        {
        }
    }
}
