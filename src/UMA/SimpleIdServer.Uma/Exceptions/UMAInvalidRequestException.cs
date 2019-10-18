using SimpleIdServer.OAuth.Exceptions;

namespace SimpleIdServer.Uma.Exceptions
{
    public class UMAInvalidRequestException : OAuthException
    {
        public UMAInvalidRequestException(string message) : base(string.Empty, message)
        {
        }
    }
}
