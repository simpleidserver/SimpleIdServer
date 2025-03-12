// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers.Models;
using System.Threading;

namespace SimpleIdServer.IdServer.Builders
{
    public class ExternalDeviceClientBuilder
    {
        private readonly Client _client;

        internal ExternalDeviceClientBuilder(Client client)
        {
            _client = client;
        }

        #region Scopes

        public ExternalDeviceClientBuilder AddScope(params Scope[] scopes)
        {
            foreach (var scope in scopes) _client.Scopes.Add(scope);
            return this;
        }

        #endregion

        #region Other Parameters

        public ExternalDeviceClientBuilder SetClientName(string clientName, string language = null)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = Domains.Language.Default;

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
