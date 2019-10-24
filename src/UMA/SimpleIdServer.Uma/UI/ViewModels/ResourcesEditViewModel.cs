// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Uma.UI.ViewModels
{
    public class ResourcesEditViewModel
    {
        public ResourcesEditViewModel() { }

        public ResourcesEditViewModel(string idToken, string resourceId)
        {
            IdToken = idToken;
            ResourceId = resourceId;
        }

        public string IdToken { get; set; }
        public string ResourceId { get; set; }
    }
}
