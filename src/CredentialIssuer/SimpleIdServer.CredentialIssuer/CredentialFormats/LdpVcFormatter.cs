// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc;
using SimpleIdServer.Vc.CredentialFormats.Serializers;
using System;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats
{
    public class LdpVcFormatter : BaseW3CVerifiableCredentialFormatter
    {
        public override string Format => "ldp_vc";

        public string Build(BuildCredentialRequest request, DidDocument didDocument, string verificationMethodId)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
            var credential = BuildCredential(request, true);
            var json = new W3CVerifiableCredentialJsonSerializer().Serialize(credential);
            return SecuredVerifiableCredential.New()
                .Secure(json, didDocument, verificationMethodId);
        }
    }
}
