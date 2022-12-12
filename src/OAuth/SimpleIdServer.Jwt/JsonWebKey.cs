// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Jwt
{
    /// <summary>
    /// Key types
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum KeyTypes
    {
        /// <summary>
        /// Ellipse Curve
        /// </summary>
        [EnumMember(Value = "EC")]
        EC = 0,
        /// <summary>
        /// RSA
        /// </summary>
        [EnumMember(Value = "RSA")]
        RSA = 1,
        /// <summary>
        /// Octet sequence (used to represent symmetric keys)
        /// </summary>
        [EnumMember(Value = "oct")]
        OCT = 2
    }

    /// <summary>
    /// Identifies the itended use of the Public Key.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Usages
    {
        /// <summary>
        /// Signature
        /// </summary>
        [EnumMember(Value = "sig")]
        SIG = 0,
        /// <summary>
        /// Encryption
        /// </summary>
        [EnumMember(Value = "enc")]
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
        [EnumMember(Value = "sign")]
        Sign = 0,
        /// <summary>
        /// Verify digital signature or MAC
        /// </summary>
        [EnumMember(Value = "verify")]
        Verify = 1,
        /// <summary>
        /// Encrypt content
        /// </summary>
        [EnumMember(Value = "encrypt")]
        Encrypt = 2,
        /// <summary>
        /// Decrypt content and validate decryption if applicable
        /// </summary>
        [EnumMember(Value = "decrypt")]
        Decrypt = 3,
        /// <summary>
        /// Encrypt key
        /// </summary>
        [EnumMember(Value = "wrapKey")]
        WrapKey = 4,
        /// <summary>
        /// Decrypt key and validate encryption if applicable
        /// </summary>
        [EnumMember(Value = "unwrapKey")]
        UnWrapKey = 5,
        /// <summary>
        /// Derive key
        /// </summary>
        [EnumMember(Value = "deriveKey")]
        DeriveKey = 6,
        /// <summary>
        /// Derive bits not to be used as a key
        /// </summary>
        [EnumMember(Value = "deriveBits")]
        DeriveBits = 7
    }

    /// <summary>
    /// Definition of a JSON Web Key (JWK)
    /// It's a JSON data structure that represents a cryptographic key
    /// </summary>
    [JsonConverter(typeof(JsonWebKeyConverter))]
    public class JsonWebKey : ICloneable, IEquatable<JsonWebKey>
    {
        private static IEnumerable<KeyOperations> PUBLIC_KEY_OPERATIONS = new[]
        {
            KeyOperations.Verify,
            KeyOperations.Encrypt,
            KeyOperations.UnWrapKey,
            KeyOperations.DeriveBits
        };
        private static Dictionary<KeyTypes, IEnumerable<string>> MAPPING_KEYTYPE_TO_PUBLIC_KEYS = new Dictionary<KeyTypes, IEnumerable<string>>
        {
            { KeyTypes.EC, ECFields.PUBLIC_FIELDS },
            { KeyTypes.RSA, RSAFields.PUBLIC_FIELDS }
        };

        public JsonWebKey() { }

        public JsonWebKey(string kid) : this()
        {
            Kid = kid;
        }

        public JsonWebKey(string kid, Usages use, ICollection<KeyOperations> keyOperations) : this(kid)
        {
            Use = use;
            KeyOps = keyOperations;
        }

        /// <summary>
        /// Gets or sets the cryptographic algorithm family used with the key.
        /// </summary>
        [JsonPropertyName("kty")]
        public KeyTypes Kty { get; set; }
        /// <summary>
        /// Gets or sets the intended use of the public key.
        /// Employed to indicate whether a public key is used for encrypting data or verifying the signature on data.
        /// </summary>
        [JsonPropertyName("use")]
        public Usages Use { get; set; }
        /// <summary>
        /// Gets or sets the operation(s) that the key is intended to be user for.
        /// </summary>
        [JsonPropertyName("key_ops")]
        public ICollection<KeyOperations> KeyOps { get; set; } = new List<KeyOperations>();
        /// <summary>
        /// Gets or sets the algorithm intended for use with the key
        /// </summary>
        [JsonPropertyName("alg")]
        public string Alg { get; set; }
        /// <summary>
        /// Gets or sets the KID (key id). 
        /// </summary>
        [JsonPropertyName("kid")]
        public string Kid { get; set; }
        /// <summary>
        /// Gets or sets the content of the key.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string>? Content { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// KID of the rotation key.
        /// </summary>
        [JsonIgnore]
        public string RotationJWKId { get; set; }
        /// <summary>
        /// Expiration datetime.
        /// </summary>
        [JsonIgnore]
        public DateTime? ExpirationDateTime { get; set; }

        public object Clone()
        {
            return new JsonWebKey
            {
                Alg = Alg,
                Use = Use,
                Kid = Kid,
                Kty = Kty,
                Content = Content == null ? new Dictionary<string, string>() : Content.ToDictionary(s => s.Key, s => s.Value),
                RotationJWKId = RotationJWKId,
                ExpirationDateTime = ExpirationDateTime,
                KeyOps = KeyOps.Select(k => k).ToList()
            };
        }

        public JsonObject Serialize()
        {
            var result = JsonSerializer.SerializeToNode(this).AsObject();
            return result;
        }

        public JsonObject GetPublicJwt()
        {
            var clone = (JsonWebKey)Clone();
            clone.KeyOps = clone.KeyOps.Where(k => PUBLIC_KEY_OPERATIONS.Contains(k)).ToList();
            clone.Content = clone.Content.Where(k => MAPPING_KEYTYPE_TO_PUBLIC_KEYS[Kty].Contains(k.Key)).ToDictionary(kvp=> kvp.Key, kvp => kvp.Value);
            return clone.Serialize();
        }

        public static JsonWebKey Deserialize(string json)
        {
            return JsonSerializer.Deserialize<JsonWebKey>(json);
        }

        private static bool Extract<T>(JsonObject jObj, string name, Dictionary<T, string> dic, out T result)
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

        public JsonWebKey Rotate(int expirationTimeInSeconds)
        {
            var result = new JsonWebKey
            {
                Kid = Guid.NewGuid().ToString(),
                Alg = Alg,
                KeyOps = KeyOps.Select(k => k).ToList(),
                Kty = Kty,
                Use = Use,
                Content = new Dictionary<string, string>()
            };
            switch(result.Kty)
            {
                case KeyTypes.RSA:
                    using (var rsa = RSA.Create())
                    {
                        foreach(var kvp in rsa.ExtractPublicKey())
                        {
                            result.Content.Add(kvp.Key, kvp.Value);
                        }

                        foreach (var kvp in rsa.ExtractPrivateKey())
                        {
                            result.Content.Add(kvp.Key, kvp.Value);
                        }
                    }
                    break;
                case KeyTypes.EC:
                    using (var ec = new ECDsaCng())
                    {
                        foreach (var kvp in ec.ExtractPublicKey())
                        {
                            result.Content.Add(kvp.Key, kvp.Value);
                        }

                        foreach (var kvp in ec.ExtractPrivateKey())
                        {
                            result.Content.Add(kvp.Key, kvp.Value);
                        }
                    }
                    break;
                case KeyTypes.OCT:
                    using (var ec = new HMACSHA256())
                    {
                        result.Content = ec.ExportKey();
                    }
                    break;
            }

            RotationJWKId = result.Kid;
            ExpirationDateTime = DateTime.UtcNow.AddSeconds(expirationTimeInSeconds);
            return result;
        }

        public override bool Equals(object obj)
        {
            var target = obj as JsonWebKey;
            if (target == null)
            {
                return false;
            }

            return Equals(target);
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
