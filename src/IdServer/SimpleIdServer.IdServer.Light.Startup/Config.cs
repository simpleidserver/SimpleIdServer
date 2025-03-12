// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Google;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Light.Startup.Converters;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Light.Startup;

public class Config
{
    private static AuthenticationSchemeProviderDefinition Google = AuthenticationSchemeProviderDefinitionBuilder.Create("google", "Google", typeof(GoogleHandler), typeof(GoogleOptionsLite)).Build();
    public static readonly Scope Scope = ScopeBuilder.CreateApiScope("api1", false).Build();

    public static List<Client> Clients => new List<Client>
    {
        ClientBuilder.BuildApiClient("client", "secret").AddScope(Scope).Build()
    };

    public static List<Scope> Scopes => new List<Scope>
    {
        Scope
    };

    public static List<User> Users => new List<User>
    {
        UserBuilder.Create("administrator", "password").SetEmail("adm@mail.com").SetPhoneNumber("0485").GenerateRandomTOTPKey().Build()
    };

    public static List<AuthenticationSchemeProvider> AuthenticationSchemes => new List<AuthenticationSchemeProvider>
    {
        AuthenticationSchemeProviderBuilder.Create(Google, "Google", "Google", "Google").Build()
    };

    public static List<AuthenticationSchemeProviderDefinition> AuthenticationSchemeDefinitions => new List<AuthenticationSchemeProviderDefinition>
    {
        Google
    };

    public static List<Realm> Realms => new List<Realm>
    {
        RealmBuilder.CreateMaster().Build()
    };

    public static List<Language> Languages => new List<Language>
    {
        LanguageBuilder.Build(Language.Default).AddDescription("English", "en").AddDescription("Anglais", "fr").Build(),
        LanguageBuilder.Build("fr").AddDescription("French", "en").AddDescription("Français", "fr").Build()
    };
}