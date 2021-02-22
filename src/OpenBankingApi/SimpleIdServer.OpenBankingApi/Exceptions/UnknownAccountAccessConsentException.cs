namespace SimpleIdServer.OpenBankingApi.Exceptions
{
    public class UnknownAccountAccessConsentException : NotFoundException
    {
        public UnknownAccountAccessConsentException(string message) : base(message)
        {

        }
    }
}
