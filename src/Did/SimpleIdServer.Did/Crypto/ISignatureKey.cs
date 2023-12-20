// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto
{
    /// <summary>
    /// Explanation about JOSE : https://www.iana.org/assignments/jose/jose.xhtml
    /// </summary>
    public interface ISignatureKey : IAsymmetricKey
    {
        bool Check(string content, string signature);
        bool Check(byte[] content, byte[] signature);
        string Sign(string content);
        string Sign(byte[] content);
    }
}