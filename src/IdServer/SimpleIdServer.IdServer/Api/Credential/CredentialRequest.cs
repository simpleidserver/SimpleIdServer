// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Api.Credential
{
    public class CredentialRequest
    {
        public string Format { get; set; }
    }

    public class CredentialRequestProof
    {
        public string ProofType { get; set; }
    }
}
