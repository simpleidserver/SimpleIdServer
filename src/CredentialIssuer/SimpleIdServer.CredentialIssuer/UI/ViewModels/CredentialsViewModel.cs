// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer.UI.ViewModels;

public class CredentialsViewModel
{
    public List<CredentialConfiguration> CredentialConfigurations { get; set; }
}
