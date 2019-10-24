// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Uma.UI.ViewModels
{
    public class ResourcesIndexViewModel
    {
        public ResourcesIndexViewModel() { }

        public ResourcesIndexViewModel(string idToken)
        {
            IdToken = idToken;
        }

        public string IdToken { get; set; }
    }
}
