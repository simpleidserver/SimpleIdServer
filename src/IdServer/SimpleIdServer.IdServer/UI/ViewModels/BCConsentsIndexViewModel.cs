// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class BCConsentsIndexViewModel
    {
        public string AuthReqId { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string BindingMessage { get; set; }
        public string ReturnUrl { get; set; }
        public IEnumerable<string> Scopes { get; set; }
    }
}
