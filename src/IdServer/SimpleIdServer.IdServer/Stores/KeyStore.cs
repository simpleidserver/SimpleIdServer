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
        IEnumerable<SigningCredentials> GetAllSigningKeys(string realm);
        IEnumerable<EncryptingCredentials> GetAllEncryptingKeys(string realm);
        void Add(string realm, SigningCredentials signingCredentials);
        void Add(string realm, EncryptingCredentials encryptingCredentials);
    }

    public class InMemoryKeyStore : IKeyStore
    {
        private ConcurrentDictionary<string, ICollection<SigningCredentials>> _signingCredentials = new ConcurrentDictionary<string, ICollection<SigningCredentials>>();
        private ConcurrentDictionary<string, ICollection<EncryptingCredentials>> _encryptedCredentials = new ConcurrentDictionary<string, ICollection<EncryptingCredentials>>();

        public InMemoryKeyStore() { }

        public IEnumerable<SigningCredentials> GetAllSigningKeys(string realm)
        {
            if(!_signingCredentials.ContainsKey(realm))
                return new List<SigningCredentials>();
            return _signingCredentials.First(c => c.Key == realm).Value.ToList();
        }

        public IEnumerable<EncryptingCredentials> GetAllEncryptingKeys(string realm)
        {
            if (!_encryptedCredentials.ContainsKey(realm))
                return new List<EncryptingCredentials>();
            return _encryptedCredentials.First(c => c.Key == realm).Value.ToList();
        }

        public void Add(string realm, SigningCredentials signingCredentials)
        {
            if(!_signingCredentials.ContainsKey(realm))
                _signingCredentials.AddOrUpdate(realm, new List<SigningCredentials>(), (a,b) => new List<SigningCredentials>());

            _signingCredentials[realm].Add(signingCredentials);
        }

        public void Add(string realm, EncryptingCredentials encryptingCredentials)
        {
            if (!_signingCredentials.ContainsKey(realm))
                _encryptedCredentials.AddOrUpdate(realm, new List<EncryptingCredentials>(), (a, b) => new List<EncryptingCredentials>());

            _encryptedCredentials[realm].Add(encryptingCredentials);
        }

        internal void SetSigningCredentials(string realm, IEnumerable<SigningCredentials> signingCredentials) => _signingCredentials.AddOrUpdate(realm, signingCredentials.ToList(), (a, b) => signingCredentials.ToList());

        internal void SetEncryptedCredentials(string realm, IEnumerable<EncryptingCredentials> encryptedCredentials) => _encryptedCredentials.AddOrUpdate(realm, encryptedCredentials.ToList(), (a, b) => encryptedCredentials.ToList());
    }
}