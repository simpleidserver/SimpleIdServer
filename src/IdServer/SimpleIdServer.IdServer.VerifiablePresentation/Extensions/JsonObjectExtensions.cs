// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.VerifiablePresentation.DTOs;

namespace System.Text.Json.Nodes;

public static class JsonObjectExtensions
{
    public static AuthorizationRequestClientMetadata GetClientMetadata(this JsonObject jsonObj)
    {
        if (!jsonObj.ContainsKey(AuthorizationRequestNames.ClientMetadata)) return null;
        var clientMetadata = jsonObj[AuthorizationRequestNames.ClientMetadata].ToJsonString();
        return JsonSerializer.Deserialize<AuthorizationRequestClientMetadata>(clientMetadata);
    }
}