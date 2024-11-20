﻿// Copyright (c) SimpleIdServer. All rights reserved.
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
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
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

        #region UMA resource request

        public static IEnumerable<string> GetUMAScopesFromRequest(this JsonObject jsonObj) => jsonObj.GetArray(UMAResourceNames.ResourceScopes);

        public static Dictionary<string, string> GetUMADescriptionFromRequest(this JsonObject jsonObj) => jsonObj.GetTranslations(UMAResourceNames.Description);

        public static string GetUMAIconURIFromRequest(this JsonObject jsonObj) => jsonObj.GetStr(UMAResourceNames.IconUri);

        public static Dictionary<string, string> GetUMANameFromRequest(this JsonObject jsonObj) => jsonObj.GetTranslations(UMAResourceNames.Name);

        public static string GetUMATypeFromRequest(this JsonObject jsonObj) => jsonObj.GetStr(UMAResourceNames.Type);

        public static string GetUMASubjectFromRequest(this JsonObject jsonObj) => jsonObj.GetStr(UMAResourceNames.Subject);

        #endregion

        #region Ticket request

        public static string GetResourceId(this JsonObject jsonObj) => jsonObj.GetStr(UMAPermissionNames.ResourceId);

        public static IEnumerable<string> GetResourceScopes(this JsonObject jsonObj) => jsonObj.GetArray(UMAPermissionNames.ResourceScopes);

        #endregion

        #region Authorization request

        /// <summary>
        /// OPTIONAL : STRING containing the wallet's OPENID CONNER ISSUER URL.
        /// The Credential Issuer will use the discovery process as defined in [SIOPv2] to determine the Wallet's capabilities and endpoints. 
        /// RECOMMENDED in Dynamic Credential Request.
        /// </summary>
        /// <returns></returns>
        public static string GetWalletIssuer(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.WalletIssuer);

        /// <summary>
        ///  OPTIONAL. JSON String containing an opaque user hint the Wallet MAY use in subsequent callbacks to optimize the user's experience. 
        ///  RECOMMENDED in Dynamic Credential Request.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetUserHint(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.UserHint);

        /// <summary>
        /// OPTIONAL. String value identifying a certain processing context at the Credential Issuer.
        /// A value for this parameter is typically passed in a Credential Offer from the Credential Issuer to the Wallet.
        /// This request parameter is used to pass the issuer_state value back to the Credential Issuer.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetIssuerState(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.IssuerState);

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

        public static string GetClientMetadataUri(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.ClientMetadataUri);

        public static Client GetClientMetadata(this JsonObject jObj)
        {
            var clientMetadata = jObj.GetStr(AuthorizationRequestParameters.ClientMetadata);
            if (clientMetadata == null) return null;
            return JsonSerializer.Deserialize<Client>(clientMetadata);
        }

        /// <summary>
        /// This parameter can be used to bind the issued authorization code to a specific key.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetDPOPJktFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.DPOPJkt);

        public static IEnumerable<string> GetScopesFromAuthorizationRequest(this JsonObject jObj)
        {
            var scope = jObj.GetStr(AuthorizationRequestParameters.Scope);
            if (string.IsNullOrWhiteSpace(scope))
            {
                return new string[0];
            }

            return scope.Split(' ');
        }

        public static ICollection<AuthorizationData> GetAuthorizationDetailsFromAuthorizationRequest(this JsonObject jObj)
        {
            var authDetails = jObj.GetToken(AuthorizationRequestParameters.AuthorizationDetails);
            return authDetails == null ? new List<AuthorizationData>() : JsonSerializerExtensions.DeserializeAuthorizationDetails(authDetails);
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

        public static string GetResponseUriFromAuthorizationRequest(this JsonObject jObj) => jObj.GetStr(AuthorizationRequestParameters.ResponseUri);

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
            var claimsValue = jObj[AuthorizationRequestParameters.Claims] as JsonValue;
            if (claimsJObj == null && claimsValue != null)
                claimsJObj = JsonObject.Parse(claimsValue.GetValue<string>()).AsObject();

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

        #region Authorization request callback

        /// <summary>
        /// Get the id_token from the authorization request callback request.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetIdTokenFromAuthorizationRequestCallback(this JsonObject jObj) => jObj.GetStr(AuthorizationResponseParameters.IdToken);

        #endregion

        #region Token request

        public static string GetPreAuthorizedCode(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.PreAuthorizedCode);

        public static string GetTransactionCode(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.TransactionCode);

        public static string GetTicket(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.Ticket);

        public static string GetClaimToken(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClaimToken);

        public static string GetClaimTokenFormat(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClaimTokenFormat);

        public static string GetPct(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.Pct);

        public static string GetRpt(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.Rpt);

        public static string GetGrantType(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.GrantType);

        public static string GetClientAssertion(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClientAssertion);

        public static string GetClientAssertionType(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClientAssertionType);

        public static string GetClientId(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClientId);

        public static string GetClientSecret(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ClientSecret);

        public static string GetRefreshToken(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.RefreshToken);

        public static string GetAuthorizationCode(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.Code);

        public static string GetCodeVerifier(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.CodeVerifier);

        public static string GetRedirectUri(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.RedirectUri);

        public static string GetDeviceCode(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.DeviceCode);

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

        #region Token Exchange

        /// <summary>
        /// A security token that represents the identify of the party on behalf of whom the request is being made.
        /// Typically, the subject of this token will be the subject of the security token issued in the response to this request.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetSubjectToken(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.SubjectToken);

        /// <summary>
        /// An identifier, that indicates the type of the security token in the subject_token parameter.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetSubjectTokenType(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.SubjectTokenType);

        /// <summary>
        /// An identifier, for the type of the requested security token.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetRequestedTokenType(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.RequestedTokenType);

        /// <summary>
        /// A security token that represents the identity of the acting party.
        /// It will be the party that is authorized to use the requested security token and act on behalf of the subject.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetActorToken(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ActorToken);

        /// <summary>
        /// An identifier, that indicates the type of the security token in the actor_token parameter.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static string GetActorTokenType(this JsonObject jObj) => jObj.GetStr(TokenRequestParameters.ActorTokenType);

        /// <summary>
        /// URI that indicates that target service or resource where the client intends to use the requested security token.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetResources(this JsonObject jObj) => jObj.GetArray(TokenRequestParameters.Resource);

        /// <summary>
        /// Logical name of the target service where the client intends to use the requested security token.
        /// Client identifier is example of thing that might be used as audience parameter.
        /// </summary>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAudiences(this JsonObject jObj) => jObj.GetArray(TokenRequestParameters.Audience);

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
            return keys.Select(k => new JsonWebKey(k.ToJsonString()));
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

        public static string GetAuthReqId(this JsonObject jObj) => jObj.GetStr(BCAuthenticationResponseParameters.AuthReqId);

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
