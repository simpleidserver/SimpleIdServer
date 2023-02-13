// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.ApiResourceStore
{
    [FeatureState]
    public record AddApiResourceState
    {
        public AddApiResourceState() { }

        public AddApiResourceState(bool isAdding, string? errorMessage)
        {
            IsAdding = isAdding;
            ErrorMessage = errorMessage;
        }

        public bool IsAdding { get; set; } = false;
        public string? ErrorMessage { get; set; } = null;
    }
}
