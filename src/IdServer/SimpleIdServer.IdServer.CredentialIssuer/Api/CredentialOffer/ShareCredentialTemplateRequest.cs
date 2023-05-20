// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialOffer
{
    public class ShareCredentialTemplateRequest
    {
        public string CredentialTemplateId { get; set; }
        public string WalletClientId { get; set; }
    }
}
