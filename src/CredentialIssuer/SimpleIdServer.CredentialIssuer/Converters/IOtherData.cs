// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.Converters;

public interface IOtherData
{
    JsonObject Data { get; set; }
}
