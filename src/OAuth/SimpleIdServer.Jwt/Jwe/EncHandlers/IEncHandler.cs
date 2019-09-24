// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Jwt.Jwe.EncHandlers
{
    public interface IEncHandler
    {
        string EncName { get; }
        int KeyLength { get; }
        byte[] Encrypt(string payload, byte[] key, byte[] iv);
        string Decrypt(byte[] payload, byte[] key, byte[] iv);
        byte[] BuildHash(byte[] key, byte[] payload);
    }
}
