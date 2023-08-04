// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Fido
{
    public static class ErrorMessages
    {
        public const string SESSION_CANNOT_BE_EXTRACTED = "either the session doesn't exist or is expired";
        public const string REGISTRATION_NOT_CONFIRMED = "registration is not yet confirmed";
        public const string AUTHENTICATION_NOT_CONFIRMED = "authentication is not yet confirmed";
    }
}
