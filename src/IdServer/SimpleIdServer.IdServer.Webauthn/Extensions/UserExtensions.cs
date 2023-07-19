// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib.Objects;
using SimpleIdServer.IdServer.Webauthn.Models;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Domains
{
    public static class UserExtensions
    {
        public static User AddFidoCredential(this User user, AttestationVerificationSuccess attestation)
        {
            user.Credentials.Add(new UserCredential
            {
                Id = Convert.ToBase64String(attestation.Id),
                CredentialType = Webauthn.Constants.CredentialType,
                Value = JsonSerializer.Serialize(new StoredFidoCredential
                {
                    Type = attestation.Type,
                    Id = attestation.Id,
                    Descriptor = new PublicKeyCredentialDescriptor(attestation.CredentialId)
                    {
                        Id = attestation.Id
                    },
                    PublicKey = attestation.PublicKey,
                    UserHandle = attestation.User.Id,
                    SignCount = attestation.Counter,
                    CredType = attestation.CredType,
                    RegDate = DateTime.Now,
                    AaGuid = attestation.AaGuid,
                    Transports = attestation.Transports,
                    BE = attestation.BE,
                    BS = attestation.BS,
                    AttestationObject = attestation.AttestationObject,
                    AttestationClientDataJSON = attestation.AttestationClientDataJSON,
                    DevicePublicKeys = new List<byte[]>() { attestation.DevicePublicKey }
                }),
                IsActive = true
            });
            return user;
        }
    }
}
