// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Did.Jwt.Crypto
{
    public interface ISignatureKey
    {
        bool Check(string content, string signature);
        string Sign(string content);
    }
}