// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.WsFederation.Api
{
    public class BaseWsFederationController : Controller
    {
        private readonly IdServerWsFederationOptions _options;
        private readonly IKeyStore _keyStore;

        public BaseWsFederationController(IOptions<IdServerWsFederationOptions> options, IKeyStore keyStore)
        {
            _options = options.Value;
            _keyStore = keyStore;
        }

        protected IKeyStore KeyStore => _keyStore;
        protected IdServerWsFederationOptions Options => _options;

        protected SigningCredentials? GetSigningCredentials(string realm) => GetSigningCredentials(_keyStore.GetAllSigningKeys(realm));

        protected SigningCredentials? GetSigningCredentials(IEnumerable<SigningCredentials> sigKeys)
        {
            if (!sigKeys.Any())
                return null;

            var sigKey = sigKeys.FirstOrDefault(k => k.Kid == _options.DefaultKid);
            if (sigKey == null)
                sigKey = sigKeys.First();

            return sigKey;
        }
    }
}
