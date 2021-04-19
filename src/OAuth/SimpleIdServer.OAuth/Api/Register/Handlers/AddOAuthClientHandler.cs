// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Register.Validators;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate.Handlers;
using SimpleIdServer.OAuth.Domains;
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

namespace SimpleIdServer.OAuth.Api.Register.Handlers
{
    public interface IAddOAuthClientHandler
    {
        Task<JObject> Handle(HandlerContext handlerContext, CancellationToken token);
    }

    /// <summary>
    /// https://tools.ietf.org/html/rfc7591
    /// </summary>
    public class AddOAuthClientHandler : IAddOAuthClientHandler
    {
        private readonly IJwtParser _jwtParser;
        private readonly IOAuthClientValidator _oauthClientValidator;
        private readonly IHttpClientFactory _httpClientFactory;

        public AddOAuthClientHandler(
            IOAuthClientQueryRepository oauthClientQueryRepository,
            IOAuthClientCommandRepository oAuthClientCommandRepository,
            IJwtParser jwtParser,
            IHttpClientFactory httpClientFactory,
            IOAuthClientValidator oauthClientValidator,
            IOptions<OAuthHostOptions> oauthHostOptions)
        {
            OAuthClientQueryRepository = oauthClientQueryRepository;
            OAuthClientCommandRepository = oAuthClientCommandRepository;
            _jwtParser = jwtParser;
            _httpClientFactory = httpClientFactory;
            _oauthClientValidator = oauthClientValidator;
            OauthHostOptions = oauthHostOptions.Value;
        }

        protected readonly OAuthHostOptions OauthHostOptions;
        protected IOAuthClientQueryRepository OAuthClientQueryRepository { get; private set; }
        protected IOAuthClientCommandRepository OAuthClientCommandRepository { get; private set; }

        public async Task<JObject> Handle(HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            await ExtractSoftwareStatement(handlerContext.Request.Data);
            var oauthClient = ExtractClient(handlerContext);
            await _oauthClientValidator.Validate(oauthClient, cancellationToken);
            OAuthClientCommandRepository.Add(oauthClient);
            await OAuthClientCommandRepository.SaveChanges(cancellationToken);
            return BuildResponse(oauthClient, handlerContext.Request.IssuerName);
        }

        protected virtual OAuthClient ExtractClient(HandlerContext handlerContext)
        {
            var result = handlerContext.Request.Data.ToDomain();
            EnrichClient(result);
            return result;
        }

        protected void EnrichClient(OAuthClient oauthClient)
        {
            string clientId = Guid.NewGuid().ToString(),
                registrationAccessToken = Guid.NewGuid().ToString(),
                clientSecret = Guid.NewGuid().ToString();
            var currentDateTime = DateTime.UtcNow;
            DateTime? expirationDateTime = null;
            if (OauthHostOptions.ClientSecretExpirationInSeconds != null)
            {
                expirationDateTime = currentDateTime.AddSeconds(OauthHostOptions.ClientSecretExpirationInSeconds.Value);
            }

            oauthClient.ClientId = clientId;
            oauthClient.RegistrationAccessToken = registrationAccessToken;
            oauthClient.CreateDateTime = DateTime.UtcNow;
            oauthClient.UpdateDateTime = DateTime.UtcNow;
            oauthClient.RefreshTokenExpirationTimeInSeconds = 60 * 30;
            oauthClient.TokenExpirationTimeInSeconds = 60 * 30;
            oauthClient.PreferredTokenProfile = OauthHostOptions.DefaultTokenProfile;
            oauthClient.Secrets.Clear();
            oauthClient.AddSharedSecret(clientSecret, expirationDateTime);
            SetDefaultClientNames(oauthClient);
            SetDefaultGrantTypes(oauthClient);
            SetDefaultScopes(oauthClient);
            SetDefaultTokenAuthMethod(oauthClient);
            SetDefaultResponseTypes(oauthClient);
            SetDefaultTokenSignedResponseAlg(oauthClient);
            SetTokenEncryptedResponseEnc(oauthClient);
        }

        protected virtual JObject BuildResponse(OAuthClient oauthClient, string issuer)
        {
            return oauthClient.ToDto(issuer);
        }

        protected virtual void SetDefaultClientNames(OAuthClient oauthClient)
        {
            if (oauthClient.ClientNames != null && !oauthClient.ClientNames.Any())
            {
                oauthClient.AddClientName(null, oauthClient.ClientId);
            }
        }

        protected virtual void SetDefaultGrantTypes(OAuthClient oauthClient)
        {
            if (oauthClient.GrantTypes == null || !oauthClient.GrantTypes.Any())
            {
                oauthClient.GrantTypes = new List<string>
                {
                    AuthorizationCodeHandler.GRANT_TYPE
                };
            }
        }

        protected virtual void SetDefaultTokenAuthMethod(OAuthClient oauthClient)
        {
            if (string.IsNullOrWhiteSpace(oauthClient.TokenEndPointAuthMethod))
            {
                oauthClient.TokenEndPointAuthMethod = OAuthClientSecretBasicAuthenticationHandler.AUTH_METHOD;
            }
        }

        protected virtual void SetDefaultResponseTypes(OAuthClient oauthClient)
        {
            if (oauthClient.ResponseTypes == null || !oauthClient.ResponseTypes.Any())
            {
                oauthClient.ResponseTypes = new List<string>
                {
                    AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE
                };
            }
        }

        protected virtual void SetDefaultScopes(OAuthClient oauthClient)
        {
            if (!oauthClient.AllowedScopes.Any())
            {
                oauthClient.AllowedScopes = OauthHostOptions.DefaultScopes.Select(_ => new OAuthScope
                {
                    Name = _
                }).ToList();
            }
        }

        protected virtual void SetDefaultTokenSignedResponseAlg(OAuthClient oauthClient)
        {
            if (string.IsNullOrWhiteSpace(oauthClient.TokenSignedResponseAlg))
            {
                oauthClient.TokenSignedResponseAlg = OauthHostOptions.DefaultTokenSignedResponseAlg;
            }
        }

        protected virtual void SetTokenEncryptedResponseEnc(OAuthClient oauthClient)
        {
            if (!string.IsNullOrWhiteSpace(oauthClient.TokenEncryptedResponseAlg) && string.IsNullOrWhiteSpace(oauthClient.TokenEncryptedResponseEnc))
            {
                oauthClient.TokenEncryptedResponseEnc = A128CBCHS256EncHandler.ENC_NAME;
            }
        }

        private async Task ExtractSoftwareStatement(JObject jObj)
        {
            var softwareStatement = jObj.GetSoftwareStatement();
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
    }
}