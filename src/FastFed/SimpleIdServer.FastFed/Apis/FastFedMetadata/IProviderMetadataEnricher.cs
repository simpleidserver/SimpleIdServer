// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;

namespace SimpleIdServer.FastFed.Apis.FastFedMetadata;

public interface IProviderMetadataEnricher
{
    void EnrichApplicationProvider(JsonObject otherParameters);
}
