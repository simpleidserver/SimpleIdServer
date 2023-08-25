// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2;
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
        Saml2Configuration BuildSamlIdpConfiguration(string issuer, string prefix);
        Saml2Configuration BuildSamSpConfiguration(Client rp);
    }

    public class Saml2ConfigurationFactory : ISaml2ConfigurationFactory
    {
        private readonly IKeyStore _keyStore;
        private readonly SamlIdpOptions _options;

        public Saml2ConfigurationFactory(IKeyStore keyStore, IOptions<SamlIdpOptions> options)
        {
            _keyStore = keyStore;
            _options = options.Value;
        }

        public Saml2Configuration BuildSamlIdpConfiguration(string issuer, string realm)
        {
            X509Certificate2 sigCertificate = null;
            if (_options.SignAuthnRequest) sigCertificate = GetSigningCertificates(realm).FirstOrDefault();
            var result = new Saml2Configuration
            {
                Issuer = issuer,
                SignAuthnRequest = _options.SignAuthnRequest,
                SigningCertificate = sigCertificate
            };
            return result;
        }

        public Saml2Configuration BuildSamSpConfiguration(Client rp)
        {
            var signingCertificate = rp.GetSaml2SigningCertificate();
            var result = new Saml2Configuration
            {
                Issuer = rp.ClientId,
                SignAuthnRequest = signingCertificate != null,
                SigningCertificate = signingCertificate
            };            
            return result;
        }

        private List<X509Certificate2> GetSigningCertificates(string realm)
        {
            var keys = _keyStore.GetAllSigningKeys(realm);
            return keys.Select(k => k.Key).Where(k => k is X509SecurityKey).Cast<X509SecurityKey>().Select(k => k.Certificate).ToList();
        }
    }
}
