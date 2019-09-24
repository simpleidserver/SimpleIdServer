// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OpenID.UI.ViewModels
{
    public class AccountViewModel
    {
        public AccountViewModel(string name, DateTimeOffset? expiresUct, DateTimeOffset? issuedUtc)
        {
            Name = name;
            ExpiresUct = expiresUct;
            IssuedUtc = issuedUtc;
        }

        public string Name { get; }
        public DateTimeOffset? ExpiresUct { get; }
        public DateTimeOffset? IssuedUtc { get; }
    }
}
