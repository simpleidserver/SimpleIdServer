using System;

namespace SimpleIdServer.OpenBankingApi.Domains
{
    public class BusinessRuleValidationException : Exception
    {
        public BusinessRuleValidationException(string message) : base(message) { }
    }
}
