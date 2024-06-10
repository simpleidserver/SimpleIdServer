// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Key;
using SimpleIdServer.Did.Models;
using System.Linq;
using System.Threading;

namespace SimpleIdServer.IdServer.CredentialIssuer;

public class CredentialIssuerOptions
{
    public CredentialIssuerOptions()
    {
        var resolver = DidKeyResolver.New();
        AsymmKey = Ed25519SignatureKey.Generate();
        var did = DidKeyGenerator.New().Generate(AsymmKey);
        DidDocument = resolver.Resolve(did, CancellationToken.None).Result;
        VerificationMethodId = DidDocument.VerificationMethod.First().Id;
    }

    /// <summary>
    /// Did Document of the issuer. Contains only the public key.
    /// </summary>
    public DidDocument DidDocument { get; set; }

    /// <summary>
    /// Identifier of the verification method. It will be used to signed the verifiable credential.
    /// </summary>
    public string VerificationMethodId { get; set; }

    /// <summary>
    /// Private key used to sign the Verifiable Credential.
    /// </summary>
    public IAsymmetricKey AsymmKey { get; set; }

    /// <summary>
    /// Base URL of the authorization server.
    /// </summary>
    public string AuthorizationServer { get; set; } = "https://localhost:5001/master";

    /// <summary>
    /// If the value is true, then the credential offer is returned in the credential_offer parameter.
    /// If the value is false, then the credential offer is returned by reference, in the credential_offer_uri parameter.
    /// </summary>
    public bool IsCredentialOfferReturnedByReference { get; set; } = false;

    /// <summary>
    /// Default expiration time of a credential.
    /// </summary>
    public int? CredentialExpirationTimeInSeconds { get; set; }

    /// <summary>
    /// Client identifier.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Client secret.
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Ignore the HTTPS certificate error.
    /// </summary>
    public bool IgnoreHttpsCertificateError { get; set; }

    /// <summary>
    /// Set the version of the credential issuer.
    /// </summary>
    public CredentialIssuerVersion Version { get; set; } = CredentialIssuerVersion.LAST;
}

public enum CredentialIssuerVersion
{
    LAST = 0,
    ESBI = 1
}