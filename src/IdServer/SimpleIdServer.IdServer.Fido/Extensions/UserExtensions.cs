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
        public static User AddFidoCredential(this User user, string credentialType, RegisteredPublicKeyCredential attestation)
        {
            var existingCredential = user.Credentials.SingleOrDefault(c => c.CredentialType == credentialType);
            if(existingCredential != null) user.Credentials.Remove(existingCredential);
            user.Credentials.Add(new UserCredential
            {
                Id = Guid.NewGuid().ToString(),
                CredentialType = credentialType,
                Value = JsonSerializer.Serialize(new StoredFidoCredential
                {
                    Type = PublicKeyCredentialType.PublicKey,
                    Id = attestation.Id,
                    Descriptor = new PublicKeyCredentialDescriptor(attestation.Id),
                    PublicKey = attestation.PublicKey,
                    UserHandle = attestation.User?.Id,
                    UserId = attestation.User?.Id,
                    SignCount = attestation.SignCount,
                    CredType = attestation.Type.ToString(),
                    RegDate = DateTime.Now,
                    AaGuid = attestation.AaGuid,
                    Transports = attestation.Transports?.ToArray() ?? Array.Empty<AuthenticatorTransport>(),
                    BE = attestation.IsBackupEligible,
                    BS = attestation.IsBackedUp,
                    AttestationObject = attestation.AttestationObject,
                    AttestationClientDataJSON = attestation.AttestationClientDataJson,
                    DevicePublicKeys = new List<byte[]>()
                }),
                IsActive = true
            });
            return user;
        }

        public static IEnumerable<UserCredential> GetFidoCredentials(this User user, string credentialType) => user.Credentials.Where(c => c.CredentialType == credentialType);

        public static IEnumerable<StoredFidoCredential> GetStoredFidoCredentials(this User user, string credentialType) => user.GetFidoCredentials(credentialType).Select(c => c.GetFidoCredential(credentialType));
    }
}
