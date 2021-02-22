// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.UI.ViewModels
{
    public class OpenBankingApiConsentAccountViewModel
    {
        public string Id { get; set; }
        public IEnumerable<string> Permissions { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
    }
}
