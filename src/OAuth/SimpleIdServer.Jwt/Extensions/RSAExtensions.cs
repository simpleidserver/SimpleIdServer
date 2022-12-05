// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Jwt.Extensions
{
    public static class RSAExtensions
    {
        public static void Import(this RSA rsa, Dictionary<string, string> content)
        {
            var rsaParameters = new RSAParameters
            {
                Modulus = content.TryGet(RSAFields.Modulus),
                Exponent = content.TryGet(RSAFields.Exponent),
                D = content.TryGet(RSAFields.D),
                P = content.TryGet(RSAFields.P),
                Q = content.TryGet(RSAFields.Q),
                DP = content.TryGet(RSAFields.DP),
                DQ = content.TryGet(RSAFields.DQ),
                InverseQ = content.TryGet(RSAFields.InverseQ)
            };
            rsa.ImportParameters(rsaParameters);
        }

        public static Dictionary<string, string> ExtractPublicKey(this RSA rsa)
        {
            var parameters = rsa.ExportParameters(false);
            var result = new Dictionary<string, string>
            {
                { RSAFields.Modulus, parameters.Modulus.Base64EncodeBytes() },
                { RSAFields.Exponent, parameters.Exponent.Base64EncodeBytes() }
            };
            return result;
        }

        public static Dictionary<string, string> ExtractPrivateKey(this RSA rsa)
        {
            try
            {
                var parameters = rsa.ExportParameters(true);
                var result = new Dictionary<string, string>
                {
                    { RSAFields.D, parameters.D.Base64EncodeBytes() },
                    { RSAFields.P, parameters.P.Base64EncodeBytes() },
                    { RSAFields.Q, parameters.Q.Base64EncodeBytes() },
                    { RSAFields.DP, parameters.DP.Base64EncodeBytes() },
                    { RSAFields.DQ, parameters.DQ.Base64EncodeBytes() },
                    { RSAFields.InverseQ, parameters.InverseQ.Base64EncodeBytes() }
                };
                return result;
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }

        public static JsonObject ExtractToJSON(this RSA rsa)
        {
            try
            {
                var parameters = rsa.ExportParameters(true);
                return new JsonObject
                {
                    [RSAFields.D] = parameters.D.Base64EncodeBytes(),
                    [RSAFields.P] = parameters.P.Base64EncodeBytes(),
                    [RSAFields.Q] = parameters.Q.Base64EncodeBytes(),
                    [RSAFields.DP] = parameters.DP.Base64EncodeBytes(),
                    [RSAFields.DQ] = parameters.DQ.Base64EncodeBytes(),
                    [RSAFields.InverseQ] =parameters.InverseQ.Base64EncodeBytes(),
                    [RSAFields.Modulus] = parameters.Modulus.Base64EncodeBytes(),
                    [RSAFields.Exponent] = parameters.Exponent.Base64EncodeBytes()
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
