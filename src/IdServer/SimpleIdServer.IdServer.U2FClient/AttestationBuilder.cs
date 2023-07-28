// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib.Cbor;
using Fido2NetLib.Objects;
using NSec.Cryptography;
using System.Formats.Asn1;
using System.Security.Cryptography;

namespace SimpleIdServer.IdServer.U2FClient
{
    public class AttestationBuilder
    {
        internal CredentialPublicKey MakeCredentialPublicKey(COSE.KeyType kty, COSE.Algorithm alg, COSE.EllipticCurve crv, byte[] x, byte[] y) => MakeCredentialPublicKey(kty, alg, crv, x, y, null, null);

        internal CredentialPublicKey MakeCredentialPublicKey(COSE.KeyType kty, COSE.Algorithm alg, COSE.EllipticCurve crv, byte[] x) => MakeCredentialPublicKey(kty, alg, crv, x, null, null, null);

        internal CredentialPublicKey MakeCredentialPublicKey(COSE.KeyType kty, COSE.Algorithm alg, byte[] n, byte[] e) => MakeCredentialPublicKey(kty, alg, null, null, null, n, e);

        internal CredentialPublicKey MakeCredentialPublicKey(COSE.KeyType kty, COSE.Algorithm alg, COSE.EllipticCurve? crv, byte[] x, byte[] y, byte[] n, byte[] e)
        {
            var cpk = new CborMap();
            cpk.Add(COSE.KeyCommonParameter.KeyType, kty);
            cpk.Add(COSE.KeyCommonParameter.Alg, alg);
            switch (kty)
            {
                case COSE.KeyType.EC2:
                    cpk.Add(COSE.KeyTypeParameter.X, x);
                    cpk.Add(COSE.KeyTypeParameter.Y, y);
                    cpk.Add((int)COSE.KeyTypeParameter.Crv, (int)crv);
                    break;
                case COSE.KeyType.RSA:
                    cpk.Add(COSE.KeyTypeParameter.N, n);
                    cpk.Add(COSE.KeyTypeParameter.E, e);
                    break;
                case COSE.KeyType.OKP:
                    cpk.Add(COSE.KeyTypeParameter.X, x);
                    cpk.Add((int)COSE.KeyTypeParameter.Crv, (int)crv);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kty), kty, "Invalid COSE key type");
            }

            return new CredentialPublicKey(cpk);
        }

        internal CredentialPublicKey MakeCredentialPublicKey((COSE.KeyType, COSE.Algorithm, COSE.EllipticCurve) param)
        {
            var (kty, alg, crv) = param;
            CredentialPublicKey cpk;
            switch (kty)
            {
                case COSE.KeyType.EC2:
                    {
                        var ecdsa = MakeECDsa(alg, crv);
                        var ecparams = ecdsa.ExportParameters(true);
                        cpk = MakeCredentialPublicKey(kty, alg, crv, ecparams.Q.X, ecparams.Q.Y);
                        break;
                    }
                case COSE.KeyType.RSA:
                    {
                        var rsa = RSA.Create();
                        var rsaparams = rsa.ExportParameters(true);
                        cpk = MakeCredentialPublicKey(kty, alg, rsaparams.Modulus, rsaparams.Exponent);
                        break;
                    }
                case COSE.KeyType.OKP:
                    {
                        MakeEdDSA(out var privateKeySeed, out byte[] publicKey, out _);
                        cpk = MakeCredentialPublicKey(kty, alg, COSE.EllipticCurve.Ed25519, publicKey);
                        break;
                    }
                default:
                    throw new ArgumentException(nameof(kty), $"Missing or unknown kty {kty}");
            }

            return cpk;
        }

        /*
        internal SigResult SignData(byte[] attToBeSigned, COSE.KeyType kty, COSE.Algorithm alg, COSE.EllipticCurve crv)
        {
            ECDsa ecdsa = null;
            RSA rsa = null;
            Key privateKey = null;
            byte[] publicKey = null;

            switch (kty)
            {
                case COSE.KeyType.EC2:
                    {
                        ecdsa = MakeECDsa(alg, crv);
                        break;
                    }
                case COSE.KeyType.RSA:
                    {
                        rsa = RSA.Create();
                        break;
                    }
                case COSE.KeyType.OKP:
                    {
                        MakeEdDSA(out var privateKeySeed, out publicKey, out byte[] expandedPrivateKey);
                        privateKey = Key.Import(SignatureAlgorithm.Ed25519, expandedPrivateKey, KeyBlobFormat.RawPrivateKey);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(kty), $"Missing or unknown kty {kty}");
            }

            return SignData(attToBeSigned, kty, alg, crv, ecdsa, rsa, privateKey, publicKey);
        }

        internal SigResult SignData(byte[] attToBeSigned, COSE.KeyType kty, COSE.Algorithm alg, COSE.EllipticCurve curve, ECDsa ecdsa = null, RSA rsa = null, Key expandedPrivateKey = null, byte[] publicKey = null)
        {
            switch (kty)
            {
                case COSE.KeyType.EC2:
                    {
                        var ecparams = ecdsa.ExportParameters(true);
                        var credentialPublicKey = MakeCredentialPublicKey(kty, alg, curve, ecparams.Q.X, ecparams.Q.Y);
                        var signature = ecdsa.SignData(attToBeSigned, CryptoUtils.HashAlgFromCOSEAlg(alg));
                        return new SigResult(credentialPublicKey, EcDsaSigFromSig(signature, ecdsa.KeySize));
                    }
                case COSE.KeyType.RSA:
                    {
                        RSASignaturePadding padding;
                        switch (alg) // https://www.iana.org/assignments/cose/cose.xhtml#algorithms
                        {
                            case COSE.Algorithm.PS256:
                            case COSE.Algorithm.PS384:
                            case COSE.Algorithm.PS512:
                                padding = RSASignaturePadding.Pss;
                                break;

                            case COSE.Algorithm.RS1:
                            case COSE.Algorithm.RS256:
                            case COSE.Algorithm.RS384:
                            case COSE.Algorithm.RS512:
                                padding = RSASignaturePadding.Pkcs1;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(alg), $"Missing or unknown alg {alg}");
                        }

                        var rsaparams = rsa.ExportParameters(true);
                        var credentialPublicKey = MakeCredentialPublicKey(kty, alg, rsaparams.Modulus, rsaparams.Exponent);
                        return new SigResult(credentialPublicKey, rsa.SignData(attToBeSigned, CryptoUtils.HashAlgFromCOSEAlg(alg), padding));
                    }
                case COSE.KeyType.OKP:
                    {
                        var credentialPublicKey = MakeCredentialPublicKey(kty, alg, COSE.EllipticCurve.Ed25519, publicKey);
                        return new SigResult(credentialPublicKey, SignatureAlgorithm.Ed25519.Sign(expandedPrivateKey, attToBeSigned));
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kty), $"Missing or unknown kty {kty}");
            }
        }
        */

        internal byte[] SignData(COSE.KeyType kty, COSE.Algorithm alg, byte[] data, ECDsa ecdsa = null, RSA rsa = null, byte[] expandedPrivateKey = null)
        {
            switch (kty)
            {
                case COSE.KeyType.EC2:
                    {
                        var signature = ecdsa.SignData(data, CryptoUtils.HashAlgFromCOSEAlg(alg));
                        return EcDsaSigFromSig(signature, ecdsa.KeySize);
                    }
                case COSE.KeyType.RSA:
                    {
                        RSASignaturePadding padding;
                        switch (alg) // https://www.iana.org/assignments/cose/cose.xhtml#algorithms
                        {
                            case COSE.Algorithm.PS256:
                            case COSE.Algorithm.PS384:
                            case COSE.Algorithm.PS512:
                                padding = RSASignaturePadding.Pss;
                                break;

                            case COSE.Algorithm.RS1:
                            case COSE.Algorithm.RS256:
                            case COSE.Algorithm.RS384:
                            case COSE.Algorithm.RS512:
                                padding = RSASignaturePadding.Pkcs1;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(alg), $"Missing or unknown alg {alg}");
                        }
                        return rsa.SignData(data, CryptoUtils.HashAlgFromCOSEAlg(alg), padding);
                    }
                case COSE.KeyType.OKP:
                    {
                        Key privateKey = Key.Import(SignatureAlgorithm.Ed25519, expandedPrivateKey, KeyBlobFormat.RawPrivateKey);
                        return SignatureAlgorithm.Ed25519.Sign(privateKey, data);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kty), $"Missing or unknown kty {kty}");
            }
        }

        internal void MakeEdDSA(out byte[] privateKeySeed, out byte[] publicKey, out byte[] expandedPrivateKey)
        {
            privateKeySeed = new byte[32];
            RandomNumberGenerator.Fill(privateKeySeed);
            var key = Key.Create(SignatureAlgorithm.Ed25519, new KeyCreationParameters() { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
            expandedPrivateKey = key.Export(KeyBlobFormat.RawPrivateKey);
            publicKey = key.Export(KeyBlobFormat.RawPublicKey);
        }

        internal ECDsa MakeECDsa(COSE.Algorithm alg, COSE.EllipticCurve crv)
        {
            ECCurve curve;
            switch (alg)
            {
                case COSE.Algorithm.ES256K:
                    switch (crv)
                    {
                        case COSE.EllipticCurve.P256K:
                            if (OperatingSystem.IsMacOS())
                            {
                                // see https://github.com/dotnet/runtime/issues/47770
                                throw new PlatformNotSupportedException($"No support currently for secP256k1 on MacOS");
                            }

                            curve = ECCurve.CreateFromFriendlyName("secP256k1");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(crv), $"Missing or unknown crv {crv}");
                    }
                    break;
                case COSE.Algorithm.ES256:
                    curve = crv switch
                    {
                        COSE.EllipticCurve.P256 => ECCurve.NamedCurves.nistP256,
                        _ => throw new ArgumentOutOfRangeException(nameof(crv), $"Missing or unknown crv {crv}"),
                    };
                    break;
                case COSE.Algorithm.ES384:
                    curve = crv switch // https://www.iana.org/assignments/cose/cose.xhtml#elliptic-curves
                    {
                        COSE.EllipticCurve.P384 => ECCurve.NamedCurves.nistP384,
                        _ => throw new ArgumentOutOfRangeException(nameof(crv), $"Missing or unknown crv {crv}"),
                    };
                    break;
                case COSE.Algorithm.ES512:
                    curve = crv switch // https://www.iana.org/assignments/cose/cose.xhtml#elliptic-curves
                    {
                        COSE.EllipticCurve.P521 => ECCurve.NamedCurves.nistP521,
                        _ => throw new ArgumentOutOfRangeException(nameof(crv), $"Missing or unknown crv {crv}"),
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alg), $"Missing or unknown alg {alg}");
            }

            return ECDsa.Create(curve);
        }

        internal byte[] EcDsaSigFromSig(ReadOnlySpan<byte> sig, int keySize)
        {
            var coefficientSize = (int)Math.Ceiling((decimal)keySize / 8);
            var r = sig.Slice(0, coefficientSize);
            var s = sig.Slice(sig.Length - coefficientSize);

            var writer = new AsnWriter(AsnEncodingRules.BER);

            ReadOnlySpan<byte> zero = new byte[1] { 0 };

            using (writer.PushSequence())
            {
                writer.WriteIntegerUnsigned(r.TrimStart(zero));
                writer.WriteIntegerUnsigned(s.TrimStart(zero));
            }

            return writer.Encode();
        }

        internal record SigResult
        {
            public SigResult(CredentialPublicKey publicKey, byte[] payload)
            {
                PublicKey = publicKey;
                Payload = payload;
            }

            public CredentialPublicKey PublicKey { get; private set; } = null!;
            public byte[] Payload { get; private set; } = null!;
        }
    }
}
