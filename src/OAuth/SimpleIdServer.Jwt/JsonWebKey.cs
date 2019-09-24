// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt
{
    /// <summary>
    /// Key types
    /// </summary>
    public enum KeyTypes
    {
        /// <summary>
        /// Ellipse Curve
        /// </summary>
        EC = 0,
        /// <summary>
        /// RSA
        /// </summary>
        RSA = 1,
        /// <summary>
        /// Octet sequence (used to represent symmetric keys)
        /// </summary>
        OCT = 2
    }

    /// <summary>
    /// Identifies the itended use of the Public Key.
    /// </summary>
    public enum Usages
    {
        /// <summary>
        /// Signature
        /// </summary>
        SIG = 0,
        /// <summary>
        /// Encryption
        /// </summary>
        ENC = 1
    }

    /// <summary>
    /// Identifies the operation(s) that the key is itended to be user for
    /// </summary>
    public enum KeyOperations
    {
        /// <summary>
        /// Compute digital signature or MAC
        /// </summary>
        Sign = 0,
        /// <summary>
        /// Verify digital signature or MAC
        /// </summary>
        Verify = 1,
        /// <summary>
        /// Encrypt content
        /// </summary>
        Encrypt = 2,
        /// <summary>
        /// Decrypt content and validate decryption if applicable
        /// </summary>
        Decrypt = 3,
        /// <summary>
        /// Encrypt key
        /// </summary>
        WrapKey = 4,
        /// <summary>
        /// Decrypt key and validate encryption if applicable
        /// </summary>
        UnWrapKey = 5,
        /// <summary>
        /// Derive key
        /// </summary>
        DeriveKey = 6,
        /// <summary>
        /// Derive bits not to be used as a key
        /// </summary>
        DeriveBits = 7
    }

    /// <summary>
    /// Definition of a JSON Web Key (JWK)
    /// It's a JSON data structure that represents a cryptographic key
    /// </summary>
    public class JsonWebKey : ICloneable, IEquatable<JsonWebKey>
    {
        private static IEnumerable<KeyOperations> PUBLIC_KEY_OPERATIONS = new[]
        {
            KeyOperations.Verify,
            KeyOperations.Decrypt,
            KeyOperations.UnWrapKey,
            KeyOperations.DeriveBits
        };
        private static Dictionary<KeyTypes, string> MAPPING_KEYTYPEENUM_TO_STR = new Dictionary<KeyTypes, string>
        {
            {  KeyTypes.EC, "EC" },
            {  KeyTypes.RSA, "RSA" },
            {  KeyTypes.OCT, "oct" }
        };
        private static Dictionary<Usages, string> MAPPING_USAGESENUM_TO_STR = new Dictionary<Usages, string>
        {
            { Usages.ENC, "enc" },
            { Usages.SIG, "sig" }
        };
        private static Dictionary<KeyOperations, string> MAPPING_KEYOPERATIONENUM_TO_STR = new Dictionary<KeyOperations, string>
        {
            { KeyOperations.Sign, "sign" },
            { KeyOperations.Verify, "verify" },
            { KeyOperations.Encrypt, "encrypt" },
            { KeyOperations.Decrypt, "decrypt" },
            { KeyOperations.WrapKey, "wrapKey" },
            { KeyOperations.UnWrapKey, "unwrapKey" },
            { KeyOperations.DeriveKey, "deriveKey" },
            { KeyOperations.DeriveBits, "deriveBits" }
        };
        private static Dictionary<KeyTypes, IEnumerable<string>> MAPPING_KEYTYPE_TO_PUBLIC_KEYS = new Dictionary<KeyTypes, IEnumerable<string>>
        {
            { KeyTypes.EC, ECFields.PUBLIC_FIELDS },
            { KeyTypes.RSA, RSAFields.PUBLIC_FIELDS }
        };

        public JsonWebKey()
        {
            Content = new Dictionary<string, string>();
        }

        public JsonWebKey(string kid) : this()
        {
            Kid = kid;
        }

        public JsonWebKey(string kid, Usages use, IEnumerable<KeyOperations> keyOperations) : this(kid)
        {
            Use = use;
            KeyOps = keyOperations;
        }

        /// <summary>
        /// Gets or sets the cryptographic algorithm family used with the key.
        /// </summary>
        public KeyTypes Kty { get; set; }
        /// <summary>
        /// Gets or sets the intended use of the public key.
        /// Employed to indicate whether a public key is used for encrypting data or verifying the signature on data.
        /// </summary>
        public Usages Use { get; set; }
        /// <summary>
        /// Gets or sets the operation(s) that the key is intended to be user for.
        /// </summary>
        public IEnumerable<KeyOperations> KeyOps { get; set; }
        /// <summary>
        /// Gets or sets the algorithm intended for use with the key
        /// </summary>
        public string Alg { get; set; }
        /// <summary>
        /// Gets or sets the KID (key id). 
        /// </summary>
        public string Kid { get; set; }
        /// <summary>
        /// Gets or sets the serialized key in XML
        /// </summary>
        public string SerializedKey { get; set; }
        /// <summary>
        /// Gets or sets the content of the key.
        /// </summary>
        public Dictionary<string, string> Content { get; set; }

        public object Clone()
        {
            return new JsonWebKey
            {
                Alg = Alg,
                KeyOps = KeyOps.ToArray(),
                Use = Use,
                Kid = Kid,
                Kty = Kty,
                SerializedKey = SerializedKey,
                Content = Content == null ? new Dictionary<string, string>() : Content.ToDictionary(s => s.Key, s => s.Value)
            };
        }

        public JObject GetPublicJwt()
        {
            var result = new JObject
            {
                { "kty",  MAPPING_KEYTYPEENUM_TO_STR[Kty] },
                { "use",  MAPPING_USAGESENUM_TO_STR[Use] },
                { "alg",  Alg },
                { "kid",  Kid }
            };
            if (KeyOps.Any())
            {
                result.Add("key_ops", JArray.FromObject(KeyOps.Where(k => PUBLIC_KEY_OPERATIONS.Contains(k)).Select(s => MAPPING_KEYOPERATIONENUM_TO_STR[s])));
            }

            var publicFields = MAPPING_KEYTYPE_TO_PUBLIC_KEYS[Kty];
            foreach (var kvp in Content)
            {
                if (!publicFields.Contains(kvp.Key))
                {
                    continue;
                }

                result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }

        public static JsonWebKey Deserialize(string json)
        {
            var jObj = JsonConvert.DeserializeObject<JObject>(json);
            var result = new JsonWebKey();
            KeyTypes kty;
            Usages use;
            if (Extract(jObj, "kty", MAPPING_KEYTYPEENUM_TO_STR, out kty))
            {
                result.Kty = kty;
            }

            if (Extract(jObj, "use", MAPPING_USAGESENUM_TO_STR, out use))
            {
                result.Use = use;
            }

            if (jObj.ContainsKey("alg"))
            {
                result.Alg = jObj["alg"].ToString();
            }

            if (jObj.ContainsKey("kid"))
            {
                result.Kid = jObj["kid"].ToString();
            }

            if (jObj.ContainsKey("key_ops"))
            {
                var keyOps = jObj["key_ops"] as JArray;
                if (keyOps != null)
                {
                    var kos = new List<KeyOperations>();
                    foreach (var keyOp in keyOps)
                    {
                        KeyOperations ko;
                        if (Extract(keyOp.ToString(), MAPPING_KEYOPERATIONENUM_TO_STR, out ko))
                        {
                            kos.Add(ko);
                        }
                    }

                    result.KeyOps = kos;
                }
            }

            foreach(var child in jObj)
            {
                if (!(new[] { "kty", "use", "alg", "kid", "key_ops" }).Contains(child.Key))
                {
                    result.Content.Add(child.Key, child.Value.ToString());
                }
            }

            return result;
        }

        private static bool Extract<T>(JObject jObj, string name, Dictionary<T, string> dic, out T result)
        {
            if (jObj.ContainsKey(name))
            {
                var val = jObj[name].ToString();
                return Extract(val, dic, out result);
            }

            result = default(T);
            return false;
        }

        private static bool Extract<T>(string value, Dictionary<T, string> dic, out T result)
        {
            if (dic.ContainsValue(value))
            {
                result = dic.First(kvp => kvp.Value == value).Key;
                return true;
            }

            result = default(T);
            return false;
        }

        public void Renew()
        {
            switch(Kty)
            {
                case KeyTypes.RSA:
                    using (var rsa = RSA.Create())
                    {
                        foreach(var kvp in rsa.ExtractPublicKey())
                        {
                            Content.Add(kvp.Key, kvp.Value);
                        }

                        foreach (var kvp in rsa.ExtractPrivateKey())
                        {
                            Content.Add(kvp.Key, kvp.Value);
                        }
                    }
                    break;
                case KeyTypes.EC:
                    using (var ec = new ECDsaCng())
                    {
                        foreach (var kvp in ec.ExtractPublicKey())
                        {
                            Content.Add(kvp.Key, kvp.Value);
                        }

                        foreach (var kvp in ec.ExtractPrivateKey())
                        {
                            Content.Add(kvp.Key, kvp.Value);
                        }
                    }
                    break;
                case KeyTypes.OCT:
                    using (var ec = new HMACSHA256())
                    {
                        Content = ec.ExportKey();
                    }
                    break;
            }
        }

        public bool Equals(JsonWebKey other)
        {
            if (other == null)
            {
                return false;
            }

            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Kid.GetHashCode();
        }
    }
}
