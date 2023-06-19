// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.AcrsStore
{
    [FeatureState]
    public record UpdateAcrState
    {
        public UpdateAcrState()
        {

        }

        public UpdateAcrState(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool IsUpdating { get; set; }
        public string ErrorMessage { get; set; }
    }
}
