// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Webauthn.Models;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido.Extensions
{
    public static class UserCredentialExtensions
    {
        public static StoredFidoCredential GetFidoCredential(this UserCredential userCredential) => userCredential.CredentialType == Fido.Constants.CredentialType ? JsonSerializer.Deserialize<StoredFidoCredential>(userCredential.Value) : null;
    }
}
