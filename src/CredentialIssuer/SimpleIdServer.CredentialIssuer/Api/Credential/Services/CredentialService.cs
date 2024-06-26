// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.IdServer.CredentialIssuer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.CredentialIssuer.Api.Credential.Services;

public interface ICredentialService
{
    CredentialResult BuildImmediateCredential(BuildImmediateCredentialRequest request);
}

public class BuildImmediateCredentialRequest
{
    public BuildImmediateCredentialRequest(
        string subject, 
        CredentialConfiguration credentialConfiguration, 
        Domains.Credential credential,
        Dictionary<string, string> claims, 
        ICredentialFormatter formatter,
        CredentialResponseEncryption encryption,
        string nonce)
    {
        Subject = subject;
        CredentialConfiguration = credentialConfiguration;
        Credential = credential;
        Claims = claims;
        Formatter = formatter;
        Encryption = encryption;
        Nonce = nonce;
    }

    public string Subject { get; set; }
    public CredentialConfiguration CredentialConfiguration { get; set; }
    public Domains.Credential Credential { get; set; }
    public Dictionary<string, string> Claims { get; set; }
    public ICredentialFormatter Formatter { get; set; }
    public CredentialResponseEncryption Encryption { get; set; }
    public string Nonce { get; set; }
}

public class CredentialService : ICredentialService
{
    private readonly CredentialIssuerOptions _options;

    public CredentialService(IOptions<CredentialIssuerOptions> options)
    {
        _options = options.Value;
    }

    public CredentialResult BuildImmediateCredential(BuildImmediateCredentialRequest request)
    {
        var buildRequest = new BuildCredentialRequest
        {
            Subject = request.Subject,
            Issuer = _options.DidDocument.Id,
            JsonLdContext = request.CredentialConfiguration.JsonLdContext,
            Type = request.CredentialConfiguration.Type,
            CredentialConfiguration = request.CredentialConfiguration,
            AdditionalTypes = request.CredentialConfiguration.AdditionalTypes,
        };
        if (request.Credential != null)
            Enrich(buildRequest, request.Credential);
        else
            Enrich(buildRequest, request.CredentialConfiguration);
        if (!string.IsNullOrWhiteSpace(request.CredentialConfiguration.CredentialSchemaId))
        {
            buildRequest.Schema = new CredentialSchema
            {
                Id = request.CredentialConfiguration.CredentialSchemaId,
                Type = request.CredentialConfiguration.CredentialSchemaType
            };
        }

        buildRequest.UserClaims = request.Claims.Select(kvp =>
        {
            var cl = request.CredentialConfiguration.Claims.Single(cl => cl.SourceUserClaimName == kvp.Key);
            return new CredentialUserClaimNode
            {
                Level = cl.Name.Split('.').Count(),
                Name = cl.Name,
                Value = kvp.Value
            };
        }).ToList();
        var formatter = request.Formatter;
        var credentialResult = formatter.Build(buildRequest,
            _options.DidDocument,
            _options.VerificationMethodId,
            _options.AsymmKey);
        if (request.Encryption != null)
        {
            var handler = new JsonWebTokenHandler();
            var encKey = request.Encryption.Jwk;
            var encryptedCredential = handler.EncryptToken(credentialResult.ToJsonString(), new Microsoft.IdentityModel.Tokens.EncryptingCredentials(
            encKey,
                request.Encryption.Alg,
                request.Encryption.Enc));
            credentialResult = encryptedCredential;
        }

        return new CredentialResult
        {
            Format = request.Formatter.Format,
            Credential = credentialResult,
            CNonce = request.Nonce
        };
    }

    private void Enrich(
        BuildCredentialRequest buildRequest,
        Domains.Credential credential)
    {
        buildRequest.Id = $"{credential.Configuration.BaseUrl}/{credential.CredentialId}";
        buildRequest.ValidFrom = credential.IssueDateTime;
        buildRequest.ValidUntil = credential.ExpirationDateTime;
    }

    private void Enrich(
        BuildCredentialRequest buildRequest,
        Domains.CredentialConfiguration credentialConfiguration)
    {
        buildRequest.Id = $"{credentialConfiguration.BaseUrl}/{Guid.NewGuid()}";
        buildRequest.ValidFrom = DateTime.UtcNow.Date;
        if (_options.CredentialExpirationTimeInSeconds != null)
        {
            buildRequest.ValidUntil = DateTime.UtcNow.Date.AddSeconds(_options.CredentialExpirationTimeInSeconds.Value);
        }

        if (!string.IsNullOrWhiteSpace(credentialConfiguration.CredentialSchemaId))
        {
            buildRequest.Schema = new CredentialSchema
            {
                Id = credentialConfiguration.CredentialSchemaId,
                Type = credentialConfiguration.CredentialSchemaType
            };
        }
    }
}