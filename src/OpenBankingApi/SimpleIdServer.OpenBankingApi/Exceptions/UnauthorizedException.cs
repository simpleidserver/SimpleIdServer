using System;

namespace SimpleIdServer.OpenBankingApi.Exceptions
{
    public class UnauthorizedException: Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
