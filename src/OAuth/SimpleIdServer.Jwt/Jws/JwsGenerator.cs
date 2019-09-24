// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using SimpleIdServer.Jwt.Exceptions;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.Jwt.Jws.Handlers;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Jwt.Jws
{
    public interface IJwsGenerator
    {
        JwsHeader ExtractHeader(string payload);
        JwsPayload ExtractPayload(string jws);
        JwsPayload ExtractPayload(string jws, JsonWebKey jsonWebKey);
        bool IsValid(string payload, out IEnumerable<string> parts);
        string Build(string payload, string alg, JsonWebKey jsonWebKey);
        bool Check(string jws, JsonWebKey jsonWebKey);
    }

    public class JwsGenerator : IJwsGenerator
    {
        private readonly IEnumerable<ISignHandler> _signHandlers;

        public JwsGenerator(IEnumerable<ISignHandler> signHandlers)
        {
            _signHandlers = signHandlers;
        }

        public JwsHeader ExtractHeader(string payload)
        {
            IEnumerable<string> parts = null;
            if (IsValid(payload, out parts))
            {
                return JsonConvert.DeserializeObject<JwsHeader>(parts.First().Base64Decode());
            }

            return null;
        }

        public JwsPayload ExtractPayload(string jws)
        {
            IEnumerable<string> parts = null;
            if (IsValid(jws, out parts))
            {
                return JsonConvert.DeserializeObject<JwsPayload>(parts.ElementAt(1).Base64Decode());
            }

            return null;
        }

        public JwsPayload ExtractPayload(string jws, JsonWebKey jsonWebKey)
        {
            if (!Check(jws, jsonWebKey))
            {
                throw new JwtException(ErrorCodes.INTERNAL_ERROR, ErrorMessages.BAD_JWS);
            }

            return JsonConvert.DeserializeObject<JwsPayload>(jws.GetParts().ElementAt(1).Base64Decode());
        }

        public bool IsValid(string payload, out IEnumerable<string> parts)
        {
            var p = payload.GetParts();
            if (p.Count() == 3)
            {
                parts = p;
                return true;
            }

            parts = null;
            return false;
        }
        
        public string Build(string payload, string alg, JsonWebKey jsonWebKey)
        {
            var signHandler = _signHandlers.FirstOrDefault(s => s.AlgName == alg);
            if (signHandler == null)
            {
                throw new JwtException(ErrorCodes.INTERNAL_ERROR, string.Format(ErrorMessages.UNKNOWN_ALG, alg));
            }

            var header = new JwsHeader("JWT", alg, jsonWebKey == null ? null : jsonWebKey.Kid);
            var serializedProtectedHeader = JsonConvert.SerializeObject(header).ToString();
            var base64EncodedSerializedProtectedHeader = serializedProtectedHeader.Base64Encode();
            var base64EncodedSerializedPayload = payload.Base64Encode();
            var combinedProtectedHeaderAndPayLoad = string.Format("{0}.{1}", base64EncodedSerializedProtectedHeader, base64EncodedSerializedPayload);
            var signature = signHandler.Sign(combinedProtectedHeaderAndPayLoad, jsonWebKey);
            return string.Format("{0}.{1}", combinedProtectedHeaderAndPayLoad, signature);
        }

        public bool Check(string jws, JsonWebKey jsonWebKey)
        {
            IEnumerable<string> parts = null;
            if (!IsValid(jws, out parts))
            {
                throw new JwtException(ErrorCodes.INTERNAL_ERROR, ErrorMessages.INVALID_JWS);
            }

            var serializedProtectedHeader = parts.ElementAt(0).Base64Decode();
            var protectedHeader = JsonConvert.DeserializeObject<JwsHeader>(serializedProtectedHeader);
            var serializedPayload = parts.ElementAt(1).Base64Decode();
            var signature = parts.ElementAt(2).Base64DecodeBytes();
            var combinedProtectedHeaderAndPayLoad = $"{parts.ElementAt(0)}.{parts.ElementAt(1)}";
            var signHandler = _signHandlers.FirstOrDefault(s => s.AlgName == protectedHeader.Alg);
            return signHandler.Verify(combinedProtectedHeaderAndPayLoad, signature, jsonWebKey);
        }
    }
}
