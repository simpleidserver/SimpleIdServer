// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class CredentialTemplateViewModel
    {
        public string Id { get; set; } = null!;
        public CredentialTemplateDisplay? Display {  get; set; } = null;
    }
}
