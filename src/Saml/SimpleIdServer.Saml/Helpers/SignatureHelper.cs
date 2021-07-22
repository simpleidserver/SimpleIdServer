// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Helpers
{
    public static class SignatureHelper
    {
        public static bool CheckSignature(AuthnRequestType authnRequest, X509Certificate2 certificate)
        {
            var signedRequest = new SamlSignedRequest(authnRequest.SerializeToXmlElement(), null);
            return signedRequest.CheckSignature(certificate, true);
        }
    }
}
