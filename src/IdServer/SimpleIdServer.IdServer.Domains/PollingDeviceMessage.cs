// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json;

namespace SimpleIdServer.IdServer.Domains
{
    public class PollingDeviceMessage
    {
        public string DeviceId { get; set; } = null!;
        public string AuthReqId { get; set; } = null!;
        public string? ClientId { get; set; } = null!;
        public string? BindingMessage { get; set; } = null;
        public double ReceptionDateTime { get; set; }
        public IEnumerable<BCAuthorizePermission> Permissions
        {
            get
            {
                return SerializedPermissions == null ? new List<BCAuthorizePermission>() : JsonSerializer.Deserialize<IEnumerable<BCAuthorizePermission>>(SerializedPermissions);
            }
            set
            {
                if (value == null) SerializedPermissions = null;
                else SerializedPermissions = JsonSerializer.Serialize(value);
            }
        }
        public IEnumerable<string> Scopes { get; set; } = new List<string>();
        public string? SerializedPermissions { get; set; } = null;
    }
}
