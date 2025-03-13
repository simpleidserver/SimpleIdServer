// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleIdServer.IdServer.Stores
{
    public interface IKeyStore
    {
        IEnumerable<SigningCredentials> GetAllSigningKeys(string realm);
        IEnumerable<EncryptingCredentials> GetAllEncryptingKeys(string realm);
        void Add(Domains.Realm realm, SigningCredentials signingCredentials);
        void Add(Domains.Realm realm, EncryptingCredentials encryptingCredentials);
    }

    public class InMemoryKeyStore : IKeyStore
    {
        private readonly IFileSerializedKeyStore _fileSerializedKeyStore;
        private readonly ITransactionBuilder _builder;

        public InMemoryKeyStore(IFileSerializedKeyStore fileSerializedKeyStore, ITransactionBuilder builder) 
        {
            _fileSerializedKeyStore = fileSerializedKeyStore;
            _builder = builder;
        }

        public IEnumerable<SigningCredentials> GetAllSigningKeys(string realm)
        {
            var result = new List<SigningCredentials>();
            var serializedKeys = _fileSerializedKeyStore.GetAllSig(realm, CancellationToken.None).Result;
            foreach(var serializedKey in serializedKeys)
            {
                SecurityKey securityKey;
                if(serializedKey.IsSymmetric)
                    securityKey = new SymmetricSecurityKey(serializedKey.Key);
                else
                {
                    var pem = new PemResult(serializedKey.PublicKeyPem, serializedKey.PrivateKeyPem);
                    securityKey = PemImporter.Import(pem, serializedKey.KeyId);
                }

                securityKey.KeyId = serializedKey.KeyId;
                result.Add(new SigningCredentials(securityKey, serializedKey.Alg));
            }

            return result;
        }

        public IEnumerable<EncryptingCredentials> GetAllEncryptingKeys(string realm)
        {
            var result = new List<EncryptingCredentials>();
            var serializedKeys = _fileSerializedKeyStore.GetAllEnc(realm, CancellationToken.None).Result;
            foreach(var serializedKey in serializedKeys)
            {
                SecurityKey securityKey;
                if (serializedKey.IsSymmetric)
                    securityKey = new SymmetricSecurityKey(serializedKey.Key);
                else
                {
                    var pemResult = new PemResult(serializedKey.PublicKeyPem, serializedKey.PrivateKeyPem);
                    securityKey = PemImporter.Import(pemResult, serializedKey.KeyId);
                }

                securityKey.KeyId = serializedKey.KeyId;
                result.Add(new EncryptingCredentials(securityKey, serializedKey.Alg, serializedKey.Enc));
            }

            return result;
        }

        public async void Add(Domains.Realm realm, SigningCredentials signingCredentials)
        {
            using (var transaction = _builder.Build())
            {
                var result = Convert(signingCredentials, realm);
                _fileSerializedKeyStore.Add(result);
                await transaction.Commit(CancellationToken.None);
            }
        }

        public async void Add(Domains.Realm realm, EncryptingCredentials encryptingCredentials)
        {
            using (var transaction = _builder.Build())
            {
                var result = Convert(encryptingCredentials, realm);
                _fileSerializedKeyStore.Add(result);
                await transaction.Commit(CancellationToken.None);
            }
        }

        internal async void SetSigningCredentials(Domains.Realm realm, IEnumerable<SigningCredentials> signingCredentials)
        {
            using (var transaction = _builder.Build())
            {
                foreach (var signingCredential in signingCredentials)
                {
                    var record = Convert(signingCredential, realm);
                    _fileSerializedKeyStore.Add(record);
                }

                await transaction.Commit(CancellationToken.None);
            }
        }

        internal async void SetEncryptedCredentials(Domains.Realm realm, IEnumerable<EncryptingCredentials> encryptedCredentials)
        {
            using (var transaction = _builder.Build())
            {
                foreach (var encryptedCredential in encryptedCredentials)
                {
                    var record = Convert(encryptedCredential, realm);
                    _fileSerializedKeyStore.Add(record);
                }

                await transaction.Commit(CancellationToken.None);
            }
        }

        public static SerializedFileKey Convert(SigningCredentials credentials, Domains.Realm realm)
        {
            var symmetric = credentials.Key as SymmetricSecurityKey;
            SerializedFileKey result = null;
            if (symmetric != null)
            {
                result = new SerializedFileKey
                {
                    Id = Guid.NewGuid().ToString(),
                    KeyId = credentials.Key.KeyId,
                    Usage = Constants.JWKUsages.Sig,
                    Alg = credentials.Algorithm,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    IsSymmetric = true,
                    Key = symmetric.Key
                };
            }
            else
            {
                var pem = PemConverter.ConvertFromSecurityKey(credentials.Key);
                result = new SerializedFileKey
                {
                    Id = Guid.NewGuid().ToString(),
                    KeyId = credentials.Key.KeyId,
                    PrivateKeyPem = pem.PrivateKey,
                    PublicKeyPem = pem.PublicKey,
                    Usage = Constants.JWKUsages.Sig,
                    Alg = credentials.Algorithm,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    IsSymmetric = false
                };
            }

            result.Realms.Add(realm);
            return result;
        }

        public static SerializedFileKey Convert(EncryptingCredentials credentials, Domains.Realm realm)
        {
            var symmetric = credentials.Key as SymmetricSecurityKey;
            SerializedFileKey result = null;
            if (symmetric != null)
            {
                result = new SerializedFileKey
                {
                    Id = Guid.NewGuid().ToString(),
                    KeyId = credentials.Key.KeyId,
                    Usage = Constants.JWKUsages.Sig,
                    Alg = credentials.Alg,
                    Enc = credentials.Enc,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    IsSymmetric = true,
                    Key = symmetric.Key
                };
            }
            else
            {
                var pem = PemConverter.ConvertFromSecurityKey(credentials.Key);
                result = new SerializedFileKey
                {
                    Id = Guid.NewGuid().ToString(),
                    KeyId = credentials.Key.KeyId,
                    PrivateKeyPem = pem.PrivateKey,
                    PublicKeyPem = pem.PublicKey,
                    Usage = Constants.JWKUsages.Enc,
                    Alg = credentials.Alg,
                    Enc = credentials.Enc,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    IsSymmetric = false
                };
            }

            result.Realms.Add(realm);
            return result;
        }
    }
}