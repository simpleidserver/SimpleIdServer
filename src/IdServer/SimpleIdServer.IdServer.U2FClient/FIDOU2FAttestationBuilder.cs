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

        public EnrollResult BuildEnrollResponse(EnrollParameter parameter)
        {
            var clientDataJson = JsonSerializer.SerializeToUtf8Bytes(new
            {
                type = "webauthn.create",
                challenge = parameter.Challenge,
                origin = parameter.Origin
            });
            var credentialId = RandomNumberGenerator.GetBytes(16);
            var attestationCertificate = BuildAttestationCertificate();
            var publicKey = GetCredentialPublicKey(attestationCertificate);
            var signature = BuildEnrollSignature(parameter.Rp, publicKey, attestationCertificate, clientDataJson, credentialId);
            var authData = GetAuthData(publicKey, parameter.Rp, credentialId);
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
            return new EnrollResult(attestationCertificate, rawResponse, credentialId);
        }

        public AuthenticatorAssertionRawResponse BuildAuthResponse(AuthenticationParameter parameter)
        {
            var clientDataJson = JsonSerializer.SerializeToUtf8Bytes(new
            {
                type = "webauthn.get",
                challenge = parameter.Challenge,
                origin = parameter.Origin
            });
            var publicKey = GetCredentialPublicKey(parameter.Certificate);
            var authData = GetAuthData(publicKey, parameter.Rp, parameter.CredentialId, parameter.Signcount);
            var signature = BuildAuthSignature(authData, clientDataJson, parameter.Certificate);
            var userHandle = new byte[16];
            RandomNumberGenerator.Fill(userHandle);
            var rawResponse = new AuthenticatorAssertionRawResponse
            {
                Response = new AuthenticatorAssertionRawResponse.AssertionResponse
                {
                    // AttestationObject = attestationObject,
                    AuthenticatorData = authData.ToByteArray(),
                    Signature = signature,
                    ClientDataJson = clientDataJson,
                    UserHandle = userHandle
                },
                // credential identifier on the device
                Id = parameter.CredentialId,
                // credential identifier on the device
                RawId = parameter.CredentialId,
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

        private byte[] BuildEnrollSignature(string rp, CredentialPublicKey credentialPublicKey, AttestationCertificateResult certificate, byte[] clientDataJson, byte[] credentialId)
        {
            var rpIdHash = SHA256.HashData(Encoding.UTF8.GetBytes(rp));
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
        }

        private byte[] BuildAuthSignature(AuthenticatorData authData, byte[] clientDataJson, AttestationCertificateResult certificate)
        {
            var authDataPayload = authData.ToByteArray();
            var hashedClientDataJson = SHA256.HashData(clientDataJson);
            byte[] data = new byte[authDataPayload.Length + hashedClientDataJson.Length];
            Buffer.BlockCopy(authDataPayload, 0, data, 0, authDataPayload.Length);
            Buffer.BlockCopy(hashedClientDataJson, 0, data, authDataPayload.Length, hashedClientDataJson.Length);
            var signature = SignData(COSE.KeyType.EC2, COSE.Algorithm.ES256, data, certificate.PrivateKey, null, null);
            return signature;
        }

        private AuthenticatorData GetAuthData(CredentialPublicKey credentialPublicKey, string rp, byte[] credentialId, uint? sigCount = null)
        {
            var rpIdHash = SHA256.HashData(Encoding.UTF8.GetBytes(rp));
            var flags = AuthenticatorFlags.AT | AuthenticatorFlags.ED | AuthenticatorFlags.UP | AuthenticatorFlags.UV;
            if(sigCount == null)
            {
                byte[] signCount = RandomNumberGenerator.GetBytes(2);
                sigCount = BitConverter.ToUInt16(signCount, 0);
            }
            else
            {
                sigCount++;
            }

            var acd = new AttestedCredentialData(Guid.Empty, credentialId, credentialPublicKey);
            var extBytes = new CborMap { { "sid", true } }.Encode();
            return new AuthenticatorData(rpIdHash, flags, sigCount.Value, acd, new Fido2NetLib.Objects.Extensions(extBytes));
        }
    }
}