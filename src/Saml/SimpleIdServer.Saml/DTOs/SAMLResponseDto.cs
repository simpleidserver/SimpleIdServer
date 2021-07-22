// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Saml.DTOs
{
    public class SAMLResponseDto
    {
        public string SAMLResponse { get; set; }
        public string RelayState { get; set; }
        public string SigAlg { get; set; }
        public string Signature { get; set; }
    }
}
