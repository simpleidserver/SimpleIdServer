// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;

namespace SimpleIdServer.Saml.Helpers
{
    public static class SignatureHelper
    {
        public static bool CheckSignature(AuthnRequestType authnRequest)
        {
            var signedRequest = new SamlSignedRequest(authnRequest.SerializeToXmlElement(), null);
            return signedRequest.CheckSignature();
        }
    }
}
