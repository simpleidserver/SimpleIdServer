// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using SimpleIdServer.Jwt.Jws.Handlers;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Register.Validators;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Authenticate;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.SubjectTypeBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Register
{
    public class OpenIdClientValidator : OAuthClientValidator
    {
        private readonly IEnumerable<ISubjectTypeBuilder> _subjectTypeBuilders;
        private readonly OpenIDHostOptions _openIDHostOptions;

        public OpenIdClientValidator(
            IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders,
            IEnumerable<IResponseTypeHandler> responseTypeHandlers, 
            IEnumerable<IGrantTypeHandler> grantTypeHandlers, 
            IEnumerable<IOAuthClientAuthenticationHandler> oAuthClientAuthenticationHandlers, 
            IEnumerable<ISignHandler> signHandlers, 
            IEnumerable<ICEKHandler> cekHandlers, 
            IEnumerable<IEncHandler> encHandlers, 
            IOAuthScopeQueryRepository oauthScopeQueryRepository,
            IOptions<OpenIDHostOptions> openIDHostOptions) : base(responseTypeHandlers, grantTypeHandlers, oAuthClientAuthenticationHandlers, signHandlers, cekHandlers, encHandlers, oauthScopeQueryRepository)
        {
            _subjectTypeBuilders = subjectTypeBuilders;
            _openIDHostOptions = openIDHostOptions.Value;
        }

        public override async Task Validate(OAuthClient client, CancellationToken cancellationToken)
        {
            var openidClient = client as OpenIdClient;
            if (!string.IsNullOrWhiteSpace(openidClient.ApplicationType) && openidClient.ApplicationType != "web" && openidClient.ApplicationType != "native")
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_APPLICATION_TYPE);
            }

            if (!string.IsNullOrWhiteSpace(openidClient.SectorIdentifierUri))
            {
                if (!Uri.IsWellFormedUriString(openidClient.SectorIdentifierUri, UriKind.Absolute))
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_SECTOR_IDENTIFIER_URI);
                }

                var uri = new Uri(openidClient.SectorIdentifierUri);
                if (uri.Scheme != "https")
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_HTTPS_SECTOR_IDENTIFIER_URI);
                }
            }

            if (!string.IsNullOrWhiteSpace(openidClient.InitiateLoginUri))
            {
                if (!Uri.IsWellFormedUriString(openidClient.InitiateLoginUri, UriKind.Absolute))
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_INITIATE_LOGIN_URI);
                }

                var uri = new Uri(openidClient.InitiateLoginUri);
                if (uri.Scheme != "https" && _openIDHostOptions.IsInitiateLoginUriHTTPSRequired)
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_HTTPS_INITIATE_LOGIN_URI);
                }
            }

            if (!string.IsNullOrWhiteSpace(openidClient.SubjectType) && !_subjectTypeBuilders.Any(s => s.SubjectType == openidClient.SubjectType))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_SUBJECT_TYPE);
            }

            CheckSignature(openidClient.IdTokenSignedResponseAlg, ErrorMessages.UNSUPPORTED_IDTOKEN_SIGNED_RESPONSE_ALG);
            CheckEncryption(openidClient.IdTokenEncryptedResponseAlg, openidClient.IdTokenEncryptedResponseEnc, ErrorMessages.UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ALG, ErrorMessages.UNSUPPORTED_IDTOKEN_ENCRYPTED_RESPONSE_ENC, OpenIdClientParameters.IdTokenEncryptedResponseAlg);
            CheckSignature(openidClient.UserInfoSignedResponseAlg, ErrorMessages.UNSUPPORTED_USERINFO_SIGNED_RESPONSE_ALG);
            CheckEncryption(openidClient.UserInfoEncryptedResponseAlg, openidClient.UserInfoEncryptedResponseEnc, ErrorMessages.UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ALG, ErrorMessages.UNSUPPORTED_USERINFO_ENCRYPTED_RESPONSE_ENC, OpenIdClientParameters.UserInfoEncryptedResponseAlg);
            CheckSignature(openidClient.RequestObjectSigningAlg, ErrorMessages.UNSUPPORTED_REQUEST_OBJECT_SIGNING_ALG);
            CheckEncryption(openidClient.RequestObjectEncryptionAlg, openidClient.RequestObjectEncryptionEnc, ErrorMessages.UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ALG, ErrorMessages.UNSUPPORTED_REQUEST_OBJECT_ENCRYPTION_ENC, OpenIdClientParameters.RequestObjectEncryptionAlg);
            CheckBC(openidClient);
            await base.Validate(client, cancellationToken);
        }

        protected override void CheckRedirectUrl(OAuthClient client, string redirectUrl)
        {
            var openidClient = client as OpenIdClient;
            if (!Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute))
            {
                throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, string.Format(OAuth.ErrorMessages.BAD_REDIRECT_URI, redirectUrl));
            }

            var uri = new Uri(redirectUrl);
            if (openidClient.ApplicationType == "web")
            {
                if (uri.Scheme.ToLowerInvariant() != "https" && _openIDHostOptions.IsRedirectionUrlHTTPSRequired)
                {
                    throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, ErrorMessages.INVALID_HTTPS_REDIRECT_URI);
                }

                if (uri.Host.ToLowerInvariant() == "localhost" && !_openIDHostOptions.IsLocalhostAllowed)
                {
                    throw new OAuthException(ErrorCodes.INVALID_REDIRECT_URI, ErrorMessages.INVALID_LOCALHOST_REDIRECT_URI);
                }
            }
        }

        protected virtual void CheckBC(OpenIdClient openIdClient)
        {
            if (!string.IsNullOrWhiteSpace(openIdClient.BCTokenDeliveryMode))
            {
                if (!SIDOpenIdConstants.AllStandardNotificationModes.Contains(openIdClient.BCTokenDeliveryMode))
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_BC_DELIVERY_MODE);
                }

                if (openIdClient.BCTokenDeliveryMode == SIDOpenIdConstants.StandardNotificationModes.Ping ||
                    openIdClient.BCTokenDeliveryMode == SIDOpenIdConstants.StandardNotificationModes.Push)
                {
                    if (string.IsNullOrWhiteSpace(openIdClient.BCClientNotificationEndpoint))
                    {
                        throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OpenIdClientParameters.BCClientNotificationEndpoint));
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(openIdClient.BCClientNotificationEndpoint))
            {
                if (!Uri.TryCreate(openIdClient.BCClientNotificationEndpoint, UriKind.Absolute, out Uri uri))
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_BC_CLIENT_NOTIFICATION_EDP);
                }

                if (uri.Scheme != "https")
                {
                    throw new OAuthException(ErrorCodes.INVALID_CLIENT_METADATA, ErrorMessages.INVALID_HTTPS_BC_CLIENT_NOTIFICATION_EDP);
                }
            }

            if (!string.IsNullOrWhiteSpace(openIdClient.BCAuthenticationRequestSigningAlg))
            {
                CheckSignature(openIdClient.BCAuthenticationRequestSigningAlg, ErrorMessages.UNSUPPORTED_BC_AUTHENTICATION_REQUEST_SIGNING_ALG);
            }
        }
    }
}
