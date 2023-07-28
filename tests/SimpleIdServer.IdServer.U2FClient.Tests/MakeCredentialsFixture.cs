// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib;
using Fido2NetLib.Objects;
using NUnit.Framework;
using SimpleIdServer.IdServer.U2FClient;
using System.Text;

namespace SimpleIdServer.IdServer.U2FClient.Tests
{
    public class MakeCredentialsFixture
    {
        [Test]
        public void When_Build_EnrollResponse_ThenSignatureIsCorrect()
        {
            // ARRANGE
            var fido2 = new Fido2(new Fido2Configuration
            {
                ServerDomain = "https://localhost:5001",
                Origins = new HashSet<string> { "https://localhost:5001" }
            });
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
            var options = fido2.RequestNewCredential(fidoUser, new List<PublicKeyCredentialDescriptor>(), authenticatorSelection, AttestationConveyancePreference.None, exts);
            var attestationBuilder = new FIDOU2FAttestationBuilder();

            // ACT
            var response = attestationBuilder.BuildEnrollResponse(new AttestationParameter
            {
                Challenge = options.Challenge
            });
            var success = fido2.MakeNewCredentialAsync(response, options, async (arg, c) =>
            {
                return true;
            }).Result;

            // ACT
            Assert.NotNull(response);
        }
    }
}
