// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Authenticate;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
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
        Task Validate(string realm, Client request, CancellationToken cancellationToken);
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

        public async Task Validate(string realm, Client client, CancellationToken cancellationToken)
        {
            var authGrantTypes = _responseTypeHandlers.Select(r => r.GrantType);
            var supportedGrantTypes = _grantTypeHandlers.Select(g => g.GrantType).Union(authGrantTypes).Distinct();
            var notSupportedGrantTypes = client.GrantTypes.Where(gt => !supportedGrantTypes.Any(sgt => sgt == gt));
            if (notSupportedGrantTypes.Any())
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(Global.UnsupportedGrantTypes, string.Join(",", notSupportedGrantTypes)));

            if (!string.IsNullOrWhiteSpace(client.TokenEndPointAuthMethod) && !_oauthClientAuthenticationHandlers.Any(o => o.AuthMethod == client.TokenEndPointAuthMethod))
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(Global.UnknownAuthMethod, client.TokenEndPointAuthMethod));

            if (!string.IsNullOrWhiteSpace(client.ApplicationType) && client.ApplicationType != "web" && client.ApplicationType != "native")
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, Global.InvalidApplicationType);

            if (!string.IsNullOrWhiteSpace(client.SectorIdentifierUri))
            {
                if (!Uri.IsWellFormedUriString(client.SectorIdentifierUri, UriKind.Absolute))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, Global.InvalidSectorIdentifierUri);

                var uri = new Uri(client.SectorIdentifierUri);
                if (uri.Scheme != "https")
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, Global.InvalidHttpsSectorIdentifierUri);
            }

            if (!string.IsNullOrWhiteSpace(client.SubjectType) && !_subjectTypeBuilders.Any(s => s.SubjectType == client.SubjectType))
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, Global.InvalidSubjectType);

            if (!string.IsNullOrWhiteSpace(client.InitiateLoginUri))
            {
                if (!Uri.IsWellFormedUriString(client.InitiateLoginUri, UriKind.Absolute))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, Global.InvalidInitiateLoginUri);

                var uri = new Uri(client.InitiateLoginUri);
                if (uri.Scheme != "https")
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, Global.InvalidHttpsInitiateLoginUri);
            }

            var supportedResponseTypeHandlers = _responseTypeHandlers.Where(r => client.GrantTypes.Contains(r.GrantType));
            if (supportedResponseTypeHandlers.Any())
            {
                var supportedResponseTypes = supportedResponseTypeHandlers.Select(s => s.ResponseType);
                var unSupportedResponseTypes = client.ResponseTypes.Where(r => !supportedResponseTypes.Contains(r));
                if (unSupportedResponseTypes.Any())
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(Global.BadResponseTypes, string.Join(",", unSupportedResponseTypes)));
            }

            foreach (var kvp in supportedResponseTypeHandlers.GroupBy(k => k.GrantType))
            {
                if (!kvp.Any(k => client.ResponseTypes.Contains(k.ResponseType)))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(Global.MissingResponseType, kvp.Key));
            }

            if (!string.IsNullOrWhiteSpace(client.Scope))
            {
                var scopes = client.Scope.ToScopes();
                var existingScopes = await _scopeRepository.Query().Include(s => s.Realms).Where(s => scopes.Contains(s.Name) && s.Realms.Any(r => r.Name == realm)).ToListAsync(cancellationToken);
                var existingScopeNames = existingScopes.Select(s => s.Name);
                var unsupportedScopes = scopes.Except(existingScopeNames);
                if (unsupportedScopes.Any())
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(Global.UnsupportedScopes, string.Join(",", unsupportedScopes)));
            }

            CheckUri(client.JwksUri, Global.BadJwksUri);
            CheckUris(client.Translations, OAuthClientParameters.ClientUri, Global.BadClientUri);
            CheckUris(client.Translations, OAuthClientParameters.LogoUri, Global.BadLogoUri);
            CheckUris(client.Translations, OAuthClientParameters.TosUri, Global.BadTosUri);
            CheckUris(client.Translations, OAuthClientParameters.PolicyUri, Global.BadPolicyUri);
            CheckSignature(client.TokenSignedResponseAlg, Global.UnsupportedTokenSignedResponseAlg);
            CheckEncryption(client.TokenEncryptedResponseAlg, client.TokenEncryptedResponseEnc, Global.UnsupportedTokenEncryptedResponseAlg, Global.UnsupportedTokenEncryptedResponseEnc, OAuthClientParameters.TokenEncryptedResponseAlg);  
            
            CheckSignature(client.IdTokenSignedResponseAlg, Global.UnsupportedIdTokenSignedResponseAlg); 
            CheckEncryption(client.IdTokenEncryptedResponseAlg, client.IdTokenEncryptedResponseEnc, Global.UnsupportedIdTokenEncryptedResponseAlg, Global.UnsupportedIdTokenEncryptedResponseEnc, OAuthClientParameters.IdTokenEncryptedResponseAlg);
            CheckSignature(client.AuthorizationSignedResponseAlg, Global.UnsupportedAuthorizationSignedResponseAlg);
            CheckEncryption(client.AuthorizationEncryptedResponseAlg, client.AuthorizationEncryptedResponseEnc, Global.UnsupportedAuthorizationEncryptedResponseAlg, Global.UnsupportedAuthorizationEncryptedResponseEnc, client.AuthorizationEncryptedResponseAlg);
            CheckSignature(client.UserInfoSignedResponseAlg, Global.UnsupportedUserInfoSignResponseAlg);
            CheckEncryption(client.UserInfoEncryptedResponseAlg, client.UserInfoEncryptedResponseEnc, Global.UnsupportedUserInfoEncryptedResponseAlg, Global.UnsupportedUserInfoEncryptedResponseEnc, OAuthClientParameters.UserInfoEncryptedResponseAlg);
            CheckSignature(client.RequestObjectSigningAlg, Global.UnsupportedRequestObjectSigningAlg);
            CheckEncryption(client.RequestObjectEncryptionAlg, client.RequestObjectEncryptionEnc, Global.UnsupportedRequestObjectEncryptionAlg, Global.UnsupportedRequestObjectEncryptionEnc, OAuthClientParameters.RequestObjectEncryptionAlg);

            if (supportedResponseTypeHandlers.Any())
            {
                if (client.RedirectionUrls == null || !client.RedirectionUrls.Any())
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(Global.MissingParameter, OAuthClientParameters.RedirectUris));

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
                throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, string.Format(Global.BadRedirectUri, redirectUrl));

            Uri.TryCreate(redirectUrl, UriKind.Absolute, out Uri uri);
            if (!string.IsNullOrWhiteSpace(uri.Fragment))
                throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, Global.RedirectUriContainsFragment);

            if (client.ApplicationType == "web")
            {
                if (uri.Scheme.ToLowerInvariant() != "https")
                    throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, Global.InvalidHttpsRedirectUri);

                if (uri.Host.ToLowerInvariant() == "localhost")
                {
                    throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, Global.InvalidLocahostRedirectUri);
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
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(Global.MissingParameter, parameterName));
        }

        protected virtual void CheckBC(RegisterClientRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.BCTokenDeliveryMode))
            {
                if (!Constants.AllStandardNotificationModes.Contains(request.BCTokenDeliveryMode))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, Global.InvalidBcDeliveryMode);

                if (request.BCTokenDeliveryMode == Constants.StandardNotificationModes.Ping ||
                    request.BCTokenDeliveryMode == Constants.StandardNotificationModes.Push)
                {
                    if (string.IsNullOrWhiteSpace(request.BCClientNotificationEndpoint))
                        throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(Global.MissingParameter, OAuthClientParameters.BCClientNotificationEndpoint));
                }
            }

            if (!string.IsNullOrWhiteSpace(request.BCClientNotificationEndpoint))
            {
                if (!Uri.TryCreate(request.BCClientNotificationEndpoint, UriKind.Absolute, out Uri uri))
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, Global.InvalidBcClientNotificationEdp);

                if (uri.Scheme != "https")
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, Global.InvalidHttpsBcClientNotificationEdp);
            }

            if (!string.IsNullOrWhiteSpace(request.BCAuthenticationRequestSigningAlg))
                CheckSignature(request.BCAuthenticationRequestSigningAlg, Global.UnsupportedBcAuthenticationRequestSigningAlg);
        }
    }
}
