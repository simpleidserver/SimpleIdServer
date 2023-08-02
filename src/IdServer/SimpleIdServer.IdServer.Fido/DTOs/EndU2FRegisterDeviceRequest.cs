// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Fido.DTOs
{
    public class EndU2FRegisterDeviceRequest
    {
        [JsonPropertyName(EndU2FRegisterDeviceRequestNames.DeviceType)]
        public string DeviceType { get; set; }
        [JsonPropertyName(EndU2FRegisterDeviceRequestNames.Model)]
        public string Model { get; set; }
        [JsonPropertyName(EndU2FRegisterDeviceRequestNames.Manufacturer)]
        public string Manufacturer { get; set; }
        [JsonPropertyName(EndU2FRegisterDeviceRequestNames.Name)]
        public string Name { get; set; }
        [JsonPropertyName(EndU2FRegisterDeviceRequestNames.Version)]
        public string Version { get; set; }
        [JsonPropertyName(EndU2FRegisterDeviceRequestNames.PushToken)]
        public string PushToken { get; set; }
        [JsonPropertyName(EndU2FRegisterDeviceRequestNames.PushType)]
        public string PushType { get; set; }
    }
}
