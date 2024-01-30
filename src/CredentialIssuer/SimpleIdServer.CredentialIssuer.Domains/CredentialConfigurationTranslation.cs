// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.CredentialIssuer.Domains;

public class CredentialConfigurationTranslation
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Locale { get; set; } = null;
    public string? LogoUrl { get; set; } = null;
    public string? LogoAltText { get; set; } = null;
    public string? Description { get; set; } = null;
    public string? BackgroundColor { get; set; } = null;
    public string? TextColor { get; set; } = null;
}
