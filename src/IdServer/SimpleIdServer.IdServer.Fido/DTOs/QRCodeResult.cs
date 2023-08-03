// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Fido.DTOs
{
    public class QRCodeResult
    {
        [JsonPropertyName(QRCodeResultNames.Action)]
        public string Action { get; set; } = null!;
        [JsonPropertyName(QRCodeResultNames.SessionId)]
        public string SessionId { get; set; } = null!;
        [JsonPropertyName(QRCodeResultNames.ReadQRCodeURL)]
        public string ReadQRCodeURL { get; set; } = null!;
    }
}
