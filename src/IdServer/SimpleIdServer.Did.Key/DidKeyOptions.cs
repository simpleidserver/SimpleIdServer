// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Did.Key
{
    public class DidKeyOptions
    {
        /// <summary>
        /// Public key format used to extract
        /// </summary>
        public string PublicKeyFormat { get; set; } = Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2020;
    }
}
