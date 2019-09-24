// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OpenID.ClaimsEnrichers
{
    public class AggregateHttpClaimsSourceOptions
    {
        public AggregateHttpClaimsSourceOptions(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public AggregateHttpClaimsSourceOptions(string name, string url, string apiToken) : this(name, url)
        {
            ApiToken = apiToken;
        }

        public string Name { get; private set; }
        public string Url { get; private set; }
        public string ApiToken { get; private set; }
    }
}
