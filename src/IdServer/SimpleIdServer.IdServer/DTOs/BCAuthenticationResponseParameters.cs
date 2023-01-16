// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
{
    public class BCAuthenticationResponseParameters
    {
        /// <summary>
        /// This is a unique identifier to identify the authentication request made by the Client.
        /// </summary>
        public const string AuthReqId = "auth_req_id";
        /// <summary>
        /// A JSON number with a positive integer value indicating the expiration time of the "auth_req_id" in seconds since the authentication request was received. 
        /// </summary>
        public const string ExpiresIn = "expires_in";
        /// <summary>
        /// A JSON number with a positive integer value indicating the minimum amount of time in seconds that the Client MUST wait between polling requests to the token endpoint.
        /// </summary>
        public const string Interval = "interval";
    }
}
