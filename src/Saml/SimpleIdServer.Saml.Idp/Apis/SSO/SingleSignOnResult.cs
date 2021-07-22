// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Xsd;

namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public class SingleSignOnResult
    {
        public bool IsValid { get; private set; }
        public ResponseType Response { get; private set; }
        public RelyingPartyAggregate RelyingParty { get; private set; }
        public string Amr { get; private set; }

        public static SingleSignOnResult Ok(ResponseType response, RelyingPartyAggregate relyingParty)
        {
            return new SingleSignOnResult
            {
                IsValid = true,
                Response = response,
                RelyingParty = relyingParty
            };
        }

        public static SingleSignOnResult Redirect(string amr)
        {
            return new SingleSignOnResult
            {
                IsValid = false,
                Amr = amr
            };
        }
    }
}
