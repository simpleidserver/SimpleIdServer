// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Vc.Models;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialTemplates;

public class UpdateCredentialTemplateParametersRequest
{
    public List<CredentialTemplateParameter> Parameters { get; set; }
}
