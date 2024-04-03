// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders
{
    public class WalletClientBuilder
    {
        private readonly Client _client;

        internal WalletClientBuilder(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Boolean value specifying whether the Credential Issuer expects presentation of a transaction code along with the Token Request in a Pre-Authorized Code Flow. 
        /// </summary>
        /// <returns></returns>
        public WalletClientBuilder RequireTransactionCode()
        {
            _client.IsTransactionCodeRequired = true;
            return this;
        }

        #region Other parameters

        /// <summary>
        /// Set client name.
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public WalletClientBuilder SetClientName(string clientName, string language = null)
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
