// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Serializer;

namespace SimpleIdServer.IdServer.Jobs
{
    public class SCIMRepresentationsExtractionJobOptions
    {
        [VisibleProperty("SCIMEdp", "SCIM Endpoint")]
        public string SCIMEdp { get; set; }
        [VisibleProperty("Authentication Types", "Select the type of authentication")]
        public ClientAuthenticationTypes AuthenticationType { get; set; }
        [VisibleProperty("API Key", "Value is present in the appsettings.json file")]
        public string ApiKey { get; set; }
        [VisibleProperty("ClientId", "Client Identifier")]
        public string ClientId { get; set; }
        [VisibleProperty("ClientSecret", "Client Secret")]
        public string ClientSecret { get; set; }
        [VisibleProperty("Count", "Maximum number of records returned by the SCIM endpoint")]
        public int Count { get; set; }
    }

    public enum ClientAuthenticationTypes
    {
        APIKEY = 0,
        CLIENT_SECRET_POST = 1
    }
}
