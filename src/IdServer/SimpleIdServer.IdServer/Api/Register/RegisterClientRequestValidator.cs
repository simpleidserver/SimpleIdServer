// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Register
{
    public interface IRegisterClientRequestValidator
    {
        Task Validate(Client request, CancellationToken cancellationToken);
    }

    public class RegisterClientRequestValidator : IRegisterClientRequestValidator
    {
        private readonly IEnumerable<IResponseTypeHandler> _responseTypeHandlers;
        private readonly IEnumerable<IGrantTypeHandler> _grantTypeHandlers;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _oauthClientAuthenticationHandlers;
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
        private readonly IScopeRepository _scopeRepository;

        public RegisterClientRequestValidator(
            IEnumerable<IResponseTypeHandler> responseTypeHandlers, IEnumerable<IGrantTypeHandler> grantTypeHandlers, 
            IEnumerable<IOAuthClientAuthenticationHandler> oauthClientAuthenticationHandlers, IScopeRepository scopeRepository, IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders)
        {
            _responseTypeHandlers = responseTypeHandlers;
            _grantTypeHandlers = grantTypeHandlers;
            _oauthClientAuthenticationHandlers = oauthClientAuthenticationHandlers;
            _scopeRepository = scopeRepository;
            _subjectTypeBuilders = subjectTypeBuilders;
        }

        public async Task Validate(Client client, CancellationToken cancellationToken)
        {
            var authGrantTypes = _responseTypeHandlers.Select(r => r.GrantType);
            var supportedGrantTypes = _grantTypeHandlers.Select(g => g.GrantType).Union(authGrantTypes).Distinct();
            var notSupportedGrantTypes = client.GrantTypes.Where(gt => !supportedGrantTypes.Any(sgt => sgt == gt));
            if (notSupportedGrantTypes.Any())
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.UNSUPPORTED_GRANT_TYPES, string.Join(",", notSupportedGrantTypes)));

            if (!string.IsNullOrWhiteSpace(client.TokenEndPointAuthMethod) && !_oauthClientAuthenticationHandlers.Any(o => o.AuthMethod == client.TokenEndPointAuthMethod))
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.UNKNOWN_AUTH_METHOD, client.TokenEndPointAuthMethod));

            if (!string.IsNullOrWhiteSpace(client.ApplicationType) && client.ApplicationType != "web" && client.ApplicationType != "native")
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_APPLICATION_TYPE);

            if (!string.IsNullOrWhiteSpace(client.SectorIdentifierUri))
            {
                if (!Uri.IsWellFormedUriString(client.SectorIdentifierUri, UriKind.Absolute))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_SECTOR_IDENTIFIER_URI);

                var uri = new Uri(client.SectorIdentifierUri);
                if (uri.Scheme != "https")
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_HTTPS_SECTOR_IDENTIFIER_URI);
            }

            if (!string.IsNullOrWhiteSpace(client.SubjectType) && !_subjectTypeBuilders.Any(s => s.SubjectType == client.SubjectType))
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_SUBJECT_TYPE);

            if (!string.IsNullOrWhiteSpace(client.InitiateLoginUri))
            {
                if (!Uri.IsWellFormedUriString(client.InitiateLoginUri, UriKind.Absolute))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_INITIATE_LOGIN_URI);

                var uri = new Uri(client.InitiateLoginUri);
                if (uri.Scheme != "https")
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_HTTPS_INITIATE_LOGIN_URI);
            }

            var supportedResponseTypeHandlers = _responseTypeHandlers.Where(r => client.GrantTypes.Contains(r.GrantType));
            if (supportedResponseTypeHandlers.Any())
            {
                var supportedResponseTypes = supportedResponseTypeHandlers.Select(s => s.ResponseType);
                var unSupportedResponseTypes = client.ResponseTypes.Where(r => !supportedResponseTypes.Contains(r));
                if (unSupportedResponseTypes.Any())
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.BAD_RESPONSE_TYPES, string.Join(",", unSupportedResponseTypes)));
            }

            foreach (var kvp in supportedResponseTypeHandlers.GroupBy(k => k.GrantType))
            {
                if (!kvp.Any(k => client.ResponseTypes.Contains(k.ResponseType)))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.MISSING_RESPONSE_TYPE, kvp.Key));
            }

            if (!string.IsNullOrWhiteSpace(client.Scope))
            {
                var scopes = client.Scope.ToScopes();
                var existingScopes = await _scopeRepository.Query().Where(s => scopes.Contains(s.Name)).ToListAsync(cancellationToken);
                var existingScopeNames = existingScopes.Select(s => s.Name);
                var unsupportedScopes = scopes.Except(existingScopeNames);
                if (unsupportedScopes.Any())
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.UNSUPPORTED_SCOPES, string.Join(",", unsupportedScopes)));
            }

            CheckUri(client.JwksUri, ErrorMessages.BAD_JWKS_URI);
            CheckUris(client.Translations, OAuthClientParameters.ClientUri, ErrorMessages.BAD_CLIENT_URI);
            CheckUris(client.Translations, OAuthClientParameters.LogoUri, ErrorMessages.BAD_LOGO_URI);
            CheckUris(client.Translations, OAuthClientParameters.TosUri, ErrorMessages.BAD_TOS_URI);
            CheckUris(client.Translations, OAuthClientParameters.PolicyUri, ErrorMessages.BAD_POLICY_URI);
            CheckSignature(client.TokenSignedResponseAlg, ErrorMessages.UNSUPPORTED_TOKEN_SIGNED_RESPONSE_ALG);
            CheckEncryption(client.TokenEncryptedResponseAlg, client.TokenEncryptedResponseEnc, ErrorMessages.UNSUPPORTED_TOKEN_ENCRYPTED_RESPONSE_ALG, ErrorMessages.UNSUPPORTED_TOKEN_ENCRYPTED_RESPONSE_ENC, OAuthClientParameters.TokenEncryptedResponseAlg);  
            
            CheckSignature(client.IdTokenSignedResponseAlg, ErrorMessages.UNSUPPORTED_IDTOKEN_SIGNED_RESPONSE_ALG); CheckEncryption(client.IdTokenEncryptedResponseAlg, client.IdTokenEncryptedResponseEnc, ErrorMessages.UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ALG, ErrorMessages.UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ENC, OAuthClientParameters.IdTokenEncryptedResponseAlg);
            CheckSignature(client.UserInfoSignedResponseAlg, ErrorMessages.UNSUPPORTED_USERINFO_SIGNED_RESPONSE_ALG);
            CheckEncryption(client.UserInfoEncryptedResponseAlg, client.UserInfoEncryptedResponseEnc, ErrorMessages.UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ALG, ErrorMessages.UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ENC, OAuthClientParameters.UserInfoEncryptedResponseAlg);
            CheckSignature(client.RequestObjectSigningAlg, ErrorMessages.UNSUPPORTED_REQUEST_OBJECT_SIGNING_ALG);
            CheckEncryption(client.RequestObjectEncryptionAlg, client.RequestObjectEncryptionEnc, ErrorMessages.UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ALG, ErrorMessages.UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ENC, OAuthClientParameters.RequestObjectEncryptionAlg);

            if (supportedResponseTypeHandlers.Any())
            {
                if (client.RedirectionUrls == null || !client.RedirectionUrls.Any())
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.MISSING_PARAMETER, OAuthClientParameters.RedirectUris));

                foreach (var redirectUrl in client.RedirectionUrls)
                {
                    CheckRedirectUrl(client, redirectUrl);
                }
            }
        }

        protected static void CheckUris(ICollection<Translation> translations, string name, string errorMessage)
        {
            var filteredTranslations = translations.Where(s => s.Key == name);
            if (!filteredTranslations.Any())
                return;

            foreach (var translation in filteredTranslations)
                CheckUri(translation.Value, errorMessage);
        }

        protected static void CheckUri(string url, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(errorMessage, url));
        }

        protected virtual void CheckRedirectUrl(Client client, string redirectUrl)
        {
            if (!Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute))
                throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, string.Format(ErrorMessages.BAD_REDIRECT_URI, redirectUrl));

            Uri.TryCreate(redirectUrl, UriKind.Absolute, out Uri uri);
            if (!string.IsNullOrWhiteSpace(uri.Fragment))
                throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, ErrorMessages.REDIRECT_URI_CONTAINS_FRAGMENT);

            if (client.ApplicationType == "web")
            {
                if (uri.Scheme.ToLowerInvariant() != "https")
                    throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, ErrorMessages.INVALID_HTTPS_REDIRECT_URI);

                if (uri.Host.ToLowerInvariant() == "localhost")
                {
                    throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, ErrorMessages.INVALID_LOCALHOST_REDIRECT_URI);
                }
            }
        }

        protected void CheckSignature(string alg, string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(alg) && !Constants.AllSigningAlgs.Contains(alg))
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, errorMessage);
        }

        protected void CheckEncryption(string alg, string enc, string unsupportedAlgMsg, string unsupportedEncMsg, string parameterName)
        {
            if (!string.IsNullOrWhiteSpace(alg) && !Constants.AllEncAlgs.Contains(alg))
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, unsupportedAlgMsg);

            if (!string.IsNullOrWhiteSpace(enc) && !Constants.AllEncryptions.Contains(enc))
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, unsupportedEncMsg);

            if (!string.IsNullOrWhiteSpace(enc) && string.IsNullOrWhiteSpace(alg))
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.MISSING_PARAMETER, parameterName));
        }

        protected virtual void CheckBC(RegisterClientRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.BCTokenDeliveryMode))
            {
                if (!Constants.AllStandardNotificationModes.Contains(request.BCTokenDeliveryMode))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_BC_DELIVERY_MODE);

                if (request.BCTokenDeliveryMode == Constants.StandardNotificationModes.Ping ||
                    request.BCTokenDeliveryMode == Constants.StandardNotificationModes.Push)
                {
                    if (string.IsNullOrWhiteSpace(request.BCClientNotificationEndpoint))
                        throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.MISSING_PARAMETER, OAuthClientParameters.BCClientNotificationEndpoint));
                }
            }

            if (!string.IsNullOrWhiteSpace(request.BCClientNotificationEndpoint))
            {
                if (!Uri.TryCreate(request.BCClientNotificationEndpoint, UriKind.Absolute, out Uri uri))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_BC_CLIENT_NOTIFICATION_EDP);

                if (uri.Scheme != "https")
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_HTTPS_BC_CLIENT_NOTIFICATION_EDP);
            }

            if (!string.IsNullOrWhiteSpace(request.BCAuthenticationRequestSigningAlg))
                CheckSignature(request.BCAuthenticationRequestSigningAlg, ErrorMessages.UNSUPPORTED_BC_AUTHENTICATION_REQUEST_SIGNING_ALG);
        }
    }
}
