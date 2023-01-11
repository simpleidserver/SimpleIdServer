// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Stores
{
    public interface IKeyStore
    {
        IEnumerable<SigningCredentials> GetAllSigningKeys();
        IEnumerable<EncryptingCredentials> GetAllEncryptingKeys();
    }

    public class InMemoryKeyStore : IKeyStore
    {
        private ConcurrentBag<SigningCredentials> _signingCredentials = new ConcurrentBag<SigningCredentials>();
        private ConcurrentBag<EncryptingCredentials> _encryptedCredentials = new ConcurrentBag<EncryptingCredentials>();

        public InMemoryKeyStore() { }

        public IEnumerable<SigningCredentials> GetAllSigningKeys() => _signingCredentials.ToList();

        public IEnumerable<EncryptingCredentials> GetAllEncryptingKeys() => _encryptedCredentials.ToList();

        public void Add(SigningCredentials signingCredentials) => _signingCredentials.Add(signingCredentials);

        internal void SetSigningCredentials(IEnumerable<SigningCredentials> signingCredentials) => _signingCredentials = new ConcurrentBag<SigningCredentials>(signingCredentials);

        internal void SetEncryptedCredentials(IEnumerable<EncryptingCredentials> encryptedCredentials) => _encryptedCredentials = new ConcurrentBag<EncryptingCredentials>(encryptedCredentials);
    }
}