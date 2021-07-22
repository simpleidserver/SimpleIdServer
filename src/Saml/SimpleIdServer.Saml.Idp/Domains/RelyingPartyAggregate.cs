// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Idp.Resources;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Domains
{
    public class RelyingPartyAggregate : ICloneable
    {
        public RelyingPartyAggregate()
        {
            ClaimMappings = new List<RelyingPartyClaimMapping>();
        }

        #region Properties

        public string Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string MetadataUrl { get; set; }
        public int AssertionExpirationTimeInSeconds { get; set; }
        public ICollection<RelyingPartyClaimMapping> ClaimMappings { get; set; }
        public EntityDescriptorType EntityDescriptor { get; set; }

        #endregion

        #region Get Actions

        public RelyingPartyClaimMapping GetMapping(string userAttribute)
        {
            var claimMapping = ClaimMappings.FirstOrDefault(c => c.UserAttribute == userAttribute);
            if (claimMapping == null)
            {
                return null;
            }

            return claimMapping;
        }

        public async Task<EntityDescriptorType> GetMetadata(CancellationToken cancellationToken)
        {
            // TODO : Implement caching.
            if (EntityDescriptor != null)
            {
                return EntityDescriptor;
            }

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetAsync(MetadataUrl, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();
                var str = await httpResponse.Content.ReadAsStringAsync();
                EntityDescriptor = str.DeserializeXml<EntityDescriptorType>();
                return EntityDescriptor;
            }
        }

        public async Task<IEnumerable<X509Certificate2>> GetSigningCertificates(CancellationToken cancellationToken)
        {
            var ssp = await GetSpSSODescriptor(cancellationToken);
            var result = new List<X509Certificate2>();
            foreach (var keyDescriptor in ssp.KeyDescriptor.Where(k => k.use == KeyTypes.signing))
            {
                var x509 = keyDescriptor.KeyInfo.Items.FirstOrDefault(i => i is X509DataType) as X509DataType;
                if (x509 == null || !x509.ItemsElementName.Any(i => i == ItemsChoiceType.X509Certificate))
                {
                    continue;
                }

                var index = Array.IndexOf(x509.ItemsElementName, ItemsChoiceType.X509Certificate);
                var payload = x509.Items[index] as byte[];
                result.Add(new X509Certificate2(payload));
            }

            return result;
        }

        public async Task<bool> GetAuthnRequestsSigned(CancellationToken cancellationToken)
        {
            var ssp = await GetSpSSODescriptor(cancellationToken);
            return ssp.AuthnRequestsSignedSpecified && ssp.AuthnRequestsSigned;
        }

        public async Task<string> GetAssertionLocation(string binding, CancellationToken cancellationToken)
        {
            var ssp = await GetSpSSODescriptor(cancellationToken);
            var assertionConsumerService = ssp.AssertionConsumerService.FirstOrDefault(a => a.Binding == binding);
            if (assertionConsumerService == null)
            {
                return null;
            }

            return assertionConsumerService.Location;
        }

        public async Task<bool> GetAssertionSigned(CancellationToken cancellationToken)
        {
            var ssp = await GetSpSSODescriptor(cancellationToken);
            return ssp.WantAssertionsSignedSpecified && ssp.WantAssertionsSigned;
        }

        #endregion

        public object Clone()
        {
            throw new NotImplementedException();
        }

        protected async Task<SPSSODescriptorType> GetSpSSODescriptor(CancellationToken cancellationToken)
        {
            var entityDescriptor = await GetMetadata(cancellationToken);
            var ssp = entityDescriptor.Items.FirstOrDefault(i => i is SPSSODescriptorType) as SPSSODescriptorType;
            if (ssp == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadRelyingPartySpMetadata);
            }

            return ssp;
        }
    }
}
