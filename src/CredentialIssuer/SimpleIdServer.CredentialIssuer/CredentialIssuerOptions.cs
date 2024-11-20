﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Key;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.IdServer.CredentialIssuer;

public class CredentialIssuerOptions
{
    /// <summary>
    /// Did Document of the issuer. Contains only the public key.
    /// </summary>
    public DidDocument DidDocument { get; set; }

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

    /// <summary>
    /// Enable or disable the developer mode.
    /// </summary>
    public bool IsDeveloperModeEnabled { get; set; } = false;

    public void GenerateRandomDidKey()
    {
        AsymmKey = ES256SignatureKey.Generate();
        var exportResult = DidKeyGenerator.New().SetSignatureKey(AsymmKey).Export(false, true);
        DidDocument = exportResult.Document;
    }
}

public enum CredentialIssuerVersion
{
    LAST = 0,
    ESBI = 1
}