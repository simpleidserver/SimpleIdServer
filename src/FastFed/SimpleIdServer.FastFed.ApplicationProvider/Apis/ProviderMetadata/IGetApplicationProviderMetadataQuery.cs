// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.FastFed.ApplicationProvider.Apis.ProviderMetadata;

public interface IGetApplicationProviderMetadataQuery
{
    SimpleIdServer.FastFed.Models.ProviderMetadata Get();
}
