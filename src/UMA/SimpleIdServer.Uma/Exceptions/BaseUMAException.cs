using System;

namespace SimpleIdServer.Uma.Exceptions
{
    public class BaseUMAException : Exception
    {
        public BaseUMAException() { }

        public BaseUMAException(string message) : base(message) { }
    }
}
