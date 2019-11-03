// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.OAuth.DTOs;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public class ClientCredentials
    {
        public ClientCredentials(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
    }

    public static class JObjectExtensions
    {
        #region Authorization request

        public static string GetCodeChallengeFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.CodeChallenge);
        }

        public static string GetCodeChallengeMethodFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.CodeChallengeMethod);
        }

        public static string GetStateFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.State);
        }

        public static IEnumerable<string> GetScopesFromAuthorizationRequest(this JObject jObj)
        {
            var scope = jObj.GetStr(AuthorizationRequestParameters.Scope);
            if (string.IsNullOrWhiteSpace(scope))
            {
                return new string[0];
            }

            return scope.Split(' ');
        }

        public static IEnumerable<string> GetResponseTypesFromAuthorizationRequest(this JObject jObj)
        {
            var responseType = jObj.GetStr(AuthorizationRequestParameters.ResponseType);
            if (string.IsNullOrWhiteSpace(responseType))
            {
                return new string[0];
            }

            return responseType.Split(' ');
        }

        public static IEnumerable<string> GetUILocalesFromAuthorizationRequest(this JObject jObj)
        {
            var uiLocales = jObj.GetStr(AuthorizationRequestParameters.UILocales);
            if (string.IsNullOrWhiteSpace(uiLocales))
            {
                return new string[0];
            }

            return uiLocales.Split(' ');
        }

        public static string GetClientIdFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.ClientId);
        }

        public static string GetResponseModeFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.ResponseMode);
        }

        public static string GetRedirectUriFromAuthorizationRequest(this JObject jObj)
        {
            return jObj.GetStr(AuthorizationRequestParameters.RedirectUri);
        }

        #endregion

        #region Token request

        public static string GetGrantType(this JObject jObj)
        {
            return jObj.GetStr(TokenRequestParameters.GrantType);
        }

        public static string GetClientAssertion(this JObject jObj)
        {
            return jObj.GetStr(TokenRequestParameters.ClientAssertion);
        }

        public static string GetClientAssertionType(this JObject jObj)
        {
            return jObj.GetStr(TokenRequestParameters.ClientAssertionType);
        }

        public static string GetClientId(this JObject jObj)
        {
            return jObj.GetStr(TokenRequestParameters.ClientId);
        }

        public static string GetClientSecret(this JObject jObj)
        {
            return jObj.GetStr(TokenRequestParameters.ClientSecret);
        }

        public static string GetRefreshToken(this JObject jObj)
        {
            return jObj.GetStr(TokenRequestParameters.RefreshToken);
        }

        public static string GetAuthorizationCode(this JObject jObj)
        {
            return jObj.GetStr(TokenRequestParameters.Code);
        }

        public static string GetCodeVerifier(this JObject jObj)
        {
            return jObj.GetStr(TokenRequestParameters.CodeVerifier);
        }

        public static ClientCredentials GetClientCredentials(this JObject jObj)
        {
            var authorizationHeaderValue = jObj.GetStr("Authorization");
            if (string.IsNullOrWhiteSpace(authorizationHeaderValue))
            {
                return null;
            }

            var splitted = authorizationHeaderValue.Split(' ');
            if (splitted.Count() != 2 || splitted.First() != TokenTypes.Bearer)
            {
                return null;
            }

            splitted = splitted.Last().Base64Decode().Split(':');
            if (splitted.Count() != 2)
            {
                return null;
            }

            return new ClientCredentials(splitted.First(), splitted.Last());
        }

        #endregion

        #region Register request

        public static string GetTokenSignedResponseAlgFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.TokenSignedResponseAlg);
        }

        public static string GetTokenEncryptedResponseAlgFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.TokenEncryptedResponseAlg);
        }

        public static string GetTokenEncryptedResponseEncFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.TokenEncryptedResponseEnc);
        }

        public static IEnumerable<string> GetGrantTypesFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetArray(RegisterRequestParameters.GrantTypes);
        }

        public static IEnumerable<string> GetRedirectUrisFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetArray(RegisterRequestParameters.RedirectUris);
        }

        public static string GetTokenEndpointAuthMethodFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.TokenEndpointAuthMethod);
        }

        public static IEnumerable<string> GetResponseTypesFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetArray(RegisterRequestParameters.ResponseTypes);
        }

        public static Dictionary<string, string> GetClientNamesFromRegisterRequest(this JObject jObj)
        {            
            return jObj.GetTranslationsFromRegisterRequest(RegisterRequestParameters.ClientName);
        }

        public static Dictionary<string, string> GetClientUrisFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetTranslationsFromRegisterRequest(RegisterRequestParameters.ClientUri);
        }

        public static Dictionary<string, string> GetLogoUrisFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetTranslationsFromRegisterRequest(RegisterRequestParameters.LogoUri);
        }

        public static Dictionary<string, string> GetTosUrisFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetTranslationsFromRegisterRequest(RegisterRequestParameters.TosUri);
        }

        public static Dictionary<string, string> GetPolicyUrisFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetTranslationsFromRegisterRequest(RegisterRequestParameters.PolicyUri);
        }

        public static string GetJwksUriFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.JwksUri);
        }

        public static string GetSoftwareIdFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.SoftwareId);
        }

        public static string GetSoftwareStatementFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.SoftwareStatement);
        }

        public static string GetSoftwareVersionFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetStr(RegisterRequestParameters.SoftwareVersion);
        }

        public static IEnumerable<JsonWebKey> GetJwksFromRegisterRequest(this JObject jObj)
        {
            var str = jObj.GetStr(RegisterRequestParameters.Jwks);
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            var json = JObject.Parse(str);
            if (json == null || !json.ContainsKey("keys"))
            {
                return null;
            }

            var keysJson = json["keys"].ToString();
            return JsonConvert.DeserializeObject<JArray>(keysJson).Select(k => JsonWebKey.Deserialize(k.ToString()));
        }

        public static IEnumerable<string> GetScopesFromRegisterRequest(this JObject jObj)
        {
            var scope = jObj.GetStr(RegisterRequestParameters.Scope);
            if (string.IsNullOrWhiteSpace(scope))
            {
                return new string[0];
            }

            return scope.Split(' ');
        }

        public static IEnumerable<string> GetContactsFromRegisterRequest(this JObject jObj)
        {
            return jObj.GetArray(RegisterRequestParameters.Contacts);
        }

        public static Dictionary<string, string> GetTranslationsFromRegisterRequest(this JObject jObj, string name)
        {
            var cultureNames = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.Name);
            var result = new Dictionary<string, string>();
            foreach(var property in jObj)
            {
                if (!property.Key.StartsWith(name))
                {
                    continue;
                }

                var splitted = property.Key.Split('#');
                var language = string.Empty;
                if (splitted.Count() == 2)
                {
                    language = splitted.Last();
                }

                if (!cultureNames.Contains(language))
                {
                    continue;
                }

                result.Add(language, property.Value.ToString());
            }

            return result;
        }

        #endregion

        public static string GetStr(this JObject jObj, string name)
        {
            var result = jObj.GetToken(name);
            return result == null ? null : result.ToString();
        }

        public static bool? GetNullableBoolean(this JObject jObj, string name)
        {
            var result = jObj.GetToken(name);
            if (result == null)
            {
                return null;
            }

            bool b;
            if (bool.TryParse(result.ToString(), out b))
            {
                return b;
            }

            return null;
        }

        public static bool GetBoolean(this JObject jObj, string name)
        {
            var result = jObj.GetToken(name);
            if (result == null)
            {
                return false;
            }

            bool b;
            if (bool.TryParse(result.ToString(), out b))
            {
                return b;
            }

            return false;
        }

        public static int? GetInt(this JObject jObj, string name)
        {
            var str = jObj.GetStr(name);
            int result;
            if (string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out result))
            {
                return null;
            }

            return result;
        }

        public static double? GetDouble(this JObject jObj, string name)
        {
            var str = jObj.GetStr(name);
            double result;
            if (string.IsNullOrWhiteSpace(str) || !double.TryParse(str, out result))
            {
                return null;
            }

            return result;
        }

        public static IEnumerable<string> GetArray(this JObject jObj, string name)
        {
            var result = jObj.GetToken(name);
            if (!(result is JArray))
            {
                return new string[0];
            }

            var lst = new List<string>();
            foreach(var record in (JArray)result)
            {
                lst.Add(record.ToString());
            }

            return lst;
        }

        public static JToken GetToken(this JObject jObj, string name)
        {
            JToken jToken;
            if (!jObj.TryGetValue(name, out jToken))
            {
                return null;
            }

            return jToken;
        }
    }
}
