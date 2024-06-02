// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("AuthenticationSchemeProviderDefinitions")]
public class SugarAuthenticationSchemeProviderDefinition
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null;
    public string? Image { get; set; } = null;
    public string? HandlerFullQualifiedName { get; set; } = null;
    public string? OptionsFullQualifiedName { get; set; } = null;
    public string? OptionsName { get; set; } = null;
    [Navigate(NavigateType.OneToMany, nameof(SugarAuthenticationSchemeProvider.AuthSchemeProviderDefinitionName))]
    public List<SugarAuthenticationSchemeProvider> AuthSchemeProviders { get; set; }

    public AuthenticationSchemeProviderDefinition ToDomain()
    {
        return new AuthenticationSchemeProviderDefinition
        {
            Name = Name,
            Description = Description,
            Image = Image,
            HandlerFullQualifiedName = HandlerFullQualifiedName,
            OptionsFullQualifiedName = OptionsFullQualifiedName,
            OptionsName = OptionsName
        };
    }
}
