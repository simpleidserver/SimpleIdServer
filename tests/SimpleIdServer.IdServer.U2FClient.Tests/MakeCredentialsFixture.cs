// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using Fido2NetLib.Objects;
using NUnit.Framework;
using System.Text;

namespace SimpleIdServer.IdServer.U2FClient.Tests
{
    public class MakeCredentialsFixture
    {
        private const string RP = "https://localhost:5001";
        private Fido2 _fido2 = new Fido2(new Fido2Configuration
        {
            ServerName = "SimpleIdServer",
            ServerDomain = "localhost",
            Origins = new HashSet<string> { RP }
        });

        private CredentialCreateOptions GetCredentialCreateOptions()
        {
            var user = "user";
            var authenticatorSelection = new AuthenticatorSelection
            {
                UserVerification = UserVerificationRequirement.Preferred,
                ResidentKey = ResidentKeyRequirement.Discouraged
            };
            var fidoUser = new Fido2User
            {
                Name = user,
                Id = Encoding.UTF8.GetBytes(user)
            };
            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs() { Attestation = "none" }
            };
            return _fido2.RequestNewCredential(fidoUser, new List<PublicKeyCredentialDescriptor>(), authenticatorSelection, AttestationConveyancePreference.None, exts);
        }

        [Test]
        public void When_Build_EnrollResponse_ThenSignatureIsCorrect()
        {
            // ARRANGE
            var options = GetCredentialCreateOptions();
            var attestationBuilder = new FIDOU2FAttestationBuilder();

            // ACT
            var response = attestationBuilder.BuildEnrollResponse(new EnrollParameter
            {
                Challenge = options.Challenge
            });
            var success = _fido2.MakeNewCredentialAsync(response.Response, options, async (arg, c) =>
            {
                return true;
            }).Result;

            // ACT
            Assert.IsTrue(success.Status == "ok");
        }

        [Test]
        public async Task When_Build_AuthenticateResponse_Then_SignatureIsCorrect()
        {
            // ARRANGE
            var options = GetCredentialCreateOptions();
            var attestationBuilder = new FIDOU2FAttestationBuilder();
            var response = attestationBuilder.BuildEnrollResponse(new EnrollParameter
            {
                Challenge = options.Challenge
            });
            var success = _fido2.MakeNewCredentialAsync(response.Response, options, async (arg, c) =>
            {
                return true;
            }).Result;
            var result = success.Result;
            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true,
                DevicePubKey = new AuthenticationExtensionsDevicePublicKeyInputs()
            };
            var descriptor = new PublicKeyCredentialDescriptor(result.Id);
            var assertionOptions = _fido2.GetAssertionOptions(
                new List<PublicKeyCredentialDescriptor> { descriptor },
                UserVerificationRequirement.Discouraged,
                exts
            );

            // ACT
            var storedCounter = result.SignCount;
            var authResponse = attestationBuilder.BuildAuthResponse(new AuthenticationParameter
            {
                Challenge = assertionOptions.Challenge,
                Rp = "localhost",
                Certificate = response.AttestationCertificate,
                CredentialId = response.CredentialId,
                Signcount = storedCounter
            });
            IsUserHandleOwnerOfCredentialIdAsync callback = (args, cancellationToken) =>
            {
                return Task.FromResult(true);
            };
            var res = await _fido2.MakeAssertionAsync(authResponse, assertionOptions, result.PublicKey, new List<byte[]> { result.PublicKey }, storedCounter, callback);

            // ASSERT
            Assert.IsTrue(res.Status == "ok");
        }
    }
}
