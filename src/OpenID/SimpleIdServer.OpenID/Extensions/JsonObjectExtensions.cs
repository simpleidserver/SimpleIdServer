// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;

namespace System.Text.Json.Nodes
{
    public static class JsonObjectExtensions
    {
        #region Openid client request

        public static ApplicationKinds? GetApplicationKind(this JsonObject jObj)
        {
            var kind = jObj.GetInt(OpenIdClientParameters.ApplicationKind);
            if (kind == null)
            {
                return null;
            }

            return (ApplicationKinds)kind.Value;
        }

        public static string GetApplicationType(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.ApplicationType);

        public static string GetSectorIdentifierUri(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.SectorIdentifierUri);

        public static string GetSubjectType(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.SubjectType);

        public static string GetIdTokenSignedResponseAlg(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.IdTokenSignedResponseAlg);

        public static string GetIdTokenEncryptedResponseAlg(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.IdTokenEncryptedResponseAlg);

        public static string GetIdTokenEncryptedResponseEnc(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.IdTokenEncryptedResponseEnc);

        public static string GetUserInfoSignedResponseAlg(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.UserInfoSignedResponseAlg);

        public static string GetUserInfoEncryptedResponseAlg(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.UserInfoEncryptedResponseAlg);

        public static string GetUserInfoEncryptedResponseEnc(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.UserInfoEncryptedResponseEnc);

        public static string GetRequestObjectSigningAlg(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.RequestObjectSigningAlg);

        public static string GetRequestObjectEncryptionAlg(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.RequestObjectEncryptionAlg);

        public static string GetRequestObjectEncryptionEnc(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.RequestObjectEncryptionEnc);

        public static double? GetDefaultMaxAge(this JsonObject jObj) => jObj.GetDouble(OpenIdClientParameters.DefaultMaxAge);

        public static bool? GetRequireAuhTime(this JsonObject jObj) => jObj.GetNullableBoolean(OpenIdClientParameters.RequireAuthTime);

        public static IEnumerable<string> GetDefaultAcrValues(this JsonObject jObj) => jObj.GetArray(OpenIdClientParameters.DefaultAcrValues);

        public static IEnumerable<string> GetPostLogoutRedirectUris(this JsonObject jObj) => jObj.GetArray(OpenIdClientParameters.PostLogoutRedirectUris);

        public static string GetInitiateLoginUri(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.InitiateLoginUri);

        public static string GetBCTokenDeliveryMode(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.BCTokenDeliveryMode);

        public static string GetBCClientNotificationEndpoint(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.BCClientNotificationEndpoint);

        public static string GetBCAuthenticationRequestSigningAlg(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.BCAuthenticationRequestSigningAlg);

        public static bool GetBCUserCodeParameter(this JsonObject jObj) => jObj.GetBoolean(OpenIdClientParameters.BCUserCodeParameter);

        public static string GetFrontChannelLogoutUri(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.FrontChannelLogoutUri);

        public static bool GetFrontChannelLogoutSessionRequired(this JsonObject jObj) => jObj.GetBoolean(OpenIdClientParameters.FrontChannelLogoutSessionRequired);

        public static bool GetBackChannelLogoutSessionRequired(this JsonObject jObj) => jObj.GetBoolean(OpenIdClientParameters.BackChannelLogoutSessionRequired);

        public static string GetBackChannelLogoutUri(this JsonObject jObj) => jObj.GetStr(OpenIdClientParameters.BackChannelLogoutUri);

        #endregion

        #region Authorization request

        public static string GetDisplayFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.Display);

        public static IEnumerable<AuthorizationRequestClaimParameter> GetClaimsFromAuthorizationRequest(this JsonObject jObj)
        {
            if (!jObj.ContainsKey(AuthorizationRequestParameters.Claims))
                return new AuthorizationRequestClaimParameter[0];

            var claimsJObj = JsonSerializer.SerializeToNode(jObj[AuthorizationRequestParameters.Claims].ToString()).AsObject();
            return claimsJObj.GetOpenIdClaims();
        }

        public static IEnumerable<AuthorizationRequestClaimParameter> GetOpenIdClaims(this JsonObject claimsJObj)
        {
            var result = new List<AuthorizationRequestClaimParameter>();
            if (claimsJObj.ContainsKey("userinfo"))
            {
                result.AddRange(ExtractClaims(claimsJObj["userinfo"] as JsonObject, AuthorizationRequestClaimTypes.UserInfo));
            }

            if (claimsJObj.ContainsKey("id_token"))
            {
                result.AddRange(ExtractClaims(claimsJObj["id_token"] as JsonObject, AuthorizationRequestClaimTypes.IdToken));
            }

            return result;
        }

        public static IEnumerable<AuthorizationRequestClaimParameter> ExtractClaims(this JsonObject jObj, AuthorizationRequestClaimTypes type)
        {
            if (jObj == null)
                return new AuthorizationRequestClaimParameter[0];

            var result = new List<AuthorizationRequestClaimParameter>();
            foreach (var rec in jObj)
            {
                var claimName = rec.Key;
                var child = rec.Value as JsonObject;
                if (child != null)
                {
                    IEnumerable<string> values = null;
                    if (child.ContainsKey(ClaimsParameter.Value))
                        values = new[] { child.GetStr(ClaimsParameter.Value) };

                    if (child.ContainsKey(ClaimsParameter.Values))
                        values = child.GetArray(ClaimsParameter.Values);

                    result.Add(new AuthorizationRequestClaimParameter(claimName, values, child.GetBoolean(ClaimsParameter.Essential), type));
                }
            }

            return result;
        }

        public static string GetNonceFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.Nonce);

        public static string GetPromptFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.Prompt);

        public static int? GetMaxAgeFromAuthorizationRequest(this JsonObject jObj)
        {
            var str = jObj.GetStr(AuthorizationRequestParameters.MaxAge);
            if (string.IsNullOrWhiteSpace(str))
                return null;

            int result;
            if (int.TryParse(str, out result))
                return result;

            return null;
        }

        public static string GetIdTokenHintFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.IdTokenHint);

        public static string GetRequestFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.Request);

        public static string GetRequestUriFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.RequestUri);

        public static string GetLoginHintFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.LoginHint);

        public static IEnumerable<string> GetAcrValuesFromAuthorizationRequest(this JsonObject jObj)
        {
            var scope = jObj.GetStr(AuthorizationRequestParameters.AcrValue);
            if (string.IsNullOrWhiteSpace(scope)) return new string[0];
            return scope.Split(' ');
        }

        #endregion

        #region RP-Initiated logout
        
        public static string GetIdTokenHintFromRpInitiatedLogoutRequest(this JsonObject jObj) => jObj.GetStr(RPInitiatedLogoutRequest.IdTokenHint);

        public static string GetPostLogoutRedirectUriFromRpInitiatedLogoutRequest(this JsonObject jObj) => jObj.GetStr(RPInitiatedLogoutRequest.PostLogoutRedirectUri);

        public static string GetStateFromRpInitiatedLogoutRequest(this JsonObject jObj) => jObj.GetStr(RPInitiatedLogoutRequest.State);

        #endregion

        #region Back channel authentication request

        public static string GetRequest(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.Request);

        public static string GetLoginHintToken(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.LoginHintToken);

        public static string GetClientNotificationToken(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.ClientNotificationToken);

        public static string GetBindingMessage(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.BindingMessage);

        public static string GetUserCode(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.UserCode);

        public static int? GetRequestedExpiry(this JsonObject jObj) => jObj.GetInt(BCAuthenticationRequestParameters.RequestedExpiry);

        public static int? GetInterval(this JsonObject jObj) => jObj.GetInt(BCAuthenticationRequestParameters.Interval);

        #endregion

        #region Back channel device registration

        public static string GetDeviceRegistrationToken(this JsonObject jObj) => jObj.GetStr(BCDeviceRegistrationRequestParameters.DeviceRegistrationToken);

        #endregion

        #region Back channel confirmation requests

        public static string GetAuthRequestId(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.AuthReqId);

        public static IEnumerable<string> GetPermissionIds(this JsonObject jObj) => jObj.GetArray(BCAuthenticationRequestParameters.PermissionIds);

        #endregion
    }
}