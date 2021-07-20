// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Saml.Builders;
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Resources;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public class SingleSignOnHandler : ISingleSignOnHandler
    {
        private readonly IRelyingPartyRepository _relyingPartyRepository;
        private readonly IEnumerable<IAuthenticator> _authenticators;
        private readonly IUserRepository _userRepository;
        private readonly SamlIdpOptions _options;
        private readonly ILogger<SingleSignOnHandler> _logger;

        public SingleSignOnHandler(
            IRelyingPartyRepository relyingPartyRepository,
            IEnumerable<IAuthenticator> authenticators,
            IUserRepository userRepository,
            IOptions<SamlIdpOptions> options,
            ILogger<SingleSignOnHandler> logger)
        {
            _relyingPartyRepository = relyingPartyRepository;
            _authenticators = authenticators;
            _userRepository = userRepository;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<SingleSignOnResult> Handle(SingleSignOnParameter parameter, CancellationToken cancellationToken)
        {
            var authnRequest = CheckParameter(parameter);
            var relyingParty = await CheckRelyingParty(authnRequest, cancellationToken);
            var authenticator = CheckAuthnContextClassRef(authnRequest);
            if (string.IsNullOrWhiteSpace(parameter.UserId))
            {
                return SingleSignOnResult.Redirect(authenticator.Amr);
            }

            var user = await _userRepository.Get(parameter.UserId, cancellationToken);
            var validationResult = CheckNameIDPolicy(relyingParty, user, authnRequest);
            var response = BuildResponse(relyingParty, validationResult);
            return SingleSignOnResult.Ok(response);
        }

        protected virtual AuthnRequestType CheckParameter(SingleSignOnParameter parameter)
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadAuthnRequestCompression);
            }

            AuthnRequestType authnRequest = null;
            try
            {
                authnRequest = decompressed.DeserializeAuthnRequestFromXml();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadAuthnRequestXml);
            }

            if (!SignatureHelper.CheckSignature(authnRequest))
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadAuthnRequestSignature);
            }

            return authnRequest;
        }

        protected virtual async Task<RelyingPartyAggregate> CheckRelyingParty(AuthnRequestType authnRequest, CancellationToken cancellationToken)
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
                throw new SamlException(HttpStatusCode.NotFound, Saml.Constants.StatusCodes.Requester, string.Format(Global.UnknownIssuer, nameof(authnRequest.Issuer.Value)));
            }

            return relyingParty;
        }

        protected virtual IAuthenticator CheckAuthnContextClassRef(AuthnRequestType authnRequest)
        {
            if (authnRequest.RequestedAuthnContext != null)
            {
                AuthnContextComparisonType comparison = AuthnContextComparisonType.exact;
                // TODO : Support other comparison.
                if (authnRequest.RequestedAuthnContext.ComparisonSpecified)
                {
                    comparison = authnRequest.RequestedAuthnContext.Comparison;
                }

                if (authnRequest.RequestedAuthnContext.Items == null || !authnRequest.RequestedAuthnContext.Items.Any())
                {
                    throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.MissingAuthnContextClassRef);
                }

                var items = authnRequest.RequestedAuthnContext.Items;
                switch (comparison)
                {
                    case AuthnContextComparisonType.exact:
                        var auth =  _authenticators.FirstOrDefault(a => items.Contains(a.AuthnContextClassRef));
                        if (auth == null)
                        {
                            throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, string.Format(Global.UnknownAuthContextClassRef, string.Join(",", items)));
                        }

                        return auth;
                }
            }

            return _authenticators.First(a => a.AuthnContextClassRef == _options.DefaultAuthnContextClassRef);
        }

        protected virtual NameIDPolicyValidationResult CheckNameIDPolicy(RelyingPartyAggregate relyingParty, User user, AuthnRequestType authnRequest)
        {
            var attributes = ConvertAttributes(relyingParty, user);
            string nameIdValue = user.Id;
            if (authnRequest.NameIDPolicy != null && 
                (string.IsNullOrWhiteSpace(authnRequest.NameIDPolicy.Format) || authnRequest.NameIDPolicy.Format == Saml.Constants.NameIdentifierFormats.Unspecified))
            {
                var attr = attributes.FirstOrDefault(a => a.AttributeName == authnRequest.NameIDPolicy.SPNameQualifier);
                if (attr == null)
                {
                    throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.InvalidNameIDPolicy, string.Format(Global.UnknownNameId, authnRequest.NameIDPolicy.SPNameQualifier));
                }

                nameIdValue = attr.Value;
            }

            return new NameIDPolicyValidationResult
            {
                NameIdValue = nameIdValue,
                Attributes = attributes
            };
        }

        protected virtual ResponseType BuildResponse(RelyingPartyAggregate relyingParty, NameIDPolicyValidationResult validationResult)
        {
            var response = SamlResponseBuilder.New()
                .AddAssertion(cb =>
                {
                    cb.SetIssuer(null, _options.Issuer);
                    cb.SetSubject(s =>
                    {
                        s.SetNameId(null, validationResult.NameIdValue);
                    });
                    cb.SetConditions(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(relyingParty.AssertionExpirationTimeInSeconds), c =>
                    {
                        c.AddAudienceRestriction(relyingParty.Id);
                    });
                    foreach(var attr in validationResult.Attributes)
                    {
                        cb.AddAttributeStatementAttribute(attr.AttributeName, null, attr.Type, attr.Value);
                    }
                })
                .SignAndBuild(relyingParty.Certificate, SignatureAlgorithms.RSASHA1, CanonicalizationMethods.C14);
            return response;
        }

        protected ICollection<SamlAttribute> ConvertAttributes(RelyingPartyAggregate relyingParty, User user)
        {
            var result = new List<SamlAttribute>();
            foreach (var claim in user.Claims)
            {
                var attrName = relyingParty.GetClaim(claim.Type);
                if (!string.IsNullOrWhiteSpace(attrName))
                {
                    result.Add(new SamlAttribute { AttributeName = attrName, Type = claim.Type, Value = claim.Value });
                }
            }

            return result;
        }

        protected class NameIDPolicyValidationResult
        {
            public string NameIdValue { get; set; }
            public ICollection<SamlAttribute> Attributes { get; set; }
        }

        protected class SamlAttribute
        {
            public string AttributeName { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}
