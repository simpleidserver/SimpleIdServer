// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class DeviceCodeViewModel
    {
        public string ClientName { get; set; }
        public string PictureUri { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public string UserCode { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
