using Microsoft.Extensions.Options;

namespace SimpleIdServer.Mobile.Services;

public interface IVerifiableCredentialFactory
{

}

public class VerifiableCredentialFactory : IVerifiableCredentialFactory
{
    private readonly MobileOptions _options;

    public VerifiableCredentialFactory(IOptions<MobileOptions> options)
    {
        _options = options.Value;
    }
}
