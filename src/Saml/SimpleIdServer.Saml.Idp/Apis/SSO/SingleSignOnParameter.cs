// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public class SingleSignOnParameter
    {
        public string SAMLRequest { get; set; }
        public string RelayState { get; set; }
    }
}
