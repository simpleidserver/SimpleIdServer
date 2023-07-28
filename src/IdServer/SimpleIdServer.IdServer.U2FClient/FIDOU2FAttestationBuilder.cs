// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using Fido2NetLib.Cbor;
using Fido2NetLib.Objects;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace SimpleIdServer.IdServer.U2FClient
{
    /// <summary>
    /// https://medium.com/webauthnworks/verifying-fido-u2f-attestations-in-fido2-f83fab80c355
    /// </summary>
    public class FIDOU2FAttestationBuilder : AttestationBuilder
    {
        public X509BasicConstraintsExtension notCAExt = new X509BasicConstraintsExtension(false, false, 0, false);
        public string Format = "fido-u2f";

        public AuthenticatorAttestationRawResponse BuildEnrollResponse(AttestationParameter parameter)
        {
            var clientDataJson = JsonSerializer.SerializeToUtf8Bytes(new
            {
                type = "webauthn.create",
                challenge = parameter.Challenge,
                origin = parameter.Rp
            });
            var credentialId = RandomNumberGenerator.GetBytes(16);
            var attestationCertificate = BuildAttestationCertificate();
            var publicKey = GetCredentialPublicKey(attestationCertificate);
            var signature = BuildVerificationData(parameter, publicKey, attestationCertificate, clientDataJson, credentialId);
            var authData = GetAuthData(publicKey, parameter, credentialId);
            var attestationObject = new CborMap {
                // The attestation statement format identifier.
                { "fmt", "fido-u2f" },
                // The attestation statement, whose format is identified by the "fmt" object member
                { "attStmt", new CborMap{
                    { "x5c", new CborArray {
                        attestationCertificate.AttestationCertificate.RawData
                    } },
                    { "sig",  signature }
                } },
                // The authenticator data object.
                { "authData", new CborByteString(authData.ToByteArray()) }
            }.Encode();
            var rawResponse = new AuthenticatorAttestationRawResponse
            {
                Response = new AuthenticatorAttestationRawResponse.ResponseData
                {
                    AttestationObject = attestationObject,
                    ClientDataJson = clientDataJson
                },
                Extensions = null,
                // credential identifier on the device
                Id = credentialId,
                // credential identifier on the device
                RawId = credentialId,
                Type = PublicKeyCredentialType.PublicKey
            };
            return rawResponse;
        }

        private CredentialPublicKey GetCredentialPublicKey(AttestationCertificateResult certificateResult)
        {
            var ecdsaAtt = certificateResult.PrivateKey;
            var ecparams = ecdsaAtt.ExportParameters(true);
            return MakeCredentialPublicKey(COSE.KeyType.EC2, COSE.Algorithm.ES256, COSE.EllipticCurve.P256, ecparams.Q.X, ecparams.Q.Y);
        }

        private AttestationCertificateResult BuildAttestationCertificate()
        {
            var ecdsaAtt = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var attRequest = new CertificateRequest("CN=U2F, OU=Authenticator Attestation, O=SID, C=US", ecdsaAtt, HashAlgorithmName.SHA256);
            attRequest.CertificateExtensions.Add(notCAExt);
            var attestnCert = attRequest.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(2));
            return new AttestationCertificateResult(attestnCert, ecdsaAtt);
        }

        private byte[] BuildVerificationData(AttestationParameter parameter, CredentialPublicKey credentialPublicKey, AttestationCertificateResult certificate, byte[] clientDataJson, byte[] credentialId)
        {
            var rpIdHash = SHA256.HashData(Encoding.UTF8.GetBytes(parameter.Rp));
            var clientDataHash = SHA256.HashData(clientDataJson);
            var x = (byte[])credentialPublicKey.GetCborObject().GetValue((long)COSE.KeyTypeParameter.X);
            var y = (byte[])credentialPublicKey.GetCborObject().GetValue((long)COSE.KeyTypeParameter.Y);
            var publicKeyU2F = DataHelper.Concat(new byte[1] { 0x4 }, x, y);
            var verificationData = DataHelper.Concat(
                new byte[1] { 0x00 },
                rpIdHash,
                clientDataHash,
                credentialId,
                publicKeyU2F
            );

            var signature = SignData(COSE.KeyType.EC2, COSE.Algorithm.ES256, verificationData, certificate.PrivateKey, null, null);

            return signature;

            /*
            AuthenticatorData GetAuthData()
            {
                byte[] signCount = RandomNumberGenerator.GetBytes(2);
                var flags = AuthenticatorFlags.AT | AuthenticatorFlags.ED | AuthenticatorFlags.UP | AuthenticatorFlags.UV;
                var sigCount = BitConverter.ToUInt16(signCount, 0);
                var acd = new AttestedCredentialData(new Guid("F1D0F1D0-F1D0-F1D0-F1D0-F1D0F1D0F1D0"), credentialId, credentialPublicKey);
                var extBytes = new CborMap { { "sid", true } }.Encode();
                return new AuthenticatorData(rpIdHash, flags, sigCount, acd, new Fido2NetLib.Objects.Extensions(extBytes));
            }
            */
        }

        private AuthenticatorData GetAuthData(CredentialPublicKey credentialPublicKey, AttestationParameter parameter, byte[] credentialId)
        {
            var rpIdHash = SHA256.HashData(Encoding.UTF8.GetBytes(parameter.Rp));
            byte[] signCount = RandomNumberGenerator.GetBytes(2);
            var flags = AuthenticatorFlags.AT | AuthenticatorFlags.ED | AuthenticatorFlags.UP | AuthenticatorFlags.UV;
            var sigCount = BitConverter.ToUInt16(signCount, 0);
            var acd = new AttestedCredentialData(Guid.Empty, credentialId, credentialPublicKey);
            var extBytes = new CborMap { { "sid", true } }.Encode();
            return new AuthenticatorData(rpIdHash, flags, sigCount, acd, new Fido2NetLib.Objects.Extensions(extBytes));
        }

        public record AttestationCertificateResult
        {
            public AttestationCertificateResult(X509Certificate2 attestationCertificate, ECDsa privateKey)
            {
                AttestationCertificate = attestationCertificate;
                PrivateKey = privateKey;
            }

            public X509Certificate2 AttestationCertificate { get; private set; }
            public ECDsa PrivateKey { get; private set; }
        }
    }
}