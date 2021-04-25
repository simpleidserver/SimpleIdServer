// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register.Validators
{
    public interface IOAuthClientValidator
    {
        Task Validate(OAuthClient client, CancellationToken cancellationToken);
    }

    public class OAuthClientValidator : IOAuthClientValidator
    {
        private readonly IEnumerable<IResponseTypeHandler> _responseTypeHandlers;
        private readonly IEnumerable<IGrantTypeHandler> _grantTypeHandlers;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _oauthClientAuthenticationHandlers;
        private readonly IEnumerable<ISignHandler> _signHandlers;
        private readonly IOAuthScopeQueryRepository _oauthScopeRepository;
        private readonly IEnumerable<ICEKHandler> _cekHandlers;
        private readonly IEnumerable<IEncHandler> _encHandlers;

        public OAuthClientValidator(
            IEnumerable<IResponseTypeHandler> responseTypeHandlers,
            IEnumerable<IGrantTypeHandler> grantTypeHandlers,
            IEnumerable<IOAuthClientAuthenticationHandler> oAuthClientAuthenticationHandlers,
            IEnumerable<ISignHandler> signHandlers,
            IEnumerable<ICEKHandler> cekHandlers,
            IEnumerable<IEncHandler> encHandlers,
            IOAuthScopeQueryRepository oauthScopeQueryRepository)
        {
            _responseTypeHandlers = responseTypeHandlers;
            _grantTypeHandlers = grantTypeHandlers;
            _oauthClientAuthenticationHandlers = oAuthClientAuthenticationHandlers;
            _signHandlers = signHandlers;
            _cekHandlers = cekHandlers;
            _encHandlers = encHandlers;
            _oauthScopeRepository = oauthScopeQueryRepository;
        }

        public virtual async Task Validate(OAuthClient client, CancellationToken cancellationToken)
        {
            var authGrantTypes = _responseTypeHandlers.Select(r => r.GrantType);
            var supportedGrantTypes = _grantTypeHandlers.Select(g => g.GrantType).Union(authGrantTypes).Distinct();
            var notSupportedGrantTypes = client.GrantTypes.Where(gt => !supportedGrantTypes.Any(sgt => sgt == gt));
            if (notSupportedGrantTypes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.UNSUPPORTED_GRANT_TYPES, string.Join(",", notSupportedGrantTypes)));
            }

            if (!string.IsNullOrWhiteSpace(client.TokenEndPointAuthMethod) && !_oauthClientAuthenticationHandlers.Any(o => o.AuthMethod == client.TokenEndPointAuthMethod))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.UNKNOWN_AUTH_METHOD, client.TokenEndPointAuthMethod));
            }

            var supportedResponseTypeHandlers = _responseTypeHandlers.Where(r => client.GrantTypes.Contains(r.GrantType));
            if (supportedResponseTypeHandlers.Any())
            {
                var supportedResponseTypes = supportedResponseTypeHandlers.Select(s => s.ResponseType);
                var unSupportedResponseTypes = client.ResponseTypes.Where(r => !supportedResponseTypes.Contains(r));
                if (unSupportedResponseTypes.Any())
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.BAD_RESPONSE_TYPES, string.Join(",", unSupportedResponseTypes)));
                }
            }

            foreach (var kvp in supportedResponseTypeHandlers.GroupBy(k => k.GrantType))
            {
                if (!kvp.Any(k => client.ResponseTypes.Contains(k.ResponseType)))
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.MISSING_RESPONSE_TYPE, kvp.Key));
                }
            }

            if (supportedResponseTypeHandlers.Any())
            {
                if (client.RedirectionUrls == null || !client.RedirectionUrls.Any())
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.MISSING_PARAMETER, OAuthClientParameters.RedirectUris));
                }

                foreach (var redirectUrl in client.RedirectionUrls)
                {
                    CheckRedirectUrl(client, redirectUrl);
                }
            }

            var scopes = client.AllowedScopes.Select(_ => _.Name);
            var existingScopeNames = (await _oauthScopeRepository.FindOAuthScopesByNames(scopes, cancellationToken)).Select(s => s.Name);
            var unsupportedScopes = scopes.Except(existingScopeNames);
            if (unsupportedScopes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(ErrorMessages.UNSUPPORTED_SCOPES, string.Join(",", unsupportedScopes)));
            }

            CheckUri(client.JwksUri, ErrorMessages.BAD_JWKS_URI);
            CheckUris(client.ClientUris, ErrorMessages.BAD_CLIENT_URI);
            CheckUris(client.LogoUris, ErrorMessages.BAD_LOGO_URI);
            CheckUris(client.TosUris,ErrorMessages.BAD_TOS_URI);
            CheckUris(client.PolicyUris, ErrorMessages.BAD_POLICY_URI);
            CheckSignature(client.TokenSignedResponseAlg, ErrorMessages.UNSUPPORTED_TOKEN_SIGNED_RESPONSE_ALG);
            CheckEncryption(client.TokenEncryptedResponseAlg, client.TokenEncryptedResponseEnc, ErrorMessages.UNSUPPORTED_TOKEN_ENCRYPTED_RESPONSE_ALG, ErrorMessages.UNSUPPORTED_TOKEN_ENCRYPTED_RESPONSE_ENC, OAuthClientParameters.TokenEncryptedResponseAlg);
            if (!string.IsNullOrWhiteSpace(client.JwksUri) && client.JsonWebKeys != null)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.DUPLICATE_JWKS);
            }
        }

        protected virtual void CheckRedirectUrl(OAuthClient oauthClient, string redirectUrl)
        {
            if (!Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute))
            {
                throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, string.Format(ErrorMessages.BAD_REDIRECT_URI, redirectUrl));
            }

            Uri.TryCreate(redirectUrl, UriKind.Absolute, out Uri uri);
            if (!string.IsNullOrWhiteSpace(uri.Fragment))
            {
                throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, ErrorMessages.REDIRECT_URI_CONTAINS_FRAGMENT);
            }
        }

        protected static void CheckUris(ICollection<OAuthTranslation> translations, string errorMessage)
        {
            if (translations == null || !translations.Any())
            {
                return;
            }

            foreach (var translation in translations)
            {
                CheckUri(translation.Value, errorMessage);
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
