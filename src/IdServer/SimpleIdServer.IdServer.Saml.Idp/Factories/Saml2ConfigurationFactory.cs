// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Saml.Idp.Extensions;
using SimpleIdServer.IdServer.Stores;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Saml.Idp.Factories
{
    public interface ISaml2ConfigurationFactory
    {
        Saml2Configuration BuildSamlIdpConfiguration(string url, string issuer, string realm);
        Task<(Saml2Configuration, EntityDescriptor)> BuildSamSpConfiguration(Client rp, CancellationToken cancellationToken);
    }

    public class Saml2ConfigurationFactory : ISaml2ConfigurationFactory
    {
        private readonly IKeyStore _keyStore;
        private readonly Helpers.IHttpClientFactory _httpClientFactory;
        private readonly SamlIdpOptions _options;

        public Saml2ConfigurationFactory(
            IKeyStore keyStore,
            Helpers.IHttpClientFactory httpClientFactory,
            IOptions<SamlIdpOptions> options)
        {
            _keyStore = keyStore;
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public Saml2Configuration BuildSamlIdpConfiguration(string url, string issuer, string realm)
        {
            X509Certificate2 sigCertificate = null;
            if (_options.SignAuthnRequest) sigCertificate = GetSigningCertificates(realm).FirstOrDefault();
            var result = new Saml2Configuration
            {
                Issuer = issuer,
                SignAuthnRequest = _options.SignAuthnRequest,
                SigningCertificate = sigCertificate,
                ArtifactResolutionService = new Saml2IndexedEndpoint
                {
                    Index = 1,
                    Location = new Uri($"{url}/{realm}/{Constants.RouteNames.SingleSignOnArtifact}")
                }
            };
            return result;
        }

        public async Task<(Saml2Configuration, EntityDescriptor)> BuildSamSpConfiguration(Client rp, CancellationToken cancellationToken)
        {
            var spMetadata = await LoadSPMetadata(rp, cancellationToken);
            var signingCertificate = spMetadata.SPSsoDescriptor.SigningCertificates.First();
            var result = new Saml2Configuration
            {
                Issuer = rp.ClientId,
                SignAuthnRequest = signingCertificate != null,
                SigningCertificate = signingCertificate,
                RevocationMode = _options.RevocationMode,
                CertificateValidationMode = _options.CertificateValidationMode
            };
            if(signingCertificate != null) result.SignatureValidationCertificates.Add(signingCertificate);
            return (result, spMetadata);
        }

        private List<X509Certificate2> GetSigningCertificates(string realm)
        {
            var keys = _keyStore.GetAllSigningKeys(realm);
            return keys.Select(k => k.Key).Where(k => k is X509SecurityKey).Cast<X509SecurityKey>().Select(k => k.Certificate).ToList();
        }

        private async Task<EntityDescriptor> LoadSPMetadata(Client client, CancellationToken cancellationToken)
        {
            var spMetadataUrl = client.GetSaml2SpMetadataUrl();
            if (string.IsNullOrWhiteSpace(spMetadataUrl)) throw new Saml2BindingException("Client doesn't contain metadata URL");
            using (var httpClient = _httpClientFactory.GetHttpClient())
            {
                var httpResult = await httpClient.GetAsync(spMetadataUrl, cancellationToken);
                var xml = await httpResult.Content.ReadAsStringAsync();
                var entityDescriptor = new EntityDescriptor();
                entityDescriptor = entityDescriptor.ReadSPSsoDescriptor(xml);
                if (entityDescriptor.SPSsoDescriptor == null) throw new Saml2BindingException($"SPSsoDescriptor not loaded from metadata '{spMetadataUrl}'.");
                return entityDescriptor;
            }
        }
    }
}
