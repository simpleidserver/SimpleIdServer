// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class ExternalAuthProviderViewModel
    {
        public string Scheme { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public DateTime CreateDateTime { get; set; }
    }
}
