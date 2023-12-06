// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Provisioning.SCIM.Jobs
{
    public class SCIMRepresentationsExtractionJobOptions
    {
        [ConfigurationRecord("SCIMEdp", "SCIM Endpoint", order: 0)]
        public string SCIMEdp { get; set; }
        [ConfigurationRecord("Authentication Types", "Select the type of authentication", order: 1)]
        public ClientAuthenticationTypes AuthenticationType { get; set; }
        [ConfigurationRecord("API Key", "Value is present in the appsettings.json file", order: 2, DisplayCondition = "AuthenticationType=APIKEY")]
        public string ApiKey { get; set; }
        [ConfigurationRecord("ClientId", "Client Identifier", order: 3, displayCondition: "AuthenticationType=CLIENT_SECRET_POST")]
        public string ClientId { get; set; }
        [ConfigurationRecord("ClientSecret", "Client Secret", 4, "AuthenticationType=CLIENT_SECRET_POST", CustomConfigurationRecordType.PASSWORD)]
        public string ClientSecret { get; set; }
        [ConfigurationRecord("Count", "Maximum number of records returned by the SCIM endpoint", order: 5)]
        public int Count { get; set; }
    }

    public enum ClientAuthenticationTypes
    {
        [ConfigurationRecordEnum("Api Key")]
        APIKEY = 0,
        [ConfigurationRecordEnum("Client secret post")]
        CLIENT_SECRET_POST = 1
    }
}