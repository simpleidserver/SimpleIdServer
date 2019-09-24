// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Jwt.Jws.Handlers
{
    public interface ISignHandler
    {
        string Sign(string payload, JsonWebKey jsonWebKey);
        bool Verify(string payload, byte[] signature, JsonWebKey jsonWebKey);
        string AlgName { get; }
    }
}