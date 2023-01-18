// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace System.Text.Json.Nodes
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

    public static class JsonObjectExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> ToEnumerable(this JsonObject jObj) => jObj.ToDictionary(r => r.Key, r =>
        {
            if (r.Value is JsonValue) return r.Value.GetValue<string>();
            else return r.Value.ToJsonString();
        });

        #region Authorization request

        /// <summary>
        /// Indicates the target service or resource to which access is being requested. 
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetResourcesFromAuthorizationRequest(this JsonObject jObj)
        {
            var result = jObj.GetToken(AuthorizationRequestParameters.Resource);
            if (result == null) return new string[0];
            if (result is JsonValue) return new string[1] { result.GetValue<string>() };
            var lst = new List<string>();
            foreach (var record in result.AsArray())
                lst.Add(record.ToString());

            return lst;
        }

        /// <summary>
        /// Hint to the OpenId Provider regarding the end-user for whom authentication is beging requested.
        /// The value may contain an email address, phone number, account number, subject identifier etc...
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetLoginHintFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.LoginHint);

        public static string GetRequestFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.Request);

        public static string GetRequestUriFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.RequestUri);

        public static string GetPromptFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.Prompt);

        public static IEnumerable<string> GetAcrValuesFromAuthorizationRequest(this JsonObject jObj)
        {
            var scope = jObj.GetStr(AuthorizationRequestParameters.AcrValue);
            if (string.IsNullOrWhiteSpace(scope)) return new string[0];
            return scope.Split(' ');
        }

        public static string GetDisplayFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.Display);

        public static string GetCodeChallengeFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.CodeChallenge);

        public static string GetCodeChallengeMethodFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.CodeChallengeMethod);

        public static string GetStateFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.State);

        public static string GetNonceFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.Nonce);

        public static IEnumerable<string> GetScopesFromAuthorizationRequest(this JsonObject jObj)
        {
            var scope = jObj.GetStr(AuthorizationRequestParameters.Scope);
            if (string.IsNullOrWhiteSpace(scope))
            {
                return new string[0];
            }

            return scope.Split(' ');
        }

        public static IEnumerable<string> GetResponseTypesFromAuthorizationRequest(this JsonObject jObj)
        {
            var responseType = jObj.GetStr(AuthorizationRequestParameters.ResponseType);
            if (string.IsNullOrWhiteSpace(responseType))
            {
                return new string[0];
            }

            return responseType.Split(' ');
        }

        public static IEnumerable<string> GetUILocalesFromAuthorizationRequest(this JsonObject jObj)
        {
            var uiLocales = jObj.GetStr(AuthorizationRequestParameters.UILocales);
            if (string.IsNullOrWhiteSpace(uiLocales))
            {
                return new string[0];
            }

            return uiLocales.Split(' ');
        }

        public static string GetClientIdFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.ClientId);

        public static string GetResponseModeFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.ResponseMode);

        public static string GetRedirectUriFromAuthorizationRequest(this JsonObject jObj)
        {
            var result = jObj.GetStr(AuthorizationRequestParameters.RedirectUri);
            result = HttpUtility.UrlDecode(result);
            return result;
        }

        /// <summary>
        /// An ID token previously issued to the client by the OpenId provider being passed back as a hint to identify the end-user for whom authentication is being requested.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetIdTokenHintFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.IdTokenHint);

        /// <summary>
        /// String value identifying an individual grant managed by a particular authorization server for a certain client and a certain resource owner. 
        /// he grant_id value must have been issued by the respective authorization server and the respective client must be authorized to use the particular grant id.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetGrantIdFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.GrantId);

        /// <summary>
        /// String value controlling the way the authorization server shall handle the grant when processing an authorization request.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetGrantManagementActionFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.GrantManagementAction);

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

        public static IEnumerable<AuthorizedClaim> GetClaimsFromAuthorizationRequest(this JsonObject jObj)
        {
            if (!jObj.ContainsKey(AuthorizationRequestParameters.Claims))
                return new AuthorizedClaim[0];

            var claimsJObj = jObj[AuthorizationRequestParameters.Claims] as JsonObject;
            return claimsJObj.GetOpenIdClaims();
        }

        public static IEnumerable<AuthorizedClaim> GetOpenIdClaims(this JsonObject claimsJObj)
        {
            var result = new List<AuthorizedClaim>();
            if (claimsJObj.ContainsKey("userinfo"))
                result.AddRange(ExtractClaims(claimsJObj["userinfo"] as JsonObject, AuthorizationClaimTypes.UserInfo));

            if (claimsJObj.ContainsKey("id_token"))
                result.AddRange(ExtractClaims(claimsJObj["id_token"] as JsonObject, AuthorizationClaimTypes.IdToken));

            return result;
        }

        public static IEnumerable<AuthorizedClaim> ExtractClaims(this JsonObject jObj, AuthorizationClaimTypes type)
        {
            if (jObj == null)
                return new AuthorizedClaim[0];

            var result = new List<AuthorizedClaim>();
            foreach (var rec in jObj)
            {
                var claimName = rec.Key;
                var child = rec.Value as JsonObject;
                if (child != null)
                {
                    IEnumerable<string> values = null;
                    if (child.ContainsKey(ClaimsParameters.Value))
                        values = new[] { child.GetStr(ClaimsParameters.Value) };

                    if (child.ContainsKey(ClaimsParameters.Values))
                        values = child.GetArray(ClaimsParameters.Values);

                    result.Add(new AuthorizedClaim(claimName, values, child.GetBoolean(ClaimsParameters.Essential), type));
                }
            }

            return result;
        }

        #endregion

        #region Token request

        public static string GetGrantType(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.GrantType);

        public static string GetClientAssertion(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClientAssertion);

        public static string GetClientAssertionType(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClientAssertionType);

        public static string GetClientId(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClientId);

        public static string GetClientSecret(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClientSecret);

        public static string GetRefreshToken(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.RefreshToken);

        public static string GetAuthorizationCode(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.Code);

        public static string GetCodeVerifier(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.CodeVerifier);

        public static string GetRedirectUri(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.RedirectUri);

        public static ClientCredentials GetClientCredentials(this JsonObject jObj)
        {
            var authorization = jObj.GetToken(Constants.AuthorizationHeaderName);
            if (authorization == null)
            {
                return null;
            }

            var lst = new List<string>();
            if (authorization is JsonArray)
            {
                var jArr = authorization.AsArray();
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

        public static int? GetRefreshTokenExpirationTimeInSeconds(this JsonObject jObj) => jObj.GetInt(OAuthClientParameters.RefreshTokenExpirationTimeInSeconds);

        public static int? GetTokenExpirationTimeInSeconds(this JsonObject jObj) => jObj.GetInt(OAuthClientParameters.TokenExpirationTimeInSeconds);

        public static string GetTokenSignedResponseAlg(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.TokenSignedResponseAlg);

        public static string GetTokenEncryptedResponseAlg(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.TokenEncryptedResponseAlg);

        public static string GetTokenEncryptedResponseEnc(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.TokenEncryptedResponseEnc);

        public static IEnumerable<string> GetGrantTypes(this JsonObject jObj) => jObj.GetArray(OAuthClientParameters.GrantTypes);

        public static IEnumerable<string> GetRedirectUris(this JsonObject jObj) => jObj.GetArray(OAuthClientParameters.RedirectUris);

        public static string GetTokenEndpointAuthMethod(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.TokenEndpointAuthMethod);

        public static IEnumerable<string> GetResponseTypes(this JsonObject jObj) =>jObj.GetArray(OAuthClientParameters.ResponseTypes).SelectMany(_ => _.Trim().Split(' '));

        public static string GetClientName(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.ClientName);

        public static Dictionary<string, string> GetClientNames(this JsonObject jObj) => jObj.GetTranslations(OAuthClientParameters.ClientName);

        public static Dictionary<string, string> GetClientUris(this JsonObject jObj) => jObj.GetTranslations(OAuthClientParameters.ClientUri);

        public static Dictionary<string, string> GetLogoUris(this JsonObject jObj) => jObj.GetTranslations(OAuthClientParameters.LogoUri);

        public static Dictionary<string, string> GetTosUris(this JsonObject jObj) => jObj.GetTranslations(OAuthClientParameters.TosUri);

        public static Dictionary<string, string> GetPolicyUris(this JsonObject jObj) => jObj.GetTranslations(OAuthClientParameters.PolicyUri);

        public static string GetJwksUri(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.JwksUri);

        public static string GetSoftwareId(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.SoftwareId);

        public static string GetSoftwareStatement(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.SoftwareStatement);

        public static string GetSoftwareVersion(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.SoftwareVersion);

        public static string GetRegistrationAccessToken(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.RegistrationAccessToken);

        public static IEnumerable<JsonWebKey> GetJwks(this JsonObject jObj)
        {
            var str = jObj.GetStr(OAuthClientParameters.Jwks);
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            var keys = JsonObject.Parse(str)?["keys"]?.AsArray();
            if (keys == null) return null;

            return keys.Select(k => JsonExtensions.DeserializeFromJson<JsonWebKey>(k.ToJsonString()));
        }

        public static IEnumerable<string> GetScopes(this JsonObject jObj)
        {
            var scope = jObj.GetStr(OAuthClientParameters.Scope);
            if (string.IsNullOrWhiteSpace(scope))
            {
                return new string[0];
            }

            return scope.Split(' ');
        }

        public static IEnumerable<string> GetContacts(this JsonObject jObj) => jObj.GetArray(OAuthClientParameters.Contacts);

        public static Dictionary<string, string> GetTranslations(this JsonObject jObj, string name)
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

        public static string GetTlsClientAuthSubjectDn(this JsonObject jObj) =>  jObj.GetStr(OAuthClientParameters.TlsClientAuthSubjectDN);

        public static string GetTlsClientAuthSanDNS(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.TlsClientAuthSanDNS);

        public static string GetTlsClientAuthSanUri(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.TlsClientAuthSanUri);

        public static string GetTlsClientAuthSanIP(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.TlsClientAuthSanIp);

        public static string GetTlsClientAuthSanEmail(this JsonObject jObj) => jObj.GetStr(OAuthClientParameters.TlsClientAuthSanEmail);

        #endregion

        #region OAuth Scope parameters

        public static string GetScopeName(this JsonObject jObj) => jObj.GetStr(OAuthScopeParameters.Name);

        public static IEnumerable<string> GetClaims(this JsonObject jObj) => jObj.GetArray(OAuthScopeParameters.Claims);

        #endregion

        #region Introspection request

        public static string GetToken(this JsonObject jObj) => jObj.GetStr(IntrospectionRequestParameters.Token);

        #endregion

        #region Back channel authentication request

        public static string GetRequest(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.Request);

        /// <summary>
        /// A token containing information identifying the end-user for whom authentication is begin requested.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetLoginHintToken(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.LoginHintToken);

        /// <summary>
        /// If the client is registered to use Ping or Push modes.
        /// If is a bearer token provided by the client that will be used by the OpenID Provider to authenticate the callback request to the client.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetClientNotificationToken(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.ClientNotificationToken);

        /// <summary>
        /// A human readable-identifier or message intended to be displayed on both the consumption device and the authentication device to interlock them together for the transaction by way of a visual cue for the end-user.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetBindingMessage(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.BindingMessage);

        public static string GetUserCode(this JsonObject jObj) => jObj.GetStr(BCAuthenticationRequestParameters.UserCode);

        /// <summary>
        /// A positive integer allowing the client to request the expires_in value for the auth_req_id the server will return.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static int? GetRequestedExpiry(this JsonObject jObj) => jObj.GetInt(BCAuthenticationRequestParameters.RequestedExpiry);

        #endregion

        #region Back channel confirmation requests

        public static string GetAuthRequestId(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.AuthReqId);

        public static IEnumerable<string> GetPermissionIds(this JsonObject jObj) => jObj.GetArray(BCAuthenticationRequestParameters.PermissionIds);

        #endregion

        #region RP-Initiated logout

        public static string GetIdTokenHintFromRpInitiatedLogoutRequest(this JsonObject jObj) => jObj.GetStr(RPInitiatedLogoutRequest.IdTokenHint);

        public static string GetPostLogoutRedirectUriFromRpInitiatedLogoutRequest(this JsonObject jObj) => jObj.GetStr(RPInitiatedLogoutRequest.PostLogoutRedirectUri);

        public static string GetStateFromRpInitiatedLogoutRequest(this JsonObject jObj) => jObj.GetStr(RPInitiatedLogoutRequest.State);

        #endregion

        public static void AddOrReplace(this JsonObject jObj, string name, string value)
        {
            if(jObj.ContainsKey(name))
                jObj[name] = value;
            else jObj.Add(name, value);
        }

        public static void AddOrReplace(this JsonObject jObj, IEnumerable<Claim> claims)
        {
            foreach (var claim in claims)
                jObj.AddOrReplace(claim.Type, claim.Value);
        }

        public static string GetStr(this JsonObject jObj, string name)
        {
            var result = jObj.GetToken(name);
            return result == null ? null : result.ToString();
        }

        public static X509Certificate2 GetCertificate(this JsonObject jObj, string name)
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

        public static bool? GetNullableBoolean(this JsonObject jObj, string name)
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

        public static bool GetBoolean(this JsonObject jObj, string name)
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

        public static int? GetInt(this JsonObject jObj, string name)
        {
            var str = jObj.GetStr(name);
            int result;
            if (string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out result))
            {
                return null;
            }

            return result;
        }

        public static double? GetDouble(this JsonObject jObj, string name)
        {
            var str = jObj.GetStr(name);
            double result;
            if (string.IsNullOrWhiteSpace(str) || !double.TryParse(str, out result))
            {
                return null;
            }

            return result;
        }

        public static IEnumerable<string> GetArray(this JsonObject jObj, string name)
        {
            var result = jObj.GetToken(name);
            if (!(result is JsonArray)) return new string[0];

            var lst = new List<string>();
            foreach(var record in result.AsArray())
                lst.Add(record.ToString());

            return lst;
        }

        public static JsonNode GetToken(this JsonObject jObj, string name)
        {
            if (!jObj.ContainsKey(name)) return null;
            var result = jObj[name];
            return result;
        }

        public static JsonObject AddNotEmpty(this JsonObject jObj, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                jObj.Add(name, value);

            return jObj;
        }

        public static JsonObject AddNotEmpty(this JsonObject jObj, string name, DateTime? value)
        {
            if (value != null)
                jObj.Add(name, value);

            return jObj;
        }

        public static JsonObject AddNotEmpty(this JsonObject jObj, string name, IEnumerable<string> values)
        {
            values = values.Where(v => !string.IsNullOrWhiteSpace(v));
            if (values != null && values.Any())
            {
                jObj.Add(name, JsonSerializer.SerializeToNode(values));
            }

            return jObj;
        }

        public static JsonObject AddNotEmpty(this JsonObject jObj, string name, ICollection<Translation> translations)
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
