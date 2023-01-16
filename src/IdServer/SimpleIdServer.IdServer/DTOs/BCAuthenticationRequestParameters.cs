// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.DTOs
{
    public static class BCAuthenticationRequestParameters
    {
        /// <summary>
        /// REQUIRED if the Client is registered to use Ping or Push modes. 
        /// It is a bearer token provided by the Client that will be used by the OpenID Provider to authenticate the callback request to the Client.
        /// </summary>
        public const string ClientNotificationToken = "client_notification_token";
        /// <summary>
        /// OPTIONAL. A human readable identifier or message intended to be displayed on both the consumption device and the authentication device to interlock them together for the transaction by way of a visual cue for the end-user.
        /// </summary>
        public const string BindingMessage = "binding_message";
        /// <summary>
        /// A secret code, such as password or pin, known only to the user but verifiable by the OP.
        /// This parameter should only be present if client registration parameter backchannel_user_code_parameter indicates support for user code.
        /// </summary>
        public const string UserCode = "user_code";
        /// <summary>
        /// A positive integer allowing the client to request the expires_in value for the auth_req_id the server will return.
        /// </summary>
        public const string RequestedExpiry = "requested_expiry";
        /// <summary>
        /// A token containing information identifying the end-user for whom authentication is being requested. 
        /// </summary>
        public const string LoginHintToken = "login_hint_token";
        /// <summary>
        /// Signed authentication request.
        /// </summary>
        public const string Request = "request";
        /// <summary>
        /// Permission identifiers.
        /// </summary>
        public const string PermissionIds = "permission_ids";
    }
}
