// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Bogus;
using FormBuilder;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Pwd.Fakers;

public class PwdAuthFakerService : IFakerDataService
{
    public string CorrelationId => StandardPwdAuthForms.PwdForm.CorrelationId;

    public object Generate()
    {
        var faker = new AuthenticatePasswordViewModelFaker();
        return faker.Generate();
    }
}

public class AuthenticatePasswordViewModelFaker : Faker<AuthenticatePasswordViewModel>
{
    public AuthenticatePasswordViewModelFaker()
    {
        RuleFor(v => v.ExternalIdsProviders, f => f.Make(3, () => new ExternalIdProviderFaker().Generate()));
        RuleFor(v => v.Realm, f => Constants.DefaultRealm);
        RuleFor(v => v.ReturnUrl, f => "https://localhost:5001");
    }
}

public class ExternalIdProviderFaker : Faker<ExternalIdProvider>
{
    public ExternalIdProviderFaker()
    {
        RuleFor(c => c.AuthenticationScheme, f => f.Lorem.Word());
        RuleFor(c => c.DisplayName, f => f.Lorem.Word());
    }
}