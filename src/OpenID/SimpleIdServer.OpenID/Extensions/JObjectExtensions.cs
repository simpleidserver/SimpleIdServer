// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.Extensions
{
    public static class JObjectExtensions
    {
        #region Openid client request

        public static string GetApplicationType(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.ApplicationType);
        }

        public static string GetSectorIdentifierUri(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.SectorIdentifierUri);
        }

        public static string GetSubjectType(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.SubjectType);
        }

        public static string GetIdTokenSignedResponseAlg(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.IdTokenSignedResponseAlg);
        }

        public static string GetIdTokenEncryptedResponseAlg(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.IdTokenEncryptedResponseAlg);
        }

        public static string GetIdTokenEncryptedResponseEnc(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.IdTokenEncryptedResponseEnc);
        }

        public static string GetUserInfoSignedResponseAlg(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.UserInfoSignedResponseAlg);
        }

        public static string GetUserInfoEncryptedResponseAlg(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.UserInfoEncryptedResponseAlg);
        }

        public static string GetUserInfoEncryptedResponseEnc(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.UserInfoEncryptedResponseEnc);
        }

        public static string GetRequestObjectSigningAlg(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.RequestObjectSigningAlg);
        }

        public static string GetRequestObjectEncryptionAlg(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.RequestObjectEncryptionAlg);
        }

        public static string GetRequestObjectEncryptionEnc(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.RequestObjectEncryptionEnc);
        }

        public static double? GetDefaultMaxAge(this JObject jObj)
        {
            return jObj.GetDouble(OpenIdClientParameters.DefaultMaxAge);
        }

        public static bool? GetRequireAuhTime(this JObject jObj)
        {
            return jObj.GetNullableBoolean(OpenIdClientParameters.RequireAuthTime);
        }

        public static IEnumerable<string> GetDefaultAcrValues(this JObject jObj)
        {
            return jObj.GetArray(OpenIdClientParameters.DefaultAcrValues);
        }

        public static IEnumerable<string> GetPostLogoutRedirectUris(this JObject jObj)
        {
            return jObj.GetArray(OpenIdClientParameters.PostLogoutRedirectUris);
        }

        public static string GetInitiateLoginUri(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.InitiateLoginUri);
        }

        public static string GetBCTokenDeliveryMode(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.BCTokenDeliveryMode);
        }

        public static string GetBCClientNotificationEndpoint(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.BCClientNotificationEndpoint);
        }

        public static string GetBCAuthenticationRequestSigningAlg(this JObject jObj)
        {
            return jObj.GetStr(OpenIdClientParameters.BCAuthenticationRequestSigningAlg);
        }

        public static bool GetBCUserCodeParameter(this JObject jObj)
        {
            return jObj.GetBoolean(OpenIdClientParameters.BCUserCodeParameter);
        }

        #endregion

        #region Authorization request

        public static string GetDisplayFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.Display);
        }

        public static IEnumerable<AuthorizationRequestClaimParameter> GetClaimsFromAuthorizationRequest(this JObject jObj)
        {
            if (!jObj.ContainsKey(AuthorizationRequestParameters.Claims))
            {
                return new AuthorizationRequestClaimParameter[0];
            }

            var claimsJObj = JsonConvert.DeserializeObject<JObject>(jObj[AuthorizationRequestParameters.Claims].ToString());
            return claimsJObj.GetClaims();
        }

        public static IEnumerable<AuthorizationRequestClaimParameter> GetClaims(this JObject claimsJObj)
        {
            var result = new List<AuthorizationRequestClaimParameter>();
            if (claimsJObj.ContainsKey("userinfo"))
            {
                result.AddRange(ExtractClaims(claimsJObj["userinfo"] as JObject, AuthorizationRequestClaimTypes.UserInfo));
            }

            if (claimsJObj.ContainsKey("id_token"))
            {
                result.AddRange(ExtractClaims(claimsJObj["id_token"] as JObject, AuthorizationRequestClaimTypes.IdToken));
            }

            return result;
        }

        public static IEnumerable<AuthorizationRequestClaimParameter> ExtractClaims(this JObject jObj, AuthorizationRequestClaimTypes type)
        {
            if (jObj == null)
            {
                return new AuthorizationRequestClaimParameter[0];
            }

            var result = new List<AuthorizationRequestClaimParameter>();
            foreach (var rec in jObj)
            {
                var claimName = rec.Key;
                var child = rec.Value as JObject;
                if (child != null)
                {
                    IEnumerable<string> values = null;
                    if (child.ContainsKey(ClaimsParameter.Value))
                    {
                        values = new[] { child.GetStr(ClaimsParameter.Value) };
                    }

                    if (child.ContainsKey(ClaimsParameter.Values))
                    {
                        values = child.GetArray(ClaimsParameter.Values);
                    }

                    result.Add(new AuthorizationRequestClaimParameter(claimName, values, child.GetBoolean(ClaimsParameter.Essential), type));
                }
            }

            return result;
        }

        public static string GetNonceFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.Nonce);
        }

        public static string GetPromptFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.Prompt);
        }

        public static int? GetMaxAgeFromAuthorizationRequest(this JObject jObj)
        {
            var str = jObj.GetStr(AuthorizationRequestParameters.MaxAge);
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            int result;
            if (int.TryParse(str, out result))
            {
                return result;
            }

            return null;
        }

        public static string GetIdTokenHintFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.IdTokenHint);
        }

        public static string GetRequestFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.Request);
        }

        public static string GetRequestUriFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.RequestUri);
        }

        public static string GetLoginHintFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.LoginHint);
        }

        public static IEnumerable<string> GetAcrValuesFromAuthorizationRequest(this JObject jObj)
        {
            var scope = jObj.GetStr(AuthorizationRequestParameters.AcrValue);
            if (string.IsNullOrWhiteSpace(scope))
            {
                return new string[0];
            }

            return scope.Split(' ');
        }

        #endregion

        #region RP-Initiated logout
        
        public static string GetIdTokenHintFromRpInitiatedLogoutRequest(this JObject jObj)
        {
            return jObj.GetStr(RPInitiatedLogoutRequest.IdTokenHint);
        }

        public static string GetPostLogoutRedirectUriFromRpInitiatedLogoutRequest(this JObject jObj)
        {
            return jObj.GetStr(RPInitiatedLogoutRequest.PostLogoutRedirectUri);
        }

        public static string GetStateFromRpInitiatedLogoutRequest(this JObject jObj)
        {
            return jObj.GetStr(RPInitiatedLogoutRequest.State);
        }

        #endregion

        #region Back channel authentication request

        public static string GetRequest(this JObject jObj)
        {
            return jObj.GetStr(BCAuthenticationRequestParameters.Request);
        }

        public static string GetLoginHintToken(this JObject jObj)
        {
            return jObj.GetStr(BCAuthenticationRequestParameters.LoginHintToken);
        }

        public static string GetClientNotificationToken(this JObject jObj)
        {
            return jObj.GetStr(BCAuthenticationRequestParameters.ClientNotificationToken);
        }

        public static string GetBindingMessage(this JObject jObj)
        {
            return jObj.GetStr(BCAuthenticationRequestParameters.BindingMessage);
        }

        public static string GetUserCode(this JObject jObj)
        {
            return jObj.GetStr(BCAuthenticationRequestParameters.UserCode);
        }

        public static int? GetRequestedExpiry(this JObject jObj)
        {
            return jObj.GetInt(BCAuthenticationRequestParameters.RequestedExpiry);
        }

        public static int? GetInterval(this JObject jObj)
        {
            return jObj.GetInt(BCAuthenticationRequestParameters.Interval);
        }

        #endregion

        #region Back channel device registration

        public static string GetDeviceRegistrationToken(this JObject jObj)
        {
            return jObj.GetStr(BCDeviceRegistrationRequestParameters.DeviceRegistrationToken);
        }

        #endregion

        #region Back channel confirmation requests

        public static string GetAuthRequestId(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.AuthReqId);
        }

        public static IEnumerable<string> GetPermissionIds(this JObject jObj)
        {
            return jObj.GetArray(BCAuthenticationRequestParameters.PermissionIds);
        }

        #endregion
    }
}