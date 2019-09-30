using System;

namespace SimpleIdServer.Scim.Exceptions
{
    public class BaseScimException : Exception
    {
        public BaseScimException(string code, string message) : base(message)
        {
            Code = code;
        }

        public string Code { get; }
    }
}
