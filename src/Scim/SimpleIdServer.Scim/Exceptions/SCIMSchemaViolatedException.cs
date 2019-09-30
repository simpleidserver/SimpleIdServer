namespace SimpleIdServer.Scim.Exceptions
{
    public class SCIMSchemaViolatedException : BaseScimException
    {
        public SCIMSchemaViolatedException(string code, string message) : base(code, message) { }
    }
}