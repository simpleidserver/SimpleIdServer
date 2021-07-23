// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.DTOs;
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Sp.Resources;
using SimpleIdServer.Saml.Stores;
using SimpleIdServer.Saml.Validators;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Sp.Validators
{
    public class SAMLResponseValidator : SAMLValidator, ISAMLResponseValidator
    {
        private readonly SamlSpOptions _options;
        private readonly IEntityDescriptorStore _entityDescriptorStore;

        public SAMLResponseValidator(
            IOptions<SamlSpOptions> options,
            IEntityDescriptorStore entityDescriptorStore,
            ILogger<SAMLValidator> loggerSamlValidator) : base(loggerSamlValidator)
        {
            _options = options.Value;
            _entityDescriptorStore = entityDescriptorStore;
        }

        public async virtual Task<AssertionType> Validate(SAMLResponseDto samlResponse, CancellationToken cancellationToken)
        {
            var certificates = await CheckEntityDescriptor(cancellationToken);
            var response = CheckSamlResponse(samlResponse);
            if (certificates != null)
            {
                CheckSignature(samlResponse, response, certificates);
            }

            return response.Items.First(i => i is AssertionType) as AssertionType;
        }

        protected virtual async Task<IEnumerable<X509Certificate2>> CheckEntityDescriptor(CancellationToken cancellationToken)
        {
            var entityDescriptor = await _entityDescriptorStore.Get(_options.IdpMetadataUrl, cancellationToken);
            var idp = entityDescriptor.Items.FirstOrDefault(i => i is IDPSSODescriptorType) as IDPSSODescriptorType;
            if (idp == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.BadIdpMetadata);
            }

            if (!_options.WantAssertionSigned)
            {
                return null;
            }

            var certificates = new List<X509Certificate2>();
            var signingKeyDescriptors = idp.KeyDescriptor.Where(k => k.use == KeyTypes.signing);
            foreach(var signingKeyDescriptor in signingKeyDescriptors)
            {
                var index = Array.IndexOf(signingKeyDescriptor.KeyInfo.ItemsElementName, ItemsChoiceType2.X509Data);
                if (index < -1)
                {
                    continue;
                }

                var x509Data = signingKeyDescriptor.KeyInfo.Items[index] as X509DataType;
                index = Array.IndexOf(x509Data.ItemsElementName, ItemsChoiceType.X509Certificate);
                if (index < - 1)
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

        protected virtual ResponseType CheckSamlResponse(SAMLResponseDto samlResponse)
        {
            var response = CheckSaml<ResponseType>(samlResponse.SAMLResponse, samlResponse.RelayState);
            var assertion = response.Items.FirstOrDefault(i => i is AssertionType) as AssertionType;
            if (assertion == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.MissingAssertion);
            }

            return response;
        }

        protected virtual void CheckSignature(SAMLResponseDto samlResponse, ResponseType response, IEnumerable<X509Certificate2> certificates)
        {
            if (string.IsNullOrWhiteSpace(samlResponse.SigAlg))
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, string.Format(Global.MissingParameter, "SigAlg"));
            }

            if (string.IsNullOrWhiteSpace(samlResponse.SAMLResponse))
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, string.Format(Global.MissingParameter, "SAMLResponse"));
            }

            if (!certificates.Any(certificate => SignatureHelper.CheckSignature(response.SerializeToXmlElement(), certificate)))
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.BadSamlResponseSignature);
            }

            var assertion = response.Items.First(i => i is AssertionType) as AssertionType;
            if (!certificates.Any(certificate => SignatureHelper.CheckSignature(assertion.SerializeToXmlElement(), certificate)))
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Responder, Global.BadAssertionSignature);
            }

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
}
