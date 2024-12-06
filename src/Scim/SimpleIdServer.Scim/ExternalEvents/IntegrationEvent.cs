﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {

        }

        public IntegrationEvent(string id, string version, string resourceType, string realm)
        {
            Id = id;
            Version = version;
            ResourceType = resourceType;
            Realm = realm;
        }


        public IntegrationEvent(string id, string version, string resourceType, string realm, JObject representation) : this(id, version, resourceType, realm)
        {
            SerializedRepresentation = representation?.ToString();
        }

        public string Id { get; set; }
        public string Version { get; set; }
        public string ResourceType { get; set; }
        public string Realm { get; set; }
        public string SerializedRepresentation { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public JObject Representation
        {
            get
            {
                return string.IsNullOrWhiteSpace(SerializedRepresentation) ? null : JObject.Parse(SerializedRepresentation);
            }
        }
    }
}
