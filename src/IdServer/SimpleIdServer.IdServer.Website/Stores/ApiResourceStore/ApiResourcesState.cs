// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ApiResourceStore
{
    [FeatureState]
    public record ApiResourcesState
    {
        public ApiResourcesState() { }

        public ApiResourcesState(bool isLoading, IEnumerable<ApiResource> availableApiResources, IEnumerable<ApiResource> activeApiResources)
        {
            AvailableApiResources = availableApiResources.Select(c => new SelectableApiResource(c));
            ActiveApiResources = activeApiResources.Select(c => new SelectableApiResource(c));
            AvailableCount = AvailableApiResources.Count();
            ActiveCount = ActiveApiResources.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableApiResource>? AvailableApiResources { get; set; } = null;
        public IEnumerable<SelectableApiResource>? ActiveApiResources { get; set; } = null;
        public int AvailableCount { get; set; } = 0;
        public int ActiveCount { get; set; } = 0;
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
