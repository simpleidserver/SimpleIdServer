// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Resources;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public class SingleSignOnHandler : ISingleSignOnHandler
    {
        private readonly IRelyingPartyRepository _relyingPartyRepository;
        private readonly ILogger<SingleSignOnHandler> _logger;

        public SingleSignOnHandler(
            IRelyingPartyRepository relyingPartyRepository,
            ILogger<SingleSignOnHandler> logger)
        {
            _relyingPartyRepository = relyingPartyRepository;
            _logger = logger;
        }

        public async Task Handle(SingleSignOnParameter parameter, CancellationToken cancellationToken)
        {
            await Validate(parameter, cancellationToken);
        }

        protected virtual async Task Validate(SingleSignOnParameter parameter, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(parameter.SAMLRequest))
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, string.Format(Global.MissingParameter, nameof(parameter.SAMLRequest)));
            }

            if (string.IsNullOrWhiteSpace(parameter.RelayState))
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, string.Format(Global.MissingParameter, nameof(parameter.RelayState)));
            }

            string decompressed;
            try
            {
                decompressed = Compression.Decompress(parameter.SAMLRequest);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadAuthnRequestCompression);
            }

            AuthnRequestType authnRequest = null;
            try
            {
                authnRequest = decompressed.DeserializeAuthnRequestFromXml();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadAuthnRequestXml);
            }

            var relyingParty = await GetRelyingParty(authnRequest, cancellationToken);
            if (!SignatureHelper.CheckSignature(authnRequest))
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadAuthnRequestSignature);
            }

            // Check NameIDPolicy.
            // Check AuthnContextClassRef.
        }

        protected virtual async Task<RelyingPartyAggregate> GetRelyingParty(AuthnRequestType authnRequest, CancellationToken cancellationToken)
        {
            if (authnRequest.Issuer == null || string.IsNullOrWhiteSpace(authnRequest.Issuer.Value))
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, string.Format(Global.MissingParameter, nameof(authnRequest.Issuer)));
            }

            if (authnRequest.Issuer.Format != Saml.Constants.NameIdentifierFormats.EntityIdentifier)
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, string.Format(Global.UnsupportNameIdFormat, nameof(authnRequest.Issuer.Format)));
            }

            var relyingParty = await _relyingPartyRepository.Get(authnRequest.Issuer.Value, cancellationToken);
            if (relyingParty == null)
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, string.Format(Global.UnknownIssuer, nameof(authnRequest.Issuer.Value)));
            }

            return relyingParty;
        }
    }
}
