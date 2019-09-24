// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Exceptions;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Jwt.Helpers;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleIdServer.Jwt.Jwe
{
    public interface IJweGenerator
    {
        JweHeader ExtractHeader(string payload);
        bool IsValid(string payload, out IEnumerable<string> parts);
        string Build(string payload, string alg, string enc, JsonWebKey jsonWebKey);
        string Build(string payload, string alg, string enc, JsonWebKey jsonWebKey, string password);
        string Decrypt(string payload, JsonWebKey jsonWebKey);
        string Decrypt(string payload, JsonWebKey jsonWebKey, string password);
    }

    public class JweGenerator : IJweGenerator
    {
        private readonly IEnumerable<IEncHandler> _encHandlers;
        private readonly IEnumerable<ICEKHandler> _cekHandlers;

        public JweGenerator(IEnumerable<IEncHandler> encHandlers, IEnumerable<ICEKHandler> cekHandlers)
        {
            _encHandlers = encHandlers;
            _cekHandlers = cekHandlers;
        }

        public JweHeader ExtractHeader(string payload)
        {
            IEnumerable<string> parts = null;
            if (IsValid(payload, out parts))
            {
                return JsonConvert.DeserializeObject<JweHeader>(parts.First().Base64Decode());
            }

            return null;
        }

        public bool IsValid(string payload, out IEnumerable<string> parts)
        {
            var p = payload.GetParts();
            if(p.Count() == 5)
            {
                parts = p;
                return true;
            }

            parts = null;
            return false;
        }

        public string Build(string payload, string alg, string enc, JsonWebKey jsonWebKey)
        {
            return InternalBuild(payload, alg, enc, jsonWebKey);
        }

        public string Build(string payload, string alg, string enc, JsonWebKey jsonWebKey, string password)
        {
            return InternalBuild(payload, alg, enc, jsonWebKey, Encoding.UTF8.GetBytes(password));
        }

        public string Decrypt(string payload, JsonWebKey jsonWebKey)
        {
            return InternalDecrypt(payload, jsonWebKey);
        }

        public string Decrypt(string payload, JsonWebKey jsonWebKey, string password)
        {
            return InternalDecrypt(payload, jsonWebKey, Encoding.UTF8.GetBytes(password));
        }

        private string InternalBuild(string payload, string alg, string enc, JsonWebKey jsonWebKey, byte[] hmacKey = null)
        {
            var contentEncryptionKeyHandler = _cekHandlers.FirstOrDefault(e => e.AlgName == alg);
            var encEncryptionHandler = _encHandlers.FirstOrDefault(e => e.EncName == enc);
            if (contentEncryptionKeyHandler == null)
            {
                throw new JwtException(ErrorCodes.INTERNAL_ERROR, string.Format(ErrorMessages.UNKNOWN_ALG, alg));
            }

            if (encEncryptionHandler == null)
            {
                throw new JwtException(ErrorCodes.INTERNAL_ERROR, string.Format(ErrorMessages.UNKNOWN_ENC, enc));
            }

            var cek = BitHelper.GenerateRandomBytes(encEncryptionHandler.KeyLength);
            var encCEK = contentEncryptionKeyHandler.Encrypt(cek, jsonWebKey);
            var splittedCEK = BitHelper.SplitInHalf(cek);
            var iv = BitHelper.GenerateRandomBytes(128/*encEncryptionHandler.KeyLength / 2*/);
            if (hmacKey == null)
            {
                hmacKey = splittedCEK.First();
            }

            var key = splittedCEK.Last();
            var cipherText = encEncryptionHandler.Encrypt(payload, key, iv);
            var protectedHeader = JsonConvert.SerializeObject(new JweHeader(alg, enc, jsonWebKey.Kid), Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            var protectedHeaderJson = protectedHeader.ToString();
            var aad = Encoding.ASCII.GetBytes(Encoding.UTF8.GetBytes(protectedHeaderJson).Base64EncodeBytes());
            var al = BitHelper.LongToBytes(aad.Length * 8);
            var hmacInput = BitHelper.Concat(aad, iv, cipherText, al);
            var hmacValue = encEncryptionHandler.BuildHash(hmacKey, hmacInput);
            var authTag = BitHelper.SplitInHalf(hmacValue).First();
            var base64EncodedjweProtectedHeaderSerialized = protectedHeader.ToString().Base64Encode();
            var base64EncodedJweEncryptedKey = encCEK.Base64EncodeBytes();
            var base64EncodedIv = iv.Base64EncodeBytes();
            var base64EncodedCipherText = cipherText.Base64EncodeBytes();
            var base64EncodedAuthenticationTag = authTag.Base64EncodeBytes();
            return base64EncodedjweProtectedHeaderSerialized + "." +
                   base64EncodedJweEncryptedKey + "." +
                   base64EncodedIv + "." +
                   base64EncodedCipherText + "." +
                   base64EncodedAuthenticationTag;
        }

        private string InternalDecrypt(string payload, JsonWebKey jsonWebKey, byte[] hmacKey = null)
        {
            IEnumerable<string> parts = null;
            if (!IsValid(payload, out parts))
            {
                throw new JwtException(ErrorCodes.INTERNAL_ERROR, ErrorMessages.INVALID_JWE);
            }

            var serializedProtectedHeader = parts.ElementAt(0).Base64Decode();
            var encryptedContentEncryptionKeyBytes = parts.ElementAt(1).Base64DecodeBytes();
            var iv = parts.ElementAt(2).Base64DecodeBytes();
            var cipherText = parts.ElementAt(3).Base64DecodeBytes();
            var authenticationTag = parts.ElementAt(4).Base64DecodeBytes();
            var protectedHeader = JsonConvert.DeserializeObject(serializedProtectedHeader) as JObject;
            var alg = protectedHeader["alg"].ToString();
            var enc = protectedHeader["enc"].ToString();
            var contentEncryptionKeyHandler = _cekHandlers.FirstOrDefault(e => e.AlgName == alg);
            var encEncryptionHandler = _encHandlers.FirstOrDefault(e => e.EncName == enc);
            if (contentEncryptionKeyHandler == null || encEncryptionHandler == null)
            {
                return null;
            }

            var cek = contentEncryptionKeyHandler.Decrypt(encryptedContentEncryptionKeyBytes, jsonWebKey);
            var splittedCEK = BitHelper.SplitInHalf(cek);
            if (hmacKey == null)
            {
                hmacKey = splittedCEK.First();
            }

            var decrypted = encEncryptionHandler.Decrypt(cipherText, splittedCEK.Last(), iv);
            var aad = Encoding.ASCII.GetBytes(Encoding.UTF8.GetBytes(serializedProtectedHeader).Base64EncodeBytes());
            var al = BitHelper.LongToBytes(aad.Length * 8);
            var hmacInput = BitHelper.Concat(aad, iv, cipherText, al);
            var hmacValue = encEncryptionHandler.BuildHash(hmacKey, hmacInput);
            var newAuthenticationTag = BitHelper.SplitInHalf(hmacValue)[0];
            if (!BitHelper.ConstantTimeEquals(newAuthenticationTag, authenticationTag))
            {
                return null;
            }

            return decrypted;
        }
    }
}