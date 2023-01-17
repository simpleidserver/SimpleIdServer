// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.BCDeviceRegistration
{
    public class BCDeviceRegistrationRequest
    {
        [BindProperty(Name = BCDeviceRegistrationRequestParameters.DeviceType)]
        [JsonPropertyName(BCDeviceRegistrationRequestParameters.DeviceType)]
        public string DeviceType { get; set; }
        [BindProperty(Name = BCDeviceRegistrationRequestParameters.Options)]
        [JsonPropertyName(BCDeviceRegistrationRequestParameters.Options)]
        public JsonObject Options { get; set; }
    }
}
