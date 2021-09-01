// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.Builders;
using SimpleIdServer.Saml.DTOs;
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Sp.Events;
using SimpleIdServer.Saml.Sp.Resources;
using SimpleIdServer.Saml.Validators;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleIdServer.Saml.Sp
{
    public class SamlSpHandler : RemoteAuthenticationHandler<SamlSpOptions>
    {
        private static Dictionary<string, string> MappingSamlAttributeToClaim = new Dictionary<string, string>
        {
            { "urn:oid:2.5.4.42", ClaimTypes.GivenName }
        };
        private Dictionary<string, EntityDescriptorType> _entityDescriptors;

        public SamlSpHandler(
            IOptionsMonitor<SamlSpOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _entityDescriptors = new Dictionary<string, EntityDescriptorType>();
        }

        public SamlSpOptions SamlSpOptions
        {
            get
            {
                return Options;
            }
        }

        protected new SamlSpEvents Events
        {
            get { return (SamlSpEvents)base.Events; }
            set { base.Events = value; }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var entityDescriptor = await GetEntityDescriptor(Options.IdpMetadataUrl, CancellationToken.None);
            var idp = entityDescriptor.Items.FirstOrDefault(i => i is IDPSSODescriptorType) as IDPSSODescriptorType;
            if (idp == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Constants.StatusCodes.Requester, Global.BadRelyingPartyIdpMetadata);
            }

            var ssoService = idp.SingleSignOnService.FirstOrDefault(s => s.Binding == Constants.Bindings.HttpRedirect);
            if (ssoService == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Constants.StatusCodes.UnsupportedBinding, Global.BadIDPSingleSignOnLocation);
            }

            var authnRequest = BuildHttpGetBinding(ssoService.Location);
            var uri = new Uri(ssoService.Location);
            var state = Options.StateDataFormat.Protect(properties);
            var redirectionUrl = MessageEncodingBuilder.EncodeHTTPBindingRequest(uri, authnRequest, state, Options.SigningCertificate, Options.SignatureAlg);
            var redirectionContext = new RedirectContext<SamlSpOptions>(
                Context, 
                Scheme, 
                Options, 
                properties,
                redirectionUrl);
            await Events.RedirectToSsoEndpoint(redirectionContext);
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            SAMLResponseDto samlResponse = null;
            if (Request.Method == "GET")
            {
                samlResponse = SAMLResponseDto.Build(Request.Query.ToDictionary(k => k.Key, k => k.Value.First()));
            }
            else
            {
                samlResponse = SAMLResponseDto.Build(Request.Form.ToDictionary(k => k.Key, k => k.Value.First()));
            }

            try
            {
                var properties = Options.StateDataFormat.Unprotect(samlResponse.RelayState);
                if (properties == null)
                {
                    return HandleRequestResult.Fail("The state was missing or invalid.");
                }

                var assertion = await Validate(samlResponse, CancellationToken.None);
                var claimsIdentity = new ClaimsIdentity(BuildClaims(assertion), Scheme.Name);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var ticket = new AuthenticationTicket(claimsPrincipal, properties, Options.SignInScheme);
                return HandleRequestResult.Success(ticket);
            }
            catch(SamlException ex)
            {
                return HandleRequestResult.Fail(ex);
            }
        }

        private static ICollection<Claim> BuildClaims(AssertionType assertion)
        {
            var claims = new List<Claim>();
            var nameId = assertion.Subject.Items.FirstOrDefault(i => i is NameIDType) as NameIDType;
            if (nameId != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, nameId.Value));
            }

            if (assertion.Items != null)
            {
                var attributeStatement = assertion.Items.FirstOrDefault(i => i is AttributeStatementType) as AttributeStatementType;
                if (attributeStatement != null)
                {
                    var attributes = attributeStatement.Items.Where(i => i is AttributeType).Cast<AttributeType>();
                    foreach (var attribute in attributes)
                    {
                        string name = attribute.Name;
                        if (MappingSamlAttributeToClaim.ContainsKey(attribute.Name))
                        {
                            name = MappingSamlAttributeToClaim[attribute.Name];
                        }

                        foreach(var attributeValue in attribute.AttributeValue)
                        {
                            claims.Add(new Claim(name, attributeValue.ToString()));
                        }
                    }
                }
            }

            return claims;
        }

        #region Validate

        public async virtual Task<AssertionType> Validate(SAMLResponseDto samlResponse, CancellationToken cancellationToken)
        {
            var certificates = await CheckEntityDescriptor(cancellationToken);
            var response = CheckSamlResponse(samlResponse);
            CheckSignature(samlResponse, response, certificates);
            return response.Content.Items.First(i => i is AssertionType) as AssertionType;
        }

        protected virtual async Task<IEnumerable<X509Certificate2>> CheckEntityDescriptor(CancellationToken cancellationToken)
        {
            var entityDescriptor = await GetEntityDescriptor(Options.IdpMetadataUrl, cancellationToken);
            var idp = entityDescriptor.Items.FirstOrDefault(i => i is IDPSSODescriptorType) as IDPSSODescriptorType;
            if (idp == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.BadIdpMetadata);
            }

            var certificates = new List<X509Certificate2>();
            var signingKeyDescriptors = idp.KeyDescriptor.Where(k => k.use == KeyTypes.signing);
            foreach (var signingKeyDescriptor in signingKeyDescriptors)
            {
                var index = Array.IndexOf(signingKeyDescriptor.KeyInfo.ItemsElementName, ItemsChoiceType2.X509Data);
                if (index < -1)
                {
                    continue;
                }

                var x509Data = signingKeyDescriptor.KeyInfo.Items[index] as X509DataType;
                index = Array.IndexOf(x509Data.ItemsElementName, ItemsChoiceType.X509Certificate);
                if (index < -1)
                {
                    continue;
                }

                var payload = x509Data.Items[index] as byte[];
                certificates.Add(new X509Certificate2(payload));
            }

            if (!certificates.Any())
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.MissingSignatureCertificates);
            }

            return certificates;
        }

        protected virtual SAMLValidatorResult<ResponseType> CheckSamlResponse(SAMLResponseDto samlResponse)
        {
            var response = SAMLValidator.CheckSaml<ResponseType>(samlResponse.SAMLResponse, samlResponse.RelayState);
            var assertion = response.Content.Items.FirstOrDefault(i => i is AssertionType) as AssertionType;
            if (assertion == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.MissingAssertion);
            }


            return response;
        }

        protected virtual void CheckSignature(SAMLResponseDto samlResponse, SAMLValidatorResult<ResponseType> response, IEnumerable<X509Certificate2> certificates)
        {
            if (string.IsNullOrWhiteSpace(samlResponse.SAMLResponse))
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, string.Format(Global.MissingParameter, "SAMLResponse"));
            }

            if (Options.WantsResponseSigned)
            {
                if (!certificates.Any(certificate => SignatureHelper.CheckSignature(response.Document.DocumentElement, certificate)))
                {
                    throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.BadSamlResponseSignature);
                }
            }

            if (Options.WantAssertionSigned)
            {
                var assertion = response.Document.GetElementsByTagName("Assertion", "urn:oasis:names:tc:SAML:2.0:assertion");
                if (!certificates.Any(certificate => SignatureHelper.CheckSignature(assertion[0] as XmlElement, certificate)))
                {
                    throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.BadAssertionSignature);
                }
            }

            if (!string.IsNullOrWhiteSpace(samlResponse.SigAlg))
            {
                var query = samlResponse.ToQuery(false);
                var sig = Constants.MappingStrToSignature[samlResponse.SigAlg];
                var hashed = Hash.Compute(query, sig);
                if (!certificates.Any(certificate =>
                {
                    var rsa = certificate.GetRSAPublicKey();
                    return rsa.VerifyHash(hashed, Convert.FromBase64String(samlResponse.Signature), Constants.MappingSignatureAlgToHash[sig], RSASignaturePadding.Pkcs1);
                }))
                {
                    throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.BadSignature);
                }
            }
        }

        #endregion

        #region AuthnRequestGenerator

        protected virtual XmlElement BuildHttpGetBinding(string destination)
        {
            return Build(Constants.Bindings.HttpRedirect, destination);
        }

        protected virtual XmlElement Build(string binding, string destination)
        {
            var request = AuthnRequestBuilder.New(Options.SPName)
                .SetBinding(binding)
                .SetDestination(destination)
                .SetIssuer(Constants.NameIdentifierFormats.EntityIdentifier, Options.SPId);
            if (Options.AuthnRequestSigned && Options.SignatureAlg != null && Options.SigningCertificate != null)
            {
                return request.SignAndBuild(Options.SigningCertificate, Options.SignatureAlg.Value, Options.CanonicalizationMethod);
            }

            return request.Build();
        }

        #endregion

        #region EntityTypeDescriptor

        public async Task<EntityDescriptorType> GetEntityDescriptor(string metadataUrl, CancellationToken cancellationToken)
        {
            if (_entityDescriptors.ContainsKey(metadataUrl))
            {
                return _entityDescriptors[metadataUrl];
            }

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetAsync(metadataUrl, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();
                var str = await httpResponse.Content.ReadAsStringAsync();
                var entityDescriptor = str.DeserializeXml<EntityDescriptorType>();
                _entityDescriptors.Add(metadataUrl, entityDescriptor);
                return entityDescriptor;
            }
        }

        #endregion
    }
}
