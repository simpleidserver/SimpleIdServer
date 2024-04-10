// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib.Objects;
using SimpleIdServer.IdServer.Fido.Extensions;
using SimpleIdServer.IdServer.Webauthn.Models;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Domains
{
    public static class UserExtensions
    {
        public static User AddFidoCredential(this User user, RegisteredPublicKeyCredential attestation)
        {
            user.Credentials.Add(new UserCredential
            {
                Id = Convert.ToBase64String(attestation.Id),
                CredentialType = Fido.Constants.CredentialType,
                Value = JsonSerializer.Serialize(new StoredFidoCredential
                {
                    Type = attestation.Type,
                    Id = attestation.Id,
                    Descriptor = new PublicKeyCredentialDescriptor(attestation.Id),
                    PublicKey = attestation.PublicKey,
                    UserHandle = attestation.User.Id,
                    RegDate = DateTime.Now,
                    AaGuid = attestation.AaGuid,
                    Transports = attestation.Transports,
                    AttestationObject = attestation.AttestationObject,
                    DevicePublicKeys = new List<byte[]>() { attestation.DevicePublicKey }
                }),
                IsActive = true
            });
            return user;
        }

        public static IEnumerable<UserCredential> GetFidoCredentials(this User user) => user.Credentials.Where(c => c.CredentialType == Fido.Constants.CredentialType);

        public static IEnumerable<StoredFidoCredential> GetStoredFidoCredentials(this User user) => user.GetFidoCredentials().Select(c => c.GetFidoCredential());
    }
}
