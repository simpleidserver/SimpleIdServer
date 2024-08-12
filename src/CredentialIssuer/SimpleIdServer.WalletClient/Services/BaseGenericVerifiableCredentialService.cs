// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.WalletClient.Clients;
using SimpleIdServer.WalletClient.DTOs;
using SimpleIdServer.WalletClient.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.WalletClient.Services;

public interface IVerifiableCredentialsService
{
    Task<RequestVerifiableCredentialResult> Request(string credentialOfferJson, string publicDid, IAsymmetricKey privateKey, CancellationToken cancellationToken);
    string Version { get; }
}

public record RequestVerifiableCredentialResult
{
    private RequestVerifiableCredentialResult()
    {
        
    }

    public CredentialIssuerResult VerifiableCredential { get; private set; }
    public string ErrorMessage { get; private set; }

    public static RequestVerifiableCredentialResult Ok(CredentialIssuerResult result) => new RequestVerifiableCredentialResult { VerifiableCredential = result };
    public static RequestVerifiableCredentialResult Nok(string errorMessage) => new RequestVerifiableCredentialResult { ErrorMessage = errorMessage };
}

public abstract class BaseGenericVerifiableCredentialService<T> : IVerifiableCredentialsService where T : BaseCredentialOffer
{
    private readonly ICredentialIssuerClient _credentialIssuerClient;
    private readonly ISidServerClient _sidServerClient;
    private readonly IEnumerable<IDeferredCredentialIssuer> _issuers;
    private readonly IEnumerable<IDidResolver> _resolvers;

    public BaseGenericVerifiableCredentialService(ICredentialIssuerClient credentialIssuerClient, ISidServerClient sidServerClient, IEnumerable<IDidResolver> resolvers)
    {
        _credentialIssuerClient = credentialIssuerClient;
        _sidServerClient = sidServerClient;
        _resolvers = resolvers;
    }

    public abstract string Version { get; }
    protected ICredentialIssuerClient CredentialIssuerClient
    {
        get
        {
            return _credentialIssuerClient;
        }
    }

    public async Task<RequestVerifiableCredentialResult> Request(string credentialOfferJson, string publicDid, IAsymmetricKey privateKey, CancellationToken cancellationToken)
    {
        var resolver = _resolvers.SingleOrDefault(r => publicDid.StartsWith($"did:{r.Method}"));
        if(resolver == null)
        {
            return RequestVerifiableCredentialResult.Nok(Global.DidFormatIsNotSupported);
        }

        var credentialOffer = JsonSerializer.Deserialize<T>(credentialOfferJson);
        if(!TryValidate(credentialOffer, out string errorMessage))
        {
            return RequestVerifiableCredentialResult.Nok(errorMessage);
        }

        var extractionResult = await Extract(credentialOffer as T, cancellationToken);
        if(extractionResult == null)
        {
            return RequestVerifiableCredentialResult.Nok(Global.CannotExtractCredentialDefinition);
        }

        var didDocument = await resolver.Resolve(publicDid, cancellationToken);
        var credIssuer = extractionResult.Value.credIssuer;
        var authorizationServers = credIssuer.GetAuthorizationServers();
        if (authorizationServers == null || authorizationServers.Count == 0) return RequestVerifiableCredentialResult.Nok(Global.AuthorizationServerCannotBeResolved);
        var authorizationServer = authorizationServers.First();
        var openidConfiguration = await _sidServerClient.GetOpenidConfiguration(authorizationServer, cancellationToken);
        CredentialTokenResult tokenResult;
        if(credentialOffer.Grants.PreAuthorizedCodeGrant != null)
        {
            tokenResult = await GetTokenWithPreAuthorizedCodeGrant(didDocument.Id, privateKey, credentialOffer, extractionResult.Value.credIssuer, openidConfiguration, extractionResult.Value.credDef, cancellationToken);
        }
        else
        {
            tokenResult = await GetTokenWithAuthorizationCodeGrant(didDocument, privateKey, credentialOffer, extractionResult.Value.credDef, extractionResult.Value.credIssuer, openidConfiguration, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(tokenResult.ErrorMessage)) return RequestVerifiableCredentialResult.Nok(tokenResult.ErrorMessage);

        var proofOfPossession = BuildProofOfPossession(didDocument, privateKey, extractionResult.Value.credIssuer, tokenResult.Token.CNonce);
        var result = await GetCredential(extractionResult.Value.credIssuer, extractionResult.Value.credDef, new CredentialProofRequest { ProofType = "jwt", Jwt = proofOfPossession }, tokenResult.Token.AccessToken, cancellationToken);
        if (result == null) return RequestVerifiableCredentialResult.Nok(Global.CannotGetCredential);
        var transaction = result.GetTransactionId();
        if(!string.IsNullOrWhiteSpace(transaction))
        {
            var issuer = _issuers.Single(r => r.Version == Version);
            // TODO
            return null;
        }

        var credential = JsonSerializer.Deserialize<W3CVerifiableCredential>(result.Credential);
        return RequestVerifiableCredentialResult.Ok(CredentialIssuerResult.Issue(credential));
    }

    protected abstract Task<(BaseCredentialDefinitionResult credDef, DTOs.BaseCredentialIssuer credIssuer)?> Extract(T credentialOffer, CancellationToken cancellationToken);

    protected abstract Task<BaseCredentialResult> GetCredential(DTOs.BaseCredentialIssuer credentialIssuer, BaseCredentialDefinitionResult credentialDefinition, CredentialProofRequest proofRequest, string accessToken, CancellationToken cancellationToken);

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

    #region Pre-authorized code

    private async Task<CredentialTokenResult> GetTokenWithPreAuthorizedCodeGrant<T>(string publicDid, IAsymmetricKey privateKey, T credentialOffer, DTOs.BaseCredentialIssuer credentialIssuer, OpenidConfigurationResult openidConfigurationResult, BaseCredentialDefinitionResult credDerf, CancellationToken cancellationToken) where T : BaseCredentialOffer
    {
        var tokenResult = await _sidServerClient.GetAccessTokenWithPreAuthorizedCode(publicDid, openidConfigurationResult.TokenEndpoint, credentialOffer.Grants.PreAuthorizedCodeGrant.PreAuthorizedCode, cancellationToken);
        if (tokenResult == null)
            return CredentialTokenResult.Nok(Global.CannotGetTokenWithPreAuthorizedCode);

        return CredentialTokenResult.Ok(tokenResult);
    }

    #endregion

    #region Authorization code grant

    private async Task<CredentialTokenResult> GetTokenWithAuthorizationCodeGrant(DidDocument didDocument, IAsymmetricKey privateKey, T credentialOffer, BaseCredentialDefinitionResult credentialDefinition, DTOs.BaseCredentialIssuer credentialIssuer, OpenidConfigurationResult openidConfigurationResult, CancellationToken cancellationToken)
    {
        var (challenge, verifier) = GeneratePkce();
        var parameters = BuildAuthorizationRequestParameters(didDocument, credentialOffer.Grants.AuthorizedCodeGrant.IssuerState, credentialDefinition, credentialIssuer, challenge);
        var authorizationResult = await _sidServerClient.GetAuthorization(openidConfigurationResult.AuthorizationEndpoint, parameters, cancellationToken);
        if (authorizationResult == null) return CredentialTokenResult.Nok(Global.BadAuthorizationRequest);
        var postAuthResult = await ExecutePostAuthorizationRequest(didDocument, privateKey, credentialIssuer, authorizationResult["redirect_uri"], authorizationResult["nonce"], authorizationResult["state"], cancellationToken);
        if (postAuthResult == null) return CredentialTokenResult.Nok(Global.BadPostAuthorizationRequest);
        var tokenResult = await _sidServerClient.GetAccessTokenWithAuthorizationCode(openidConfigurationResult.TokenEndpoint, didDocument.Id, postAuthResult["code"], verifier, cancellationToken);
        if (tokenResult == null) return CredentialTokenResult.Nok(Global.CannotGetTokenWithAuthorizationCode);
        return CredentialTokenResult.Ok(tokenResult);
    }

    private Dictionary<string, string> BuildAuthorizationRequestParameters(DidDocument didDocument,  string issuerState, BaseCredentialDefinitionResult credentialDefinition, DTOs.BaseCredentialIssuer credentialIssuer, string challenge)
    {
        var types = new JsonArray();
        foreach (var type in credentialDefinition.GetTypes())
            types.Add(type);
        var authorizationDetails = new JsonArray
        {
            new JsonObject
            {
                { "type", "openid_credential" },
                { "format", credentialDefinition.Format },
                { "types", types },
                { "locations", new JsonArray
                {
                    credentialIssuer.CredentialIssuer
                } }
            }
        };
        var clientMetadata = new JsonObject
        {
            {  "response_types_supported",
                new JsonArray
                {
                    "vp_token", "id_token"
                }
            },
            {  "authorization_endpoint", "openid://"}
        };
        return new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "scope", "openid" },
            { "state", "client-state" },
            { "client_id", didDocument.Id },
            { "authorization_details", HttpUtility.UrlEncode(authorizationDetails.ToJsonString()) },
            { "redirect_uri", "openid://" },
            { "nonce", "nonce" },
            { "code_challenge", challenge },
            { "code_challenge_method", "S256" },
            { "client_metadata", HttpUtility.UrlEncode(clientMetadata.ToJsonString()) },
            { "issuer_state", issuerState }
        };
    }

    private async Task<Dictionary<string, string>> ExecutePostAuthorizationRequest(DidDocument didDocument, IAsymmetricKey privateKey, DTOs.BaseCredentialIssuer credentialIssuer, string redirectUri, string nonce, string state, CancellationToken cancellationToken)
    {
        var signingCredentials = privateKey.BuildSigningCredentials(didDocument.Authentication.First().ToString());
        var handler = new JsonWebTokenHandler();
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = signingCredentials,
            Audience = credentialIssuer.CredentialIssuer
        };
        var claims = new Dictionary<string, object>
        {
            { "nonce", nonce },
            { "iss", didDocument.Id },
            { "sub", didDocument.Id }
        };
        securityTokenDescriptor.Claims = claims;
        var token = handler.CreateToken(securityTokenDescriptor);
        return await _sidServerClient.PostAuthorizationRequest(redirectUri, token, state, cancellationToken);
    }

    #endregion

    private string BuildProofOfPossession(DidDocument didDocument, IAsymmetricKey privateKey, DTOs.BaseCredentialIssuer credentialIssuer, string cNonce)
    {
        var signingCredentials = privateKey.BuildSigningCredentials(didDocument.Authentication.First().ToString());
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
            { "iss", didDocument.Id },
            { "nonce", cNonce }
        };
        securityTokenDescriptor.Claims = claims;
        return handler.CreateToken(securityTokenDescriptor);
    }

    private static (string codeChallenge, string verifier) GeneratePkce(int size = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[size];
        rng.GetBytes(randomBytes);
        var verifier = Base64UrlEncode(randomBytes);

        var buffer = Encoding.UTF8.GetBytes(verifier);
        var hash = SHA256.Create().ComputeHash(buffer);
        var challenge = Base64UrlEncode(hash);

        return (challenge, verifier);
    }

    private static string Base64UrlEncode(byte[] data) =>
            Convert.ToBase64String(data)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');

    private record CredentialTokenResult
    {
        public string ErrorMessage { get; private set; }
        public TokenResult Token { get; private set; }

        public static CredentialTokenResult Ok(TokenResult token) => new CredentialTokenResult { Token = token };

        public static CredentialTokenResult Nok(string errorMessage) => new CredentialTokenResult { ErrorMessage = errorMessage };
    }
}