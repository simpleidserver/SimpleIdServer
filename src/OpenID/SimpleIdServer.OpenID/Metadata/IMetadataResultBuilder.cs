// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Metadata
{
    public interface IMetadataResultBuilder
    {
        IMetadataResultBuilder AddTranslatedEnum<T>(string name) where T : struct;
        Task<MetadataResult> Build(string language, CancellationToken cancellationToken);
    }
}
