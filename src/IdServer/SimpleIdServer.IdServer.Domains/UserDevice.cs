// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class UserDevice
    {
        public string Id { get; set; } = null!;
        public string? DeviceType { get; set; } = null;
        public string? Model { get; set; } = null;
        public string? Manufacturer { get; set; } = null;
        public string? Name { get; set; } = null;
        public string? Version { get; set; } = null;
        public string? PushToken { get; set; } = null;
        public string? PushType { get; set; } = null;
        public DateTime CreateDateTime { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
