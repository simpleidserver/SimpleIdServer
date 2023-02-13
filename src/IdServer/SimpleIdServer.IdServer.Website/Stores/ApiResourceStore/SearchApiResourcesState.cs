// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ApiResourceStore
{
    [FeatureState]
    public record SearchApiResourcesState
    {
        public SearchApiResourcesState() { }

        public SearchApiResourcesState(bool isLoading, IEnumerable<ApiResource> apiResources)
        {
            ApiResources = apiResources.Select(c => new SelectableApiResource(c));
            Count = apiResources.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableApiResource>? ApiResources { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
    }

    public class SelectableApiResource
    {
        public SelectableApiResource(ApiResource apiResource) 
        {
            Value = apiResource;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public ApiResource Value { get; set; }
    }
}
