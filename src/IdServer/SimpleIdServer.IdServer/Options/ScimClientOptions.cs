// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Options
{
    public class ScimClientOptions
    {
        public string SCIMEdp { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiKey { get; set; }
        public ScimClientAuthenticateOptions AuthenticationType { get; set; }
    }

    public enum ScimClientAuthenticateOptions
    {
        OAUTH = 0,
        APIKEY = 1
    }
}
