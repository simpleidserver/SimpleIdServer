// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using System.Security.Cryptography.X509Certificates;
namespace SimpleIdServer.IdServer.Saml.Idp.Extensions;

public static class ClientExtensions
{
    private const string SAML2_SIG_CERTIFICATE_NAME = "SAML2_SIG_CERTIFICATE";
    private const string SAML2_SP_METADATA_NAME = "SAML2_SP_METADATA";
    private const string SAML2_USE_ACS_ARTIFACT_NAME = "SAML2_USE_ACS_ARTIFACT";

    public static X509Certificate2? GetSaml2SigningCertificate(this Client client)
    {
        var parameters = client.Parameters;
        if (!parameters.ContainsKey(SAML2_SIG_CERTIFICATE_NAME)) return null;
        var sigCertificateId = parameters[SAML2_SIG_CERTIFICATE_NAME];
        var jsonWebKey = client.JsonWebKeys.Single(j => j.KeyId == sigCertificateId);
        var x5c = jsonWebKey.X5c;
        var x5cBase64Str = x5c.First();
        return new X509Certificate2(Convert.FromBase64String(x5cBase64Str));
    }

    public static string GetSerializedSaml2SigningCertificate(this Client client)
    {
        var parameters = client.Parameters;
        if (!parameters.ContainsKey(SAML2_SIG_CERTIFICATE_NAME)) return null;
        var sigCertificateId = parameters[SAML2_SIG_CERTIFICATE_NAME];
        var jsonWebKey = client.SerializedJsonWebKeys.Single(j => j.Kid == sigCertificateId);
        return jsonWebKey.SerializedJsonWebKey;
    }

    public static void SetSaml2SigningCertificate(this Client client, X509Certificate2 sigCertificate, string alg = SecurityAlgorithms.RsaSha256)
    {
        var parameters = client.Parameters;
        if (parameters.ContainsKey(SAML2_SIG_CERTIFICATE_NAME)) return;
        var keyId = Guid.NewGuid().ToString();
        var securityKey = new X509SecurityKey(sigCertificate, keyId);
        var signingCredentials = new SigningCredentials(securityKey, alg);
        var jsonWebKey = signingCredentials.SerializePublicJWK();
        client.Add(keyId, jsonWebKey, IdServer.Constants.JWKUsages.Sig, SecurityKeyTypes.CERTIFICATE);
        parameters.Add(SAML2_SIG_CERTIFICATE_NAME, keyId);
        client.Parameters = parameters;
    }

    public static void AddSaml2SigningCertificate(this Client client) => SetSaml2SigningCertificate(client, KeyGenerator.GenerateSelfSignedCertificate());

    public static string? GetSaml2SpMetadataUrl(this Client client)
    {
        var parameters = client.Parameters;
        if (!parameters.ContainsKey(SAML2_SP_METADATA_NAME)) return null;
        return parameters.Single(p => p.Key == SAML2_SP_METADATA_NAME).Value;
    }

    public static void SetSaml2SpMetadataUrl(this Client client, string saml2SpMetadataUrl)
    {
        var parameters = client.Parameters;
        if (parameters.ContainsKey(SAML2_SP_METADATA_NAME)) parameters[SAML2_SP_METADATA_NAME] = saml2SpMetadataUrl;
        else
        {
            parameters.Add(SAML2_SP_METADATA_NAME, saml2SpMetadataUrl);
            client.Parameters = parameters;
        }
    }

    public static void SetUseAcsArtifact(this Client client, bool useAcs)
    {
        var parameters = client.Parameters;
        if (parameters.ContainsKey(SAML2_USE_ACS_ARTIFACT_NAME)) parameters[SAML2_USE_ACS_ARTIFACT_NAME] = useAcs.ToString();
        else parameters.Add(SAML2_USE_ACS_ARTIFACT_NAME, useAcs.ToString());
        client.Parameters = parameters;
    }

    public static bool GetUseAcrsArtifact(this Client client)
    {
        var parameters = client.Parameters;
        if (!parameters.ContainsKey(SAML2_USE_ACS_ARTIFACT_NAME)) return false;
        if (bool.TryParse(parameters[SAML2_USE_ACS_ARTIFACT_NAME], out bool r)) return r;
        return false;
    }
}
