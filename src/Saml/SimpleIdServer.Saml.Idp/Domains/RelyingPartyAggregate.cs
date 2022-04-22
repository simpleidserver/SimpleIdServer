// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Idp.Resources;
using SimpleIdServer.Saml.Stores;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public int AssertionExpirationBeforeInSeconds { get; set; }
        public ICollection<RelyingPartyClaimMapping> ClaimMappings { get; set; }

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

        public async Task<IEnumerable<X509Certificate2>> GetSigningCertificates(IEntityDescriptorStore entityDescriptorStore, CancellationToken cancellationToken)
        {
            var ssp = await GetSpSSODescriptor(entityDescriptorStore, cancellationToken);
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

        public async Task<bool> GetAuthnRequestsSigned(IEntityDescriptorStore entityDescriptorStore, CancellationToken cancellationToken)
        {
            var ssp = await GetSpSSODescriptor(entityDescriptorStore, cancellationToken);
            return ssp.AuthnRequestsSignedSpecified && ssp.AuthnRequestsSigned;
        }

        public async Task<string> GetAssertionLocation(IEntityDescriptorStore entityDescriptorStore, string binding, CancellationToken cancellationToken)
        {
            var ssp = await GetSpSSODescriptor(entityDescriptorStore, cancellationToken);
            var assertionConsumerService = ssp.AssertionConsumerService.FirstOrDefault(a => a.Binding == binding);
            if (assertionConsumerService == null)
            {
                return null;
            }

            return assertionConsumerService.Location;
        }

        public async Task<bool> GetAssertionSigned(IEntityDescriptorStore entityDescriptorStore, CancellationToken cancellationToken)
        {
            var ssp = await GetSpSSODescriptor(entityDescriptorStore, cancellationToken);
            return ssp.WantAssertionsSignedSpecified && ssp.WantAssertionsSigned;
        }

        #endregion

        public static RelyingPartyAggregate Create(string metadataUrl)
        {
            var result = new RelyingPartyAggregate
            {
                Id = Guid.NewGuid().ToString(),
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                MetadataUrl = metadataUrl
            };
            return result;
        }

        public object Clone()
        {
            return new RelyingPartyAggregate
            {
                AssertionExpirationTimeInSeconds = AssertionExpirationTimeInSeconds,
                AssertionExpirationBeforeInSeconds = AssertionExpirationBeforeInSeconds,
                ClaimMappings = ClaimMappings.Select(c => (RelyingPartyClaimMapping)c.Clone()).ToList(),
                CreateDateTime = CreateDateTime,
                Id = Id,
                MetadataUrl = MetadataUrl,
                UpdateDateTime = UpdateDateTime
            };
        }

        protected async Task<SPSSODescriptorType> GetSpSSODescriptor(IEntityDescriptorStore entityDescriptorStore, CancellationToken cancellationToken)
        {
            var entityDescriptor = await entityDescriptorStore.Get(MetadataUrl, cancellationToken);
            var ssp = entityDescriptor.Items.FirstOrDefault(i => i is SPSSODescriptorType) as SPSSODescriptorType;
            if (ssp == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadRelyingPartySpMetadata);
            }

            return ssp;
        }
    }
}
