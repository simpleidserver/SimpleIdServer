// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Fido
{
    public class Constants
    {
        public const string AMR = "webauthn";
        public const string MobileAMR = "mobile";
        public const string CredentialType = "fido";

        public static class EndPoints
        {
            public const string BeginRegister = "u2f/begin-register";
            public const string RegisterStatus = "u2f/status-register";
            public const string BeginQRCodeRegister = "u2f/begin-qr-register";
            public const string EndRegister = "u2f/end-register";
            public const string BeginLogin = "u2f/begin-login";
            public const string EndLogin = "u2f/end-login";
            public const string LoginStatus = "u2f/login-register";
            public const string BeginQRCodeLogin = "u2f/begin-qr-login";
        }
    }
}