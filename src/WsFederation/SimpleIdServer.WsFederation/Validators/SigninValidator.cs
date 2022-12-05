using Microsoft.IdentityModel.Protocols.WsFederation;

namespace SimpleIdServer.WsFederation.Validators
{
    public interface ISigninValidator
    {

    }

    public class SigninValidator : ISigninValidator
    {
        public SigninValidator()
        {

        }

        public async Task Validate(WsFederationMessage message)
        {
            // Check client...
            message.Wtrealm;
        }
    }
}
