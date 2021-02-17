using System;

namespace SimpleIdServer.OpenBankingApi.Exceptions
{
    public class UnknownAccountException : NotFoundException
    {
        public UnknownAccountException(string message) : base(message)
        {

        }
    }
}
