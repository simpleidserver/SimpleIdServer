// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

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
                { RSAFields.Modulus, Convert.ToBase64String(parameters.Modulus) },
                { RSAFields.Exponent, Convert.ToBase64String(parameters.Exponent) }
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
                    { RSAFields.D, Convert.ToBase64String(parameters.D) },
                    { RSAFields.P, Convert.ToBase64String(parameters.P) },
                    { RSAFields.Q, Convert.ToBase64String(parameters.Q) },
                    { RSAFields.DP, Convert.ToBase64String(parameters.DP) },
                    { RSAFields.DQ, Convert.ToBase64String(parameters.DQ) },
                    { RSAFields.InverseQ, Convert.ToBase64String(parameters.InverseQ) }
                };
                return result;
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
