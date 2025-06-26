using System.Collections.Generic;

namespace SimpleIdServer.IdServer
{
    public interface IPasswordValidationService
    {
        List<(string code, string errorMessage)>? Validate(string password);
    }

    public class DefaultPasswordValidationService : IPasswordValidationService
    {
        public List<(string code, string errorMessage)>? Validate(string password)
        {
            return null;
        }
    }
}
