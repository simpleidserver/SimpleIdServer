// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt;
using SimpleIdServer.Jwt.Extensions;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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
        public static IEnumerable<KeyValuePair<string, string>> ToEnumerable(this JObject jObj)
        {
            var result = new List<KeyValuePair<string, string>>();
            foreach(JProperty record in jObj.Properties())
            {
                result.Add(new KeyValuePair<string, string>(record.Name, record.Value.ToString()));
            }

            return result;
        }

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
            var authorization = jObj.GetToken(Constants.AuthorizationHeaderName);
            if (authorization == null)
            {
                return null;
            }

            var jArr = authorization as JArray;
            var lst = new List<string>();
            if (jArr != null)
            {
                lst = jArr.Select(_ => _.ToString()).ToList();
            }
            else
            {
                lst.Add(authorization.ToString());
            }

            foreach(var record in lst)
            {
                var value = record.ExtractAuthorizationValue(new string[] { AutenticationSchemes.Bearer, AutenticationSchemes.Basic });
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                var result = ExtractClientCredentialsFromHeader(value);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static ClientCredentials ExtractClientCredentialsFromHeader(string header)
        {
            var splitted = header.Base64Decode().Split(':');
            if (splitted.Count() != 2)
            {
                return null;
            }

            return new ClientCredentials(splitted.First(), splitted.Last());
        }

        #endregion

        #region OAuth client parameters

        public static string GetTokenSignedResponseAlg(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.TokenSignedResponseAlg);
        }

        public static string GetTokenEncryptedResponseAlg(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.TokenEncryptedResponseAlg);
        }

        public static string GetTokenEncryptedResponseEnc(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.TokenEncryptedResponseEnc);
        }

        public static IEnumerable<string> GetGrantTypes(this JObject jObj)
        {
            return jObj.GetArray(OAuthClientParameters.GrantTypes);
        }

        public static IEnumerable<string> GetRedirectUris(this JObject jObj)
        {
            return jObj.GetArray(OAuthClientParameters.RedirectUris);
        }

        public static string GetTokenEndpointAuthMethod(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.TokenEndpointAuthMethod);
        }

        public static IEnumerable<string> GetResponseTypes(this JObject jObj)
        {
            return jObj.GetArray(OAuthClientParameters.ResponseTypes).SelectMany(_ => _.Trim().Split(' '));
        }

        public static Dictionary<string, string> GetClientNames(this JObject jObj)
        {            
            return jObj.GetTranslations(OAuthClientParameters.ClientName);
        }

        public static Dictionary<string, string> GetClientUris(this JObject jObj)
        {
            return jObj.GetTranslations(OAuthClientParameters.ClientUri);
        }

        public static Dictionary<string, string> GetLogoUris(this JObject jObj)
        {
            return jObj.GetTranslations(OAuthClientParameters.LogoUri);
        }

        public static Dictionary<string, string> GetTosUris(this JObject jObj)
        {
            return jObj.GetTranslations(OAuthClientParameters.TosUri);
        }

        public static Dictionary<string, string> GetPolicyUris(this JObject jObj)
        {
            return jObj.GetTranslations(OAuthClientParameters.PolicyUri);
        }

        public static string GetJwksUri(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.JwksUri);
        }

        public static string GetSoftwareId(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.SoftwareId);
        }

        public static string GetSoftwareStatement(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.SoftwareStatement);
        }

        public static string GetSoftwareVersion(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.SoftwareVersion);
        }

        public static string GetRegistrationAccessToken(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.RegistrationAccessToken);
        }

        public static IEnumerable<JsonWebKey> GetJwks(this JObject jObj)
        {
            var str = jObj.GetStr(OAuthClientParameters.Jwks);
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

        public static IEnumerable<string> GetScopes(this JObject jObj)
        {
            var scope = jObj.GetStr(OAuthClientParameters.Scope);
            if (string.IsNullOrWhiteSpace(scope))
            {
                return new string[0];
            }

            return scope.Split(' ');
        }

        public static IEnumerable<string> GetContacts(this JObject jObj)
        {
            return jObj.GetArray(OAuthClientParameters.Contacts);
        }

        public static Dictionary<string, string> GetTranslations(this JObject jObj, string name)
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

        public static string GetTlsClientAuthSubjectDn(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.TlsClientAuthSubjectDN);
        }

        public static string GetTlsClientAuthSanDNS(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.TlsClientAuthSanDNS);
        }

        public static string GetTlsClientAuthSanUri(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.TlsClientAuthSanUri);
        }

        public static string GetTlsClientAuthSanIP(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.TlsClientAuthSanIp);
        }

        public static string GetTlsClientAuthSanEmail(this JObject jObj)
        {
            return jObj.GetStr(OAuthClientParameters.TlsClientAuthSanEmail);
        }

        #endregion

        public static string GetStr(this JObject jObj, string name)
        {
            var result = jObj.GetToken(name);
            return result == null ? null : result.ToString();
        }

        public static X509Certificate2 GetCertificate(this JObject jObj, string name)
        {
            var result = jObj.GetArray(name);
            if (result == null || !result.Any())
            {
                return null;
            }

            foreach(var str in result)
            {
                try
                {
                    var bytes = Encoding.UTF8.GetBytes(Uri.UnescapeDataString(str));
                    return new X509Certificate2(bytes);
                }
                catch
                {

                }
            }

            return null;
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

        public static JObject AddNotEmpty(this JObject jObj, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                jObj.Add(name, value);
            }

            return jObj;
        }

        public static JObject AddNotEmpty(this JObject jObj, string name, IEnumerable<string> values)
        {
            if (values != null && values.Any())
            {
                jObj.Add(name, JArray.FromObject(values));
            }

            return jObj;
        }

        public static JObject AddNotEmpty(this JObject jObj, string name, ICollection<OAuthTranslation> translations)
        {
            if (translations != null && translations.Any())
            {
                foreach (var translation in translations)
                {
                    if (string.IsNullOrWhiteSpace(translation.Language))
                    {
                        jObj.Add(name, translation.Value);
                    }
                    else
                    {
                        jObj.Add($"{name}#{translation.Language}", translation.Value);
                    }
                }
            }

            return jObj;
        }
    }
}
