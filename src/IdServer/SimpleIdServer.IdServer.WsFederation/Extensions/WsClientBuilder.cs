// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.WsFederation;
using SimpleIdServer.IdServer.WsFederation.Extensions;

namespace SimpleIdServer.IdServer.Builders
{
    public static class WsClientBuilder
    {
        public static  WsFederationClientBuilder BuildWsFederationClient(string clientId)
        {
            var client = new Client
            {
                ClientId = clientId,
                ClientSecret = Guid.NewGuid().ToString(),
                ClientType = WsFederationConstants.CLIENT_TYPE,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            client.Realms.Add(SimpleIdServer.IdServer.Constants.StandardRealms.Master);
            client.Scopes.Add(Constants.StandardScopes.SAMLProfile);
            client.SetWsFederationEnabled(true);
            return new WsFederationClientBuilder(client);
        }
    }

    public class WsFederationClientBuilder
    {
        private readonly Client _client;

        internal WsFederationClientBuilder(Client client)
        {
            _client = client;
        }

        #region Translations

        /// <summary>
        /// Set client name.
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public WsFederationClientBuilder SetClientName(string clientName, string language = null)
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

        #endregion

        public Client Build() => _client;
    }
}
