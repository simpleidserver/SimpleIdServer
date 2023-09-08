// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;

namespace SimpleIdServer.Configuration
{
    public class AutomaticConfigurationSource : IConfigurationSource
    {
        private readonly AutomaticConfigurationOptions _options;
        private readonly IKeyValueConnector _keyValueConnector;

        public AutomaticConfigurationSource(AutomaticConfigurationOptions options, IKeyValueConnector keyValueConnector)
        {
            _options = options;
            _keyValueConnector = keyValueConnector;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AutomaticConfigurationProvider(_options, _keyValueConnector);
        }
    }
}
