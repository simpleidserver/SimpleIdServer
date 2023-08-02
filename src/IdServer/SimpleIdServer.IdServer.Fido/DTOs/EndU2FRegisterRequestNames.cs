// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Fido.DTOs
{
    public static class EndU2FRegisterRequestNames
    {
        public const string SessionId = "session_id";
        public const string Login = "login";
        public const string AuthenticatorAttestationRawResponse = "attestation";
        public const string DeviceData = "device_data";
    }
}
