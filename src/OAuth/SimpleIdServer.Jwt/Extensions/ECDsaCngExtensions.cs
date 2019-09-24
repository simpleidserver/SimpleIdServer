// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Extensions
{
    public static class ECDsaCngExtensions
    {
        public static void Import(this ECDsaCng eCDsaCng, Dictionary<string, string> content)
        {
            var ecParameters = new ECParameters();
            ECCurve curve = default(ECCurve);
            if (content.ContainsKey(ECFields.CURVE))
            {
                switch (content[ECFields.CURVE])
                {
                    case "P-256":
                        curve = ECCurve.NamedCurves.nistP256;
                        break;
                    case "P-384":
                        curve = ECCurve.NamedCurves.nistP384;
                        break;
                    case "P-521":
                        curve = ECCurve.NamedCurves.nistP521;
                        break;
                }
            }

            var q = new ECPoint
            {
                X = content.TryGet(ECFields.X),
                Y = content.TryGet(ECFields.Y)
            };
            ecParameters.Q = q;
            ecParameters.Curve = curve;
            ecParameters.D = content.TryGet(ECFields.D);
            eCDsaCng.ImportParameters(ecParameters);
        }

        public static Dictionary<string, string> ExtractPublicKey(this ECDsaCng eCDsaCng)
        {
            var parameters = eCDsaCng.ExportParameters(false);
            string crv = string.Empty;
            if (parameters.Curve.Oid.FriendlyName == ECCurve.NamedCurves.nistP256.Oid.FriendlyName)
            {
                crv = "P-256";
            }
            else if (parameters.Curve.Oid.FriendlyName == ECCurve.NamedCurves.nistP384.Oid.FriendlyName)
            {
                crv = "P-384";
            }
            else if (parameters.Curve.Oid.FriendlyName == ECCurve.NamedCurves.nistP521.Oid.FriendlyName)
            {
                crv = "P-521";
            }

            var result = new Dictionary<string, string>
            {
                { ECFields.CURVE, crv },
                { ECFields.X, parameters.Q.X.Base64EncodeBytes() },
                { ECFields.Y, parameters.Q.Y.Base64EncodeBytes() }
            };
            return result;
        }

        public static Dictionary<string, string> ExtractPrivateKey(this ECDsaCng eCDsaCng)
        {
            var parameters = eCDsaCng.ExportParameters(true);
            var result = new Dictionary<string, string>
            {
                { ECFields.D, parameters.D.Base64EncodeBytes() }
            };
            return result;
        }
    }
}