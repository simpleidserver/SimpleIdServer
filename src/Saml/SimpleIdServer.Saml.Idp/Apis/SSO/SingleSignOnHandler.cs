// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Saml.Builders;
using SimpleIdServer.Saml.DTOs;
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Resources;
using SimpleIdServer.Saml.Stores;
using SimpleIdServer.Saml.Validators;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public class SingleSignOnHandler : SAMLValidator, ISingleSignOnHandler
    {
        private readonly IEntityDescriptorStore _entityDescriptorStore;
        private readonly IRelyingPartyRepository _relyingPartyRepository;
        private readonly IEnumerable<IAuthenticator> _authenticators;
        private readonly IUserRepository _userRepository;
        private readonly SamlIdpOptions _options;

        public SingleSignOnHandler(
            IEntityDescriptorStore entityDescriptorStore,
            IRelyingPartyRepository relyingPartyRepository,
            IEnumerable<IAuthenticator> authenticators,
            IUserRepository userRepository,
            IOptions<SamlIdpOptions> options,
            ILogger<SAMLValidator> loggerSamlValidator) : base(loggerSamlValidator)
        {
            _entityDescriptorStore = entityDescriptorStore;
            _relyingPartyRepository = relyingPartyRepository;
            _authenticators = authenticators;
            _userRepository = userRepository;
            _options = options.Value;
        }

        public async Task<SingleSignOnResult> Handle(SAMLRequestDto parameter, string userId, CancellationToken cancellationToken)
        {
            var authnRequest = CheckParameter(parameter);
            var relyingParty = await CheckRelyingParty(authnRequest, cancellationToken);
            var authenticator = CheckAuthnContextClassRef(authnRequest);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return SingleSignOnResult.Redirect(authenticator.Amr);
            }

            var user = await _userRepository.Get(userId, cancellationToken);
            var validationResult = CheckNameIDPolicy(relyingParty, user, authnRequest);
            var response = await BuildResponse(authnRequest, relyingParty, validationResult, cancellationToken);
            return SingleSignOnResult.Ok(response, relyingParty);
        }

        protected virtual AuthnRequestType CheckParameter(SAMLRequestDto parameter)
        {
            var authnRequest = CheckSaml<AuthnRequestType>(parameter.SAMLRequest, parameter.RelayState);
            if (authnRequest.Issuer == null || string.IsNullOrWhiteSpace(authnRequest.Issuer.Value))
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, string.Format(Global.MissingParameter, nameof(authnRequest.Issuer)));
            }

            if (!string.IsNullOrWhiteSpace(authnRequest.Issuer.Format) && authnRequest.Issuer.Format != Saml.Constants.NameIdentifierFormats.EntityIdentifier)
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, string.Format(Global.UnsupportNameIdFormat, nameof(authnRequest.Issuer.Format)));
            }

            if (!string.IsNullOrWhiteSpace(authnRequest.ProtocolBinding) && authnRequest.ProtocolBinding != Saml.Constants.Bindings.HttpRedirect)
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.UnsupportedBinding, string.Format(Global.UnsupportBinding, authnRequest.ProtocolBinding));
            }

            return authnRequest;
        }

        protected virtual async Task<RelyingPartyAggregate> CheckRelyingParty(AuthnRequestType authnRequest, CancellationToken cancellationToken)
        {
            var relyingParty = await _relyingPartyRepository.Get(authnRequest.Issuer.Value, cancellationToken);
            if (relyingParty == null)
            {
                throw new SamlException(HttpStatusCode.NotFound, Saml.Constants.StatusCodes.Requester, string.Format(Global.UnknownIssuer, nameof(authnRequest.Issuer.Value)));
            }

            if (await relyingParty.GetAuthnRequestsSigned(_entityDescriptorStore, cancellationToken))
            {
                var certificates = await relyingParty.GetSigningCertificates(_entityDescriptorStore, cancellationToken);
                foreach (var certificate in certificates)
                {
                    if (SignatureHelper.CheckSignature(authnRequest.SerializeToXmlElement(), certificate))
                    {
                        return relyingParty;
                    }
                }

                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadAuthnRequestSignature);
            }

            if((await relyingParty.GetAssertionLocation(_entityDescriptorStore, Saml.Constants.Bindings.HttpRedirect, cancellationToken)) == null)
            {
                throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.UnsupportedBinding, Global.BadSPAssertionLocation);
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
                    throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.NoAuthnContext, Global.MissingAuthnContextClassRef);
                }

                var items = authnRequest.RequestedAuthnContext.Items;
                switch (comparison)
                {
                    case AuthnContextComparisonType.exact:
                        var auth =  _authenticators.FirstOrDefault(a => items.Contains(a.AuthnContextClassRef));
                        if (auth == null)
                        {
                            throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.NoAuthnContext, string.Format(Global.UnknownAuthContextClassRef, string.Join(",", items)));
                        }

                        return auth;
                }
            }

            return _authenticators.First(a => a.AuthnContextClassRef == _options.DefaultAuthnContextClassRef);
        }

        protected virtual NameIDPolicyValidationResult CheckNameIDPolicy(RelyingPartyAggregate relyingParty, User user, AuthnRequestType authnRequest)
        {
            var attributes = ConvertAttributes(relyingParty, user);
            string nameIdFormat = Saml.Constants.NameIdentifierFormats.EntityIdentifier;
            string nameIdValue = user.Claims.First(c => c.Type == Jwt.Constants.UserClaims.Subject).Value;
            if (authnRequest.NameIDPolicy != null && 
                (string.IsNullOrWhiteSpace(authnRequest.NameIDPolicy.Format) || authnRequest.NameIDPolicy.Format == Saml.Constants.NameIdentifierFormats.Unspecified))
            {
                var attr = attributes.FirstOrDefault(a => a.AttributeFormat == authnRequest.NameIDPolicy.Format);
                if (attr == null)
                {
                    throw new SamlException(HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.InvalidNameIDPolicy, string.Format(Global.UnknownNameId, authnRequest.NameIDPolicy.SPNameQualifier));
                }

                nameIdFormat = attr.AttributeFormat;
                nameIdValue = attr.Value;
            }

            return new NameIDPolicyValidationResult
            {
                NameIdFormat = nameIdFormat,
                NameIdValue = nameIdValue,
                Attributes = attributes
            };
        }

        protected virtual async Task<ResponseType> BuildResponse(AuthnRequestType authnRequest, RelyingPartyAggregate relyingParty, NameIDPolicyValidationResult validationResult, CancellationToken cancellationToken)
        {
            var builder = SamlResponseBuilder.New()
                .AddAssertion(cb =>
                {
                    cb.SetIssuer(Constants.NameIdentifierFormats.PersistentIdentifier, _options.IDPId);
                    cb.SetSubject(s =>
                    {
                        s.SetNameId(validationResult.NameIdFormat, validationResult.NameIdValue);
                        s.AddSubjectConfirmationBearer(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(relyingParty.AssertionExpirationTimeInSeconds), inResponseTo: authnRequest.ID);
                    });
                    cb.SetConditions(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(relyingParty.AssertionExpirationTimeInSeconds), c =>
                    {
                        c.AddAudienceRestriction(relyingParty.Id);
                    });
                    foreach (var attr in validationResult.Attributes)
                    {
                        cb.AddAttributeStatementAttribute(attr.AttributeName, null, attr.Type, attr.Value);
                    }
                });
            if (await relyingParty.GetAssertionSigned(_entityDescriptorStore, cancellationToken))
            {
                return builder.SignAndBuild(_options.SigningCertificate, _options.SignatureAlg.Value, _options.CanonicalizationMethod);
            }

            return builder.Build();
        }

        protected ICollection<SamlAttribute> ConvertAttributes(RelyingPartyAggregate relyingParty, User user)
        {
            var result = new List<SamlAttribute>();
            foreach (var claim in user.Claims)
            {
                var mapping = relyingParty.GetMapping(claim.Type);
                if (mapping != null)
                {
                    result.Add(new SamlAttribute { AttributeName = mapping.ClaimName, Type = claim.Type, Value = claim.Value, AttributeFormat = mapping.ClaimFormat });
                }
            }

            return result;
        }

        protected class NameIDPolicyValidationResult
        {
            public string NameIdFormat { get; set; }
            public string NameIdValue { get; set; }
            public ICollection<SamlAttribute> Attributes { get; set; }
        }

        protected class SamlAttribute
        {
            public string AttributeName { get; set; }
            public string AttributeFormat { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}
