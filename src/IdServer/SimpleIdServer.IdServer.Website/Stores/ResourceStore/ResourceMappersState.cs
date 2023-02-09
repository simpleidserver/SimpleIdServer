// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ResourceStore
{
    [FeatureState]
    public record ResourceMappersState
    {
        public ResourceMappersState() { }

        public ResourceMappersState(bool isLoading, IEnumerable<SelectableResourceMapper> mappers)
        {
            Mappers = mappers;
            Count = mappers.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableResourceMapper>? Mappers { get; set; } = new List<SelectableResourceMapper>();
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
    }

    public class SelectableResourceMapper
    {
        public SelectableResourceMapper(ScopeClaimMapper mapper)
        {
            Value = mapper;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public ScopeClaimMapper Value { get; set; }
    }
}
