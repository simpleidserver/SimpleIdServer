// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.UI.ViewModels;

public class AuthenticationMethodViewModel
{
    public string Name { get; set; }
    public string Amr { get; set; }
    public bool IsCredentialExists { get; set; }
}
