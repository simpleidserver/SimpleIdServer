// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Domains
{
    public static class ClientExtensions
    {
        public static double? GetDefaultMaxAge(this Client client) => client.GetDoubleParameter("default_max_age");
        /// <summary>
        /// if the token delivery mode is set to ping or push. 
        /// This is the endpoint to which the OP will post a notification after a successful or failed end-user authentication. 
        /// It MUST be an HTTPS URL
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string GetBCClientNotificationEndpoint(this Client client) => client.GetStringParameter("backchannel_client_notification_endpoint");
        /// <summary>
        /// One of the following values: poll, ping, or push.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string GetBCTokenDeliveryMode(this Client client) => client.GetStringParameter("backchannel_token_delivery_mode");
        /// <summary>
        /// Cryptographic algorithm used to secure the JWS identity token. 
        /// </summary>
        public static string GetIdTokenSignedResponseAlg(this Client client) => client.GetStringParameter("id_token_signed_response_alg");
        /// <summary>
        /// Cryptographic algorithm used to encrypt the JWS identity token.
        /// </summary>
        public static string GetIdTokenEncryptedResponseAlg(this Client client) => client.GetStringParameter("id_token_encrypted_response_alg");
        /// <summary>
        /// Content encryption algorithm used perform authenticated encryption on the JWS identity token.
        /// </summary>
        public static string GetIdTokenEncryptedResponseEnc(this Client client) => client.GetStringParameter("id_token_encrypted_response_enc");
        /// <summary>
        /// Alg algorithm that MUST be used for signing Request Objects sent to the OP. 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string GetRequestObjectSigningAlg(this Client client) => client.GetStringParameter("request_object_signing_alg");
        /// <summary>
        /// Alg algorithm the RP is declaring that it may use for encrypting Request Objects sent to the OP.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string GetRequestObjectEncryptionAlg(this Client client) => client.GetStringParameter("request_object_encryption_alg");
        /// <summary>
        /// JWE enc algorithm the RP is declaring that it may use for encrypting Request Objects sent to the OP. 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string GetRequestObjectEncryptionEnc(this Client client) => client.GetStringParameter("request_object_encryption_enc");
        /// <summary>
        /// The JWS algorithm alg value that the Client will use for signing authentication request.
        /// When omitted, the Client will not send signed authentication requests.
        /// </summary>
        public static string GetBCAuthenticationRequestSigningAlg(this Client client) => client.GetStringParameter("backchannel_authentication_request_signing_alg");
    }
}
