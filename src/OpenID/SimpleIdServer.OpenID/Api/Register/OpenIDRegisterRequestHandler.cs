// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Register;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OAuth.Domains.Scopes;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Register
{
    /// <summary>
    /// https://openid.net/specs/openid-connect-registration-1_0.html
    /// </summary>
    public class OpenIDRegisterRequestHandler : OAuthRegisterRequestHandler
    {
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
        private readonly OpenIDHostOptions _openIDHostOptions;

        public OpenIDRegisterRequestHandler(IEnumerable<IGrantTypeHandler> grantTypeHandlers, IEnumerable<IResponseTypeHandler> responseTypeHandlers,
            IEnumerable<IOAuthClientAuthenticationHandler> oauthClientAuthenticationHandlers, IOAuthClientQueryRepository oauthClientQueryRepository,
            IOAuthClientCommandRepository oAuthClientCommandRepository, IOAuthScopeQueryRepository oauthScopeRepository, IJwtParser jwtParser, IHttpClientFactory httpClientFactory, IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders, IEnumerable<ISignHandler> signHandlers,
            IEnumerable<ICEKHandler> cekHandlers, IEnumerable<IEncHandler> encHandlers, IOptions<OAuthHostOptions> oauthHostOptions, IOptions<OpenIDHostOptions> openidHostOptions) 
            : base(grantTypeHandlers, responseTypeHandlers, oauthClientAuthenticationHandlers, oauthClientQueryRepository, oAuthClientCommandRepository, oauthScopeRepository, jwtParser, httpClientFactory, signHandlers, cekHandlers, encHandlers, oauthHostOptions)
        {
            _subjectTypeBuilders = subjectTypeBuilders;
            _openIDHostOptions = openidHostOptions.Value;
        }

        protected override Task Check(HandlerContext handlerContext)
        {
            var jObj = handlerContext.Request.HttpBody;
            var applicationType = jObj.GetApplicationTypeFromRegisterRequest();
            var sectorIdentifierUri = jObj.GetSectorIdentifierUriFromRegisterRequest();
            var subjectType = jObj.GetSubjectTypeFromRegisterRequest();
            var idTokenSignedResponseAlg = jObj.GetIdTokenSignedResponseAlgFromRegisterRequest();
            var idTokenEncryptedResponseAlg = jObj.GetIdTokenEncryptedResponseAlgFromRegisterRequest();
            var idTokenEncryptedResponseEnc = jObj.GetIdTokenEncryptedResponseEncFromRegisterRequest();
            var userInfoSignedResponseAlg = jObj.GetUserInfoSignedResponseAlgFromRegisterRequest();
            var userInfoEncryptedResponseAlg = jObj.GetUserInfoEncryptedResponseAlgFromRegisterRequest();
            var userInfoEncryptedResponseEnc = jObj.GetUserInfoEncryptedResponseEncFromRegisterRequest();
            var requestObjectSigningAlg = jObj.GetRequestObjectSigningAlgFromRegisterRequest();
            var requestObjectEncryptionAlg  = jObj.GetRequestObjectEncryptionAlgFromRegisterRequest();
            var requestObjectEncryptionEnc  = jObj.GetRequestObjectEncryptionEncFromRegisterRequest();
            if (!string.IsNullOrWhiteSpace(applicationType) && applicationType != "web" && applicationType != "native")
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_APPLICATION_TYPE);
            }

            if (!string.IsNullOrWhiteSpace(sectorIdentifierUri))
            {
                if (!Uri.IsWellFormedUriString(sectorIdentifierUri, UriKind.Absolute))
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_SECTOR_IDENTIFIER_URI);
                }

                var uri = new Uri(sectorIdentifierUri);
                if (uri.Scheme != "https")
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_HTTPS_SECTOR_IDENTIFIER_URI);
                }
            }

            if (!string.IsNullOrWhiteSpace(subjectType) && !_subjectTypeBuilders.Any(s => s.SubjectType == subjectType))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_SUBJECT_TYPE);
            }

            CheckSignature(idTokenSignedResponseAlg, ErrorMessages.UNSUPPORTED_IDTOKEN_SIGNED_RESPONSE_ALG);
            CheckEncryption(idTokenEncryptedResponseAlg, idTokenEncryptedResponseEnc, ErrorMessages.UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ALG, ErrorMessages.UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ENC, RegisterRequestParameters.IdTokenEncryptedResponseAlg);
            CheckSignature(userInfoSignedResponseAlg, ErrorMessages.UNSUPPORTED_USERINFO_SIGNED_RESPONSE_ALG);
            CheckEncryption(userInfoEncryptedResponseAlg, userInfoEncryptedResponseEnc, ErrorMessages.UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ALG, ErrorMessages.UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ENC, RegisterRequestParameters.UserInfoEncryptedResponseAlg);
            CheckSignature(requestObjectSigningAlg, ErrorMessages.UNSUPPORTED_REQUEST_OBJECT_SIGNING_ALG);
            CheckEncryption(requestObjectEncryptionAlg, requestObjectEncryptionEnc, ErrorMessages.UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ALG, ErrorMessages.UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ENC, RegisterRequestParameters.RequestObjectEncryptionAlg);
            return base.Check(handlerContext);
        }

        protected override async Task<JObject> Create(HandlerContext context)
        {
            var jObj = context.Request.HttpBody;
            var applicationType = GetDefaultApplicationType(jObj);
            var sectorIdentifierUri = jObj.GetSectorIdentifierUriFromRegisterRequest();
            var subjectType = jObj.GetSubjectTypeFromRegisterRequest();
            var idTokenSignedResponseAlg = jObj.GetIdTokenSignedResponseAlgFromRegisterRequest();
            var idTokenEncryptedResponseAlg = jObj.GetIdTokenEncryptedResponseAlgFromRegisterRequest();
            var idTokenEncryptedResponseEnc = jObj.GetIdTokenEncryptedResponseEncFromRegisterRequest();
            var userInfoSignedResponseAlg = jObj.GetUserInfoSignedResponseAlgFromRegisterRequest();
            var userInfoEncryptedResponseAlg = jObj.GetUserInfoEncryptedResponseAlgFromRegisterRequest();
            var userInfoEncryptedResponseEnc = jObj.GetUserInfoEncryptedResponseEncFromRegisterRequest();
            var requestObjectSigningAlg = jObj.GetRequestObjectSigningAlgFromRegisterRequest();
            var requestObjectEncryptionAlg = jObj.GetRequestObjectEncryptionAlgFromRegisterRequest();
            var requestObjectEncryptionEnc = jObj.GetRequestObjectEncryptionEncFromRegisterRequest();
            var defaultMaxAge = jObj.GetDefaultMaxAgeFromRegisterRequest();
            var requireAuthTime = jObj.GetRequireAuhTimeFromRegisterRequest();
            var acrValues = jObj.GetDefaultAcrValuesFromRegisterRequest();
            if (string.IsNullOrWhiteSpace(subjectType))
            {
                subjectType = _openIDHostOptions.DefaultSubjectType;
            }

            if (string.IsNullOrWhiteSpace(idTokenSignedResponseAlg))
            {
                idTokenSignedResponseAlg = RSA256SignHandler.ALG_NAME;
            }

            if (defaultMaxAge == null)
            {
                defaultMaxAge = _openIDHostOptions.DefaultMaxAge;
            }

            if (requireAuthTime == null)
            {
                requireAuthTime = false;
            }

            if (!string.IsNullOrWhiteSpace(idTokenEncryptedResponseAlg) && string.IsNullOrWhiteSpace(idTokenEncryptedResponseEnc))
            {
                idTokenEncryptedResponseEnc = A128CBCHS256EncHandler.ENC_NAME;
            }

            if (!string.IsNullOrWhiteSpace(userInfoEncryptedResponseAlg) && string.IsNullOrWhiteSpace(userInfoEncryptedResponseEnc))
            {
                userInfoEncryptedResponseEnc = A128CBCHS256EncHandler.ENC_NAME;
            }

            if (!string.IsNullOrWhiteSpace(requestObjectEncryptionAlg) && string.IsNullOrWhiteSpace(requestObjectEncryptionEnc))
            {
                requestObjectEncryptionEnc = A128CBCHS256EncHandler.ENC_NAME;
            }

            var kvp = await BuildOAuthClient(context);
            kvp.Key.ApplicationType = applicationType;
            kvp.Key.SectorIdentifierUri = sectorIdentifierUri;
            kvp.Key.SubjectType = subjectType;
            kvp.Key.IdTokenSignedResponseAlg = idTokenSignedResponseAlg;
            kvp.Key.IdTokenEncryptedResponseAlg = idTokenEncryptedResponseAlg;
            kvp.Key.IdTokenEncryptedResponseEnc = idTokenEncryptedResponseEnc;
            kvp.Key.UserInfoSignedResponseAlg = userInfoSignedResponseAlg;
            kvp.Key.UserInfoEncryptedResponseAlg = userInfoEncryptedResponseAlg;
            kvp.Key.UserInfoEncryptedResponseEnc = userInfoEncryptedResponseEnc;
            kvp.Key.RequestObjectSigningAlg = requestObjectSigningAlg;
            kvp.Key.RequestObjectEncryptionAlg = requestObjectEncryptionAlg;
            kvp.Key.RequestObjectEncryptionEnc = requestObjectEncryptionEnc;
            kvp.Key.DefaultMaxAge = defaultMaxAge;
            kvp.Key.RequireAuthTime = requireAuthTime.Value;
            kvp.Key.DefaultAcrValues = acrValues.ToList();
            OAuthClientCommandRepository.Add(kvp.Key);
            await OAuthClientCommandRepository.SaveChanges();
            AddNotEmpty(kvp.Value, RegisterRequestParameters.ApplicationType, applicationType);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.SectorIdentifierUri, sectorIdentifierUri);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.SubjectType, subjectType);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.IdTokenSignedResponseAlg, idTokenSignedResponseAlg);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.IdTokenEncryptedResponseAlg, idTokenEncryptedResponseAlg);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.IdTokenEncryptedResponseEnc, idTokenEncryptedResponseEnc);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.UserInfoSignedResponseAlg, userInfoSignedResponseAlg);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.UserInfoEncryptedResponseAlg, userInfoEncryptedResponseAlg);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.UserInfoEncryptedResponseEnc, userInfoEncryptedResponseEnc);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.RequestObjectSigningAlg, requestObjectSigningAlg);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.RequestObjectEncryptionAlg, requestObjectEncryptionAlg);
            AddNotEmpty(kvp.Value, RegisterRequestParameters.RequestObjectEncryptionEnc, requestObjectEncryptionEnc);
            if (defaultMaxAge != null)
            {
                AddNotEmpty(kvp.Value, RegisterRequestParameters.DefaultMaxAge, defaultMaxAge.Value.ToString().ToLowerInvariant());
            }

            AddNotEmpty(kvp.Value, RegisterRequestParameters.RequireAuthTime, requireAuthTime.Value.ToString().ToLowerInvariant());
            AddNotEmpty(kvp.Value, RegisterRequestParameters.DefaultAcrValues, acrValues);
            return kvp.Value;
        }

        protected override void CheckRedirectUrl(HandlerContext context, string redirectUrl)
        {
            var jObj = context.Request.HttpBody;
            var applicationType = GetDefaultApplicationType(jObj);
            if (!Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute))
            {
                throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, string.Format(OAuth.ErrorMessages.BAD_REDIRECT_URI, redirectUrl));
            }

            var uri = new Uri(redirectUrl);
            if (applicationType == "web")
            {
                if (uri.Scheme.ToLowerInvariant() != "https")
                {
                    throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, ErrorMessages.INVALID_HTTPS_REDIRECT_URI);
                }

                if (uri.Host.ToLowerInvariant() == "localhost")
                {
                    throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, ErrorMessages.INVALID_LOCALHOST_REDIRECT_URI);
                }
            }
        }

        private static string GetDefaultApplicationType(JObject jObj)
        {
            var applicationType = jObj.GetApplicationTypeFromRegisterRequest();
            if (string.IsNullOrWhiteSpace(applicationType))
            {
                applicationType = "web";
            }

            return applicationType;
        }
    }
}
