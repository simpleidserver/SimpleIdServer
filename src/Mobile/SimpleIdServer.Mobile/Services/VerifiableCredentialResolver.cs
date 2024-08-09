using System.Text.Json.Nodes;

namespace SimpleIdServer.Mobile.Services;

public interface IVerifiableCredentialResolver
{
    Task<RequestVerifiableCredentialResult> Resolve(string credentialOffer);
}

public class VerifiableCredentialResolver : IVerifiableCredentialResolver
{
    private readonly IEnumerable<IVerifiableCredentialsService> _verifiableCredentialsServices;

    public VerifiableCredentialResolver(IEnumerable<IVerifiableCredentialsService> verifiableCredentialsServices)
    {
        _verifiableCredentialsServices = verifiableCredentialsServices;
    }

    public Task<RequestVerifiableCredentialResult> Resolve(string credentialOffer)
    {
        var version = SupportedVcVersions.LATEST;
        var jsonObj = JsonObject.Parse(credentialOffer).AsObject();
        if(jsonObj.ContainsKey("credentials")) version = SupportedVcVersions.ESBI;
        var service = _verifiableCredentialsServices.Single(v => v.Version ==  version);
        return service.Request(credentialOffer, null, null);        
    }
}