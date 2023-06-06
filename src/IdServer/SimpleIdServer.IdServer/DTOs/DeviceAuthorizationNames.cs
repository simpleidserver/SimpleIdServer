// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
{
    public static class DeviceAuthorizationNames
    {
        /// <summary>
        /// Device verification code.
        /// </summary>
        public const string DeviceCode = "device_code";
        /// <summary>
        /// End-user verification code.
        /// </summary>
        public const string UserCode = "user_code";
        /// <summary>
        /// The end-user verification URI on the authorization server.
        /// </summary>
        public const string VerificationUri = "verification_uri";
        /// <summary>
        /// A verification URI that includes the "user_code".
        /// </summary>
        public const string VerificationUriComplete = "verification_uri_complete";
        /// <summary>
        /// The lifetime in seconds of the "device_code" and "user_code".
        /// </summary>
        public const string ExpiresIn = "expires_in";
        /// <summary>
        /// The minimum amount of time in seconds that the client SHOULD wait between polling requests to the token endpoint.
        /// </summary>
        public const string Interval = "interval";
    }
}
