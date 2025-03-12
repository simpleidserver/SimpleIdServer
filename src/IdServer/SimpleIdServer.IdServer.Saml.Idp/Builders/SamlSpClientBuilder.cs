// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers.Models;
using SimpleIdServer.IdServer.Saml.Idp.Extensions;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Builders
{
    public class SamlSpClientBuilder
    {
        private readonly Client _client;

        private SamlSpClientBuilder(Client client)
        {
            _client = client;
        }

        public static SamlSpClientBuilder BuildSamlSpClient(string clientId, string metadataUrl, Domains.Realm? realm = null) => BuildSamlSpClient(clientId, metadataUrl, KeyGenerator.GenerateSelfSignedCertificate(), realm);

        public static SamlSpClientBuilder BuildSamlSpClient(string clientId, string metadataUrl, X509Certificate2 certificate, Domains.Realm? realm = null)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                ClientSecret = Guid.NewGuid().ToString(),
                ClientId = clientId,
                ClientType = ClientTypes.SAML,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            if (realm == null) client.Realms.Add(Constants.StandardRealms.Master);
            else client.Realms.Add(realm);
            client.Scopes.Add(Constants.StandardScopes.SAMLProfile);
            client.SetSaml2SpMetadataUrl(metadataUrl);
            client.SetSaml2SigningCertificate(certificate);
            return new SamlSpClientBuilder(client);
        }

        /// <summary>
        /// Set client name.
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public SamlSpClientBuilder SetClientName(string clientName, string language = null)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            _client.Translations.Add(new Translation
            {
                Key = "client_name",
                Value = clientName,
                Language = language
            });
            return this;
        }

        /// <summary>
        /// Use artifact.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public SamlSpClientBuilder SetUseAcsArtifact(bool b)
        {
            _client.SetUseAcsArtifact(b);
            return this;
        }

        public Client Build() => _client;
    }
}
