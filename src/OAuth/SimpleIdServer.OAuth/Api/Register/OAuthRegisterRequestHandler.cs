// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.Authenticate.Handlers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register
{
    public interface IRegisterRequestHandler
    {
        Task<JObject> Handle(HandlerContext handlerContext, CancellationToken token);
    }

    /// <summary>
    /// https://tools.ietf.org/html/rfc7591
    /// </summary>
    public class OAuthRegisterRequestHandler : IRegisterRequestHandler
    {
        private readonly IEnumerable<IGrantTypeHandler> _grantTypeHandlers;
        private readonly IEnumerable<IResponseTypeHandler> _responseTypeHandlers;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _oauthClientAuthenticationHandlers;
        private readonly IOAuthClientQueryRepository _oauthClientQueryRepository;
        private readonly IOAuthClientCommandRepository _oauthClientCommandRepository;
        private readonly IOAuthScopeQueryRepository _oauthScopeRepository;
        private readonly IJwtParser _jwtParser;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEnumerable<ISignHandler> _signHandlers;
        private readonly IEnumerable<ICEKHandler> _cekHandlers;
        private readonly IEnumerable<IEncHandler> _encHandlers;

        public OAuthRegisterRequestHandler(IEnumerable<IGrantTypeHandler> grantTypeHandlers, IEnumerable<IResponseTypeHandler> responseTypeHandlers,
            IEnumerable<IOAuthClientAuthenticationHandler> oauthClientAuthenticationHandlers, IOAuthClientQueryRepository oauthClientQueryRepository,
            IOAuthClientCommandRepository oAuthClientCommandRepository, IOAuthScopeQueryRepository oauthScopeRepository, IJwtParser jwtParser, IHttpClientFactory httpClientFactory,
            IEnumerable<ISignHandler> signHandlers, IEnumerable<ICEKHandler> cekHandlers, IEnumerable<IEncHandler> encHandlers, IOptions<OAuthHostOptions> oauthHostOptions)
        {
            _grantTypeHandlers = grantTypeHandlers;
            _responseTypeHandlers = responseTypeHandlers;
            _oauthClientAuthenticationHandlers = oauthClientAuthenticationHandlers;
            _oauthClientQueryRepository = oauthClientQueryRepository;
            _oauthClientCommandRepository = oAuthClientCommandRepository;
            _oauthScopeRepository = oauthScopeRepository;
            _jwtParser = jwtParser;
            _httpClientFactory = httpClientFactory;
            _signHandlers = signHandlers;
            _cekHandlers = cekHandlers;
            _encHandlers = encHandlers;
            OauthHostOptions = oauthHostOptions.Value;
        }

        protected readonly OAuthHostOptions OauthHostOptions;
        protected IOAuthClientQueryRepository OAuthClientQueryRepository => _oauthClientQueryRepository;
        protected IOAuthClientCommandRepository OAuthClientCommandRepository => _oauthClientCommandRepository;
        protected IEnumerable<ISignHandler> SignHandlers => _signHandlers;
        protected IEnumerable<ICEKHandler> CEKHandlers => _cekHandlers;
        protected IEnumerable<IEncHandler> ENCHandlers => _encHandlers;

        public async Task<JObject> Handle(HandlerContext handlerContext, CancellationToken token)
        {
            await ExtractSoftwareStatement(handlerContext.Request.Data);
            await Check(handlerContext, token);
            return await Create(handlerContext, token);
        }

        protected virtual async Task Check(HandlerContext context, CancellationToken token)
        {
            var jObj = context.Request.Data;
            var grantTypes = GetDefaultGrantTypes(jObj);
            var scopes = GetDefaultScopes(jObj);
            var tokenEndpointAuthMethod = GetDefaultTokenAuthMethod(jObj);
            var responseTypes = GetDefaultResponseTypes(jObj);
            var redirectUris = jObj.GetRedirectUrisFromRegisterRequest();
            var clientNames = jObj.GetClientNamesFromRegisterRequest();
            var clientUris = jObj.GetClientUrisFromRegisterRequest();
            var logoUris = jObj.GetLogoUrisFromRegisterRequest();
            var contacts = jObj.GetContactsFromRegisterRequest();
            var tosUris = jObj.GetTosUrisFromRegisterRequest();
            var policyUris = jObj.GetPolicyUrisFromRegisterRequest();
            var jwksUri = jObj.GetJwksUriFromRegisterRequest();
            var jwks = jObj.GetJwksFromRegisterRequest();
            var softwareStatement = jObj.GetSoftwareStatementFromRegisterRequest();
            var softwareId = jObj.GetSoftwareIdFromRegisterRequest();
            var softwareVersion = jObj.GetSoftwareVersionFromRegisterRequest();
            var authGrantTypes = _responseTypeHandlers.Select(r => r.GrantType);
            var supportedGrantTypes = _grantTypeHandlers.Select(g => g.GrantType).Union(authGrantTypes).Distinct();
            var notSupportedGrantTypes = grantTypes.Where(gt => !supportedGrantTypes.Any(sgt => sgt == gt));
            var tokenSignedResponseAlg = jObj.GetTokenSignedResponseAlgFromRegisterRequest();
            var tokenEncryptedResponseAlg = jObj.GetTokenEncryptedResponseAlgFromRegisterRequest();
            var tokenEncryptedResponseEnc = jObj.GetTokenEncryptedResponseEncFromRegisterRequest();
            if (notSupportedGrantTypes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.UNSUPPORTED_GRANT_TYPES, string.Join(",", notSupportedGrantTypes)));
            }

            if (!string.IsNullOrWhiteSpace(tokenEndpointAuthMethod) && !_oauthClientAuthenticationHandlers.Any(o => o.AuthMethod == tokenEndpointAuthMethod))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.UNKNOWN_AUTH_METHOD, tokenEndpointAuthMethod));
            }

            var supportedResponseTypeHandlers = _responseTypeHandlers.Where(r => grantTypes.Contains(r.GrantType));
            if (supportedResponseTypeHandlers.Any())
            {
                var supportedResponseTypes = supportedResponseTypeHandlers.Select(s => s.ResponseType);
                var unSupportedResponseTypes = responseTypes.Where(r => !supportedResponseTypes.Contains(r));
                if (unSupportedResponseTypes.Any())
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.BAD_RESPONSE_TYPES, string.Join(",", unSupportedResponseTypes)));
                }
            }

            foreach (var kvp in supportedResponseTypeHandlers.GroupBy(k => k.GrantType))
            {
                if (!kvp.Any(k => responseTypes.Contains(k.ResponseType)))
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.MISSING_RESPONSE_TYPE, kvp.Key));
                }
            }

            if (supportedResponseTypeHandlers.Any())
            {
                if (redirectUris == null || !redirectUris.Any())
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.MISSING_PARAMETER, RegisterRequestParameters.RedirectUris));
                }

                foreach (var redirectUrl in redirectUris)
                {
                    CheckRedirectUrl(context, redirectUrl);
                }
            }

            var existingScopeNames = (await _oauthScopeRepository.FindOAuthScopesByNames(scopes, token)).Select(s => s.Name);
            var unsupportedScopes = scopes.Except(existingScopeNames);
            if (unsupportedScopes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.UNSUPPORTED_SCOPES, string.Join(",", unsupportedScopes)));
            }

            CheckUri(jwksUri, ErrorMessages.BAD_JWKS_URI);
            CheckUris(clientUris, ErrorMessages.BAD_CLIENT_URI);
            CheckUris(logoUris, ErrorMessages.BAD_LOGO_URI);
            CheckUris(tosUris, ErrorMessages.BAD_TOS_URI);
            CheckUris(policyUris, ErrorMessages.BAD_POLICY_URI);
            CheckSignature(tokenSignedResponseAlg, ErrorMessages.UNSUPPORTED_TOKEN_SIGNED_RESPONSE_ALG);
            CheckEncryption(tokenEncryptedResponseAlg, tokenEncryptedResponseEnc, ErrorMessages.UNSUPPORTED_TOKEN_ENCRYPTED_RESPONSE_ALG, ErrorMessages.UNSUPPORTED_TOKEN_ENCRYPTED_RESPONSE_ENC, RegisterRequestParameters.TokenEncryptedResponseAlg);
            if (!string.IsNullOrWhiteSpace(jwksUri) && jwks != null)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.DUPLICATE_JWKS);
            }
        }

        protected virtual async Task<JObject> Create(HandlerContext context, CancellationToken token)
        {
            var oauthClient = new OAuthClient();
            var jObj = await EnrichOAuthClient(context, oauthClient, token);
            _oauthClientCommandRepository.Add(oauthClient);
            await _oauthClientCommandRepository.SaveChanges(token);
            return jObj;
        }

        protected async Task<JObject> EnrichOAuthClient(HandlerContext context, OAuthClient oauthClient, CancellationToken token)
        {
            var jObj = context.Request.Data;
            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();
            var grantTypes = GetDefaultGrantTypes(jObj);
            var scopes = GetDefaultScopes(jObj);
            var tokenEndpointAuthMethod = GetDefaultTokenAuthMethod(jObj);
            var responseTypes = GetDefaultResponseTypes(jObj);
            var redirectUris = jObj.GetRedirectUrisFromRegisterRequest();
            var clientNames = jObj.GetClientNamesFromRegisterRequest();
            var clientUris = jObj.GetClientUrisFromRegisterRequest();
            var logoUris = jObj.GetLogoUrisFromRegisterRequest();
            var contacts = jObj.GetContactsFromRegisterRequest();
            var tosUris = jObj.GetTosUrisFromRegisterRequest();
            var policyUris = jObj.GetPolicyUrisFromRegisterRequest();
            var jwksUri = jObj.GetJwksUriFromRegisterRequest();
            var jwks = jObj.GetJwksFromRegisterRequest();
            var softwareStatement = jObj.GetSoftwareStatementFromRegisterRequest();
            var softwareId = jObj.GetSoftwareIdFromRegisterRequest();
            var softwareVersion = jObj.GetSoftwareVersionFromRegisterRequest();
            var tokenSignedResponseAlg = jObj.GetTokenSignedResponseAlgFromRegisterRequest();
            var tokenEncryptedResponseAlg = jObj.GetTokenEncryptedResponseAlgFromRegisterRequest();
            var tokenEncryptedResponseEnc = jObj.GetTokenEncryptedResponseEncFromRegisterRequest();
            if (!clientNames.Any(c => string.IsNullOrWhiteSpace(c.Key)))
            {
                clientNames.Add(string.Empty, clientId);
            }

            if (string.IsNullOrWhiteSpace(tokenSignedResponseAlg))
            {
                tokenSignedResponseAlg = RSA256SignHandler.ALG_NAME;
            }

            if (!string.IsNullOrWhiteSpace(tokenEncryptedResponseAlg) && string.IsNullOrWhiteSpace(tokenEncryptedResponseEnc))
            {
                tokenEncryptedResponseEnc = A128CBCHS256EncHandler.ENC_NAME;
            }

            var supportedScopes = await _oauthScopeRepository.FindOAuthScopesByNames(scopes, token);
            oauthClient.ClientId = clientId;
            oauthClient.TokenEndPointAuthMethod = tokenEndpointAuthMethod;
            oauthClient.GrantTypes = grantTypes.ToList();
            oauthClient.ResponseTypes = responseTypes;
            oauthClient.Contacts = contacts == null ? new List<string>() : contacts.ToList();
            oauthClient.JwksUri = jwksUri;
            oauthClient.JsonWebKeys = jwks == null ? new List<SimpleIdServer.Jwt.JsonWebKey>() : jwks.ToList();
            oauthClient.SoftwareId = softwareId;
            oauthClient.SoftwareVersion = softwareVersion;
            oauthClient.RedirectionUrls = redirectUris.ToList();
            oauthClient.PreferredTokenProfile = OauthHostOptions.DefaultTokenProfile;
            oauthClient.AllowedScopes = supportedScopes.ToList();
            oauthClient.TokenSignedResponseAlg = tokenSignedResponseAlg;
            oauthClient.TokenEncryptedResponseAlg = tokenEncryptedResponseAlg;
            oauthClient.TokenEncryptedResponseEnc = tokenEncryptedResponseEnc;
            oauthClient.RefreshTokenExpirationTimeInSeconds = 60 * 30;
            oauthClient.TokenExpirationTimeInSeconds = 60 * 30;
            foreach (var kvp in clientNames)
            {
                oauthClient.AddClientName(kvp.Key, kvp.Value);
            }

            foreach (var kvp in clientUris)
            {
                oauthClient.AddClientUri(kvp.Key, kvp.Value);
            }

            foreach (var kvp in logoUris)
            {
                oauthClient.AddLogoUri(kvp.Key, kvp.Value);
            }

            foreach (var kvp in tosUris)
            {
                oauthClient.AddTosUri(kvp.Key, kvp.Value);
            }

            foreach (var kvp in policyUris)
            {
                oauthClient.AddPolicyUri(kvp.Key, kvp.Value);
            }

            oauthClient.AddSharedSecret(clientSecret);
            var currentDateTime = DateTime.UtcNow;
            var result = new JObject();
            AddNotEmpty(result, RegisterResponseParameters.ClientId, clientId);
            AddNotEmpty(result, RegisterResponseParameters.ClientSecret, clientSecret);
            AddNotEmpty(result, RegisterResponseParameters.ClientIdIssuedAt, currentDateTime.ConvertToUnixTimestamp().ToString());
            if (OauthHostOptions.ClientSecretExpirationInSeconds != null)
            {
                AddNotEmpty(result, RegisterResponseParameters.ClientSecretExpiresAt, currentDateTime.AddSeconds(OauthHostOptions.ClientSecretExpirationInSeconds.Value).ConvertToUnixTimestamp().ToString());
            }

            AddNotEmpty(result, RegisterRequestParameters.GrantTypes, grantTypes);
            AddNotEmpty(result, RegisterRequestParameters.RedirectUris, redirectUris);
            AddNotEmpty(result, RegisterRequestParameters.TokenEndpointAuthMethod, tokenEndpointAuthMethod);
            AddNotEmpty(result, RegisterRequestParameters.ResponseTypes, responseTypes);
            AddNotEmpty(result, RegisterRequestParameters.ClientName, clientNames);
            AddNotEmpty(result, RegisterRequestParameters.ClientUri, clientUris);
            AddNotEmpty(result, RegisterRequestParameters.LogoUri, logoUris);
            if (scopes.Any())
            {
                AddNotEmpty(result, RegisterRequestParameters.Scope, string.Join(" ", scopes));
            }

            AddNotEmpty(result, RegisterRequestParameters.Contacts, contacts);
            AddNotEmpty(result, RegisterRequestParameters.TosUri, tosUris);
            AddNotEmpty(result, RegisterRequestParameters.PolicyUri, policyUris);
            AddNotEmpty(result, RegisterRequestParameters.JwksUri, jwksUri);
            AddNotEmpty(result, RegisterRequestParameters.Jwks, jwks);
            AddNotEmpty(result, RegisterRequestParameters.SoftwareId, softwareId);
            AddNotEmpty(result, RegisterRequestParameters.SoftwareVersion, softwareVersion);
            AddNotEmpty(result, RegisterRequestParameters.SoftwareStatement, softwareStatement);
            return result;
        }

        private async Task ExtractSoftwareStatement(JObject jObj)
        {
            var softwareStatement = jObj.GetSoftwareStatementFromRegisterRequest();
            if (string.IsNullOrWhiteSpace(softwareStatement))
            {
                return;
            }

            if (!_jwtParser.IsJwsToken(softwareStatement))
            {
                throw new OAuthException(ErrorCodes.INVALID_SOFTWARE_STATEMENT, ErrorMessages.BAD_JWS_SOFTWARE_STATEMENT);
            }

            SimpleIdServer.Jwt.Jws.JwsPayload jwsPayload;
            SimpleIdServer.Jwt.Jws.JwsHeader header;
            try
            {
                jwsPayload = _jwtParser.ExtractJwsPayload(softwareStatement);
                header = _jwtParser.ExtractJwsHeader(softwareStatement);
                if (jwsPayload == null)
                {
                    throw new OAuthException(ErrorCodes.INVALID_SOFTWARE_STATEMENT, ErrorMessages.BAD_JWS_SOFTWARE_STATEMENT);
                }
            }
            catch
            {
                throw new OAuthException(ErrorCodes.INVALID_SOFTWARE_STATEMENT, ErrorMessages.BAD_JWS_SOFTWARE_STATEMENT);
            }

            var issuer = jwsPayload.GetIssuer();
            var trustedParty = OauthHostOptions.SoftwareStatementTrustedParties.FirstOrDefault(s => s.Iss == issuer);
            if (trustedParty == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_SOFTWARE_STATEMENT, ErrorMessages.BAD_ISSUER_SOFTWARE_STATEMENT);
            }

            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                var json = await httpClient.GetStringAsync(trustedParty.JwksUrl);
                var keysJson = JObject.Parse(json)["keys"].ToString();
                var jsonWebKeys = JsonConvert.DeserializeObject<JArray>(keysJson).Select(k => SimpleIdServer.Jwt.JsonWebKey.Deserialize(k.ToString()));
                var jsonWebKey = jsonWebKeys.FirstOrDefault(j => j.Kid == header.Kid);
                jwsPayload = _jwtParser.Unsign(softwareStatement, jsonWebKey);
                if (jwsPayload == null)
                {
                    throw new OAuthException(ErrorCodes.INVALID_SOFTWARE_STATEMENT, ErrorMessages.BAD_SOFTWARE_STATEMENT_SIGNATURE);
                }

                foreach (var kvp in jwsPayload)
                {
                    if (jObj.ContainsKey(kvp.Key))
                    {
                        jObj.Remove(kvp.Key);
                    }

                    jObj.Add(kvp.Key, JToken.FromObject(kvp.Value));
                }
            }
        }

        protected static IEnumerable<string> GetDefaultGrantTypes(JObject jObj)
        {
            var grantTypes = jObj.GetGrantTypesFromRegisterRequest();
            if (grantTypes == null || !grantTypes.Any())
            {
                grantTypes = new List<string>
                {
                    AuthorizationCodeHandler.GRANT_TYPE
                };
            }

            return grantTypes;
        }

        protected static string GetDefaultTokenAuthMethod(JObject jObj)
        {
            var tokenEndpointAuthMethod = jObj.GetTokenEndpointAuthMethodFromRegisterRequest();
            if (string.IsNullOrWhiteSpace(tokenEndpointAuthMethod))
            {
                tokenEndpointAuthMethod = OAuthClientSecretBasicAuthenticationHandler.AUTH_METHOD;
            }

            return tokenEndpointAuthMethod;
        }

        protected static IEnumerable<string> GetDefaultResponseTypes(JObject jObj)
        {
            var responseTypes = jObj.GetResponseTypesFromRegisterRequest();
            if (responseTypes == null || !responseTypes.Any())
            {
                responseTypes = new List<string>
                {
                    AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE
                };
            }

            return responseTypes;
        }

        private IEnumerable<string> GetDefaultScopes(JObject jObj)
        {
            var scopes = jObj.GetScopesFromRegisterRequest();
            if (!scopes.Any())
            {
                scopes = OauthHostOptions.DefaultScopes;
            }

            return scopes;
        }

        protected virtual void CheckRedirectUrl(HandlerContext context, string redirectUrl)
        {
            if (!Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute))
            {
                throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, string.Format(ErrorMessages.BAD_REDIRECT_URI, redirectUrl));
            }
        }

        protected static void CheckUri(string url, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(errorMessage, url));
            }
        }

        protected static void CheckUris(IDictionary<string, string> dic, string errorMessage)
        {
            if (dic == null || !dic.Any())
            {
                return;
            }

            foreach (var kvp in dic)
            {
                CheckUri(kvp.Value, errorMessage);
            }
        }

        protected static void AddNotEmpty(JObject jObj, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                jObj.Add(name, value);
            }
        }

        protected static void AddNotEmpty(JObject jObj, string name, IEnumerable<string> values)
        {
            if (values != null && values.Any())
            {
                jObj.Add(name, JArray.FromObject(values));
            }
        }

        protected static void AddNotEmpty(JObject jObj, string name, Dictionary<string, string> dic)
        {
            if (dic != null && dic.Any())
            {
                foreach (var kvp in dic)
                {
                    if (string.IsNullOrWhiteSpace(kvp.Key))
                    {
                        jObj.Add(name, kvp.Value);
                    }
                    else
                    {
                        jObj.Add($"{name}#{kvp.Key}", kvp.Value);
                    }
                }
            }
        }

        protected static void AddNotEmpty(JObject jObj, string name, IEnumerable<SimpleIdServer.Jwt.JsonWebKey> jwks)
        {
            if (jwks != null && jwks.Any())
            {
                var result = new JObject
                {
                    { "keys", JArray.FromObject(jwks.Select(_ => _.Serialize())) }
                };
                jObj.Add(name, result);
            }
        }

        protected void CheckSignature(string alg, string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(alg) && !_signHandlers.Any(s => s.AlgName == alg))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, errorMessage);
            }
        }

        protected void CheckEncryption(string alg, string enc, string unsupportedAlgMsg, string unsupportedEncMsg, string parameterName)
        {
            if (!string.IsNullOrWhiteSpace(alg) && !_cekHandlers.Any(c => c.AlgName == alg))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, unsupportedAlgMsg);
            }

            if (!string.IsNullOrWhiteSpace(enc) && !_encHandlers.Any(c => c.EncName == enc))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, unsupportedEncMsg);
            }

            if (!string.IsNullOrWhiteSpace(enc) && string.IsNullOrWhiteSpace(alg))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, parameterName));
            }
        }
    }
}