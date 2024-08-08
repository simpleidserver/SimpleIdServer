using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Mobile.Clients;
using SimpleIdServer.Mobile.DTOs;
using SimpleIdServer.Mobile.Resources;

namespace SimpleIdServer.Mobile.Services;

public interface IVerifiableCredentialsService
{
    Task<RequestVerifiableCredentialResult> Request(BaseCredentialOffer credentialOffer, string publicDid, IAsymmetricKey privateKey);
}

public class RequestVerifiableCredentialResult
{
    private RequestVerifiableCredentialResult()
    {
        
    }

    public BaseCredentialResult VerifiableCredential { get; private set; }
    public string ErrorMessage { get; private set; }

    public static RequestVerifiableCredentialResult Ok(BaseCredentialResult result) => new RequestVerifiableCredentialResult { VerifiableCredential = result };
    public static RequestVerifiableCredentialResult Nok(string errorMessage) => new RequestVerifiableCredentialResult { ErrorMessage = errorMessage };
}

public abstract class BaseGenericVerifiableCredentialService<T> : IVerifiableCredentialsService where T : BaseCredentialOffer
{
    private readonly ICredentialIssuerClient _credentialIssuerClient;
    private readonly ISidServerClient _sidServerClient;

    public BaseGenericVerifiableCredentialService(ICredentialIssuerClient credentialIssuerClient, ISidServerClient sidServerClient)
    {
        _credentialIssuerClient = credentialIssuerClient;
        _sidServerClient = sidServerClient;
    }

    public abstract string Version { get; }
    protected ICredentialIssuerClient CredentialIssuerClient
    {
        get
        {
            return _credentialIssuerClient;
        }
    }

    public async Task<RequestVerifiableCredentialResult> Request(BaseCredentialOffer credentialOffer, string publicDid, IAsymmetricKey privateKey)
    {        
        if(!TryValidate(credentialOffer, out string errorMessage))
        {
            return RequestVerifiableCredentialResult.Nok(errorMessage);
        }

        var extractionResult = await Extract(credentialOffer as T);
        if(extractionResult == null)
        {
            return RequestVerifiableCredentialResult.Nok(Global.CannotExtractCredentialDefinition);
        }

        if(credentialOffer.Grants.PreAuthorizedCodeGrant != null)
        {
            return await RequestWithPreAuthorizedCodeGrant(publicDid, privateKey, credentialOffer, extractionResult.Value.credIssuer, extractionResult.Value.credDef);
        }

        // MANAGE IMMEDIATE.
        return null;
    }

    protected abstract Task<(BaseCredentialDefinitionResult credDef, BaseCredentialIssuer credIssuer)?> Extract(T credentialOffer);

    protected abstract Task<BaseCredentialResult> GetCredential(BaseCredentialIssuer credentialIssuer, BaseCredentialDefinitionResult credentialDefinition, CredentialProofRequest proofRequest, string accessToken);

    private bool TryValidate<T>(T credentialOffer, out string errorMessage) where T : BaseCredentialOffer
    {
        errorMessage = null;
        if(credentialOffer.Grants == null ||
            (credentialOffer.Grants.PreAuthorizedCodeGrant == null && credentialOffer.Grants.AuthorizedCodeGrant == null))
        {
            errorMessage = Global.GrantsCannotBeNull;
            return false;
        }

        if(credentialOffer.Grants.PreAuthorizedCodeGrant != null && string.IsNullOrWhiteSpace(credentialOffer.Grants.PreAuthorizedCodeGrant.PreAuthorizedCode))
        {
            errorMessage = Global.PreAuthorizedCodeMissing;
            return false;
        }

        if(!credentialOffer.HasOneCredential())
        {
            errorMessage = Global.CredentialOfferMustContainsOneCredential;
            return false;
        }

        return true;
    }

    private async Task<RequestVerifiableCredentialResult> RequestWithPreAuthorizedCodeGrant<T>(string publicDid, IAsymmetricKey privateKey, T credentialOffer, BaseCredentialIssuer credentialIssuer, BaseCredentialDefinitionResult credDerf) where T : BaseCredentialOffer
    {
        var tokenResult = await _sidServerClient.GetAccessTokenWithPreAuthorizedCode(credentialIssuer.AuthorizationServers.First(), credentialOffer.Grants.PreAuthorizedCodeGrant.PreAuthorizedCode);
        if (tokenResult == null)
        {
            return null;
        }

        var proofOfPossession = BuildProofOfPossession(publicDid, privateKey, credentialIssuer, tokenResult.Value.cNonce);
        var result = await GetCredential(credentialIssuer, credDerf, new CredentialProofRequest { ProofType = "jwt", Jwt = proofOfPossession }, tokenResult.Value.accessToken);
        return RequestVerifiableCredentialResult.Ok(result);
    }

    private string BuildProofOfPossession(string publicDid, IAsymmetricKey privateKey, BaseCredentialIssuer credentialIssuer, string cNonce)
    {
        var signingCredentials = privateKey.BuildSigningCredentials(publicDid);
        var handler = new JsonWebTokenHandler();
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = signingCredentials,
            Audience = credentialIssuer.CredentialIssuer,
            TokenType = "openid4vci-proof+jwt"
        };
        var claims = new Dictionary<string, object>
        {
            { "iss", publicDid },
            { "nonce", cNonce }
        };
        securityTokenDescriptor.Claims = claims;
        return handler.CreateToken(securityTokenDescriptor);
    }
}