// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.IO;
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Jwe.EncHandlers
{
    public abstract class AESCBCHSEncHandler : IEncHandler
    {
        public byte[] Encrypt(string payload, byte[] key, byte[] iv)
        {
            byte[] result;
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(payload);
                            }

                            result = msEncrypt.ToArray();
                        }
                    }
                }
            }

            return result;
        }

        public string Decrypt(byte[] payload, byte[] key, byte[] iv)
        {
            string result;
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var msDecrypt = new MemoryStream(payload))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return result;
        }

        public byte[] BuildHash(byte[] key, byte[] payload)
        {
            using (var hmac = GetHmac(key))
            {
                return hmac.ComputeHash(payload);
            }
        }

        public abstract int KeyLength { get; }
        public abstract string EncName { get; }
        public abstract HMAC GetHmac(byte[] key);
    }
}