// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Saml.Idp.Extensions;
public static class ClientExtensions
{
    private const string SAML2_SIG_CERTIFICATE_NAME = "SAML2_SIG_CERTIFICATE_NAME";

    public static X509Certificate2 GetSaml2SigningCertificate(this Client client)
    {
        if (!client.Parameters.ContainsKey(SAML2_SIG_CERTIFICATE_NAME)) return null;
        var sigCertificateId = client.Parameters[SAML2_SIG_CERTIFICATE_NAME];
        var jsonWebKey = client.JsonWebKeys.Single(j => j.KeyId == sigCertificateId);
        var x5c = jsonWebKey.X5c;
        var x5cBase64Str = x5c.First();
        return new X509Certificate2(Convert.FromBase64String(x5cBase64Str));
    }

    public static string GetSaml2SpMetadataUrl(this Client client)
    {
        return null;
    }
}
