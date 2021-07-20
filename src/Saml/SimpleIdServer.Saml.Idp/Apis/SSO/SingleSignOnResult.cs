// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Xsd;

namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public class SingleSignOnResult
    {
        public bool IsValid { get; private set; }
        public ResponseType Response { get; private set; }
        public string Amr { get; private set; }

        public static SingleSignOnResult Ok(ResponseType response)
        {
            return new SingleSignOnResult
            {
                IsValid = true,
                Response = response
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
