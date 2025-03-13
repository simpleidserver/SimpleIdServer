// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Saml.Idp.DTOs;

public record SamlSSOError
{
    public string ErrorMessage { get; set; } = null!;
}
