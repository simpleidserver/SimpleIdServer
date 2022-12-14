// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Stores
{
    public interface IKeyStore
    {
        IEnumerable<SigningCredentials> GetAllSigningKeys();
        IEnumerable<EncryptingCredentials> GetAllEncryptingKeys();
    }

    public class InMemoryKeyStore : IKeyStore
    {
        private readonly ConcurrentBag<SigningCredentials> _signingCredentials = new ConcurrentBag<SigningCredentials>();
        private readonly ConcurrentBag<EncryptingCredentials> _encryptedCredentials = new ConcurrentBag<EncryptingCredentials>();

        public InMemoryKeyStore() { }

        public InMemoryKeyStore(SigningCredentials signingCredentials)
        {
            _signingCredentials.Add(signingCredentials);
        }

        public IEnumerable<SigningCredentials> GetAllSigningKeys() => _signingCredentials.ToList();

        public IEnumerable<EncryptingCredentials> GetAllEncryptingKeys() => _encryptedCredentials.ToList();
    }
}