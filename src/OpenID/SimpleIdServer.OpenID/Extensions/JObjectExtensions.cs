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
        #region Register request

        public static string GetApplicationTypeFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.ApplicationType);
        }

        public static string GetSectorIdentifierUriFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.SectorIdentifierUri);
        }

        public static string GetSubjectTypeFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.SubjectType);
        }

        public static string GetIdTokenSignedResponseAlgFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.IdTokenSignedResponseAlg);
        }

        public static string GetIdTokenEncryptedResponseAlgFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.IdTokenEncryptedResponseAlg);
        }

        public static string GetIdTokenEncryptedResponseEncFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.IdTokenEncryptedResponseEnc);
        }

        public static string GetUserInfoSignedResponseAlgFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.UserInfoSignedResponseAlg);
        }

        public static string GetUserInfoEncryptedResponseAlgFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.UserInfoEncryptedResponseAlg);
        }

        public static string GetUserInfoEncryptedResponseEncFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.UserInfoEncryptedResponseEnc);
        }

        public static string GetRequestObjectSigningAlgFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.RequestObjectSigningAlg);
        }

        public static string GetRequestObjectEncryptionAlgFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.RequestObjectEncryptionAlg);
        }

        public static string GetRequestObjectEncryptionEncFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.RequestObjectEncryptionEnc);
        }

        public static double? GetDefaultMaxAgeFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetDouble(RegisterRequestParameters.DefaultMaxAge);
        }

        public static bool? GetRequireAuhTimeFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetNullableBoolean(RegisterRequestParameters.RequireAuthTime);
        }

        public static IEnumerable<string> GetDefaultAcrValuesFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetArray(RegisterRequestParameters.DefaultAcrValues);
        }

        public static IEnumerable<string> GetPostLogoutRedirectUrisFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetArray(RegisterRequestParameters.PostLogoutRedirectUris);
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
    }
}