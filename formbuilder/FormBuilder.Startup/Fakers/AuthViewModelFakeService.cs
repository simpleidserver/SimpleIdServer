using Bogus;
using FormBuilder.Startup.Controllers.ViewModels;

namespace FormBuilder.Startup.Fakers;

public class AuthViewModelFakeService : IFakerDataService
{
    public string FormRecordName => Constants.LoginPwdAuthForm.Name;

    public object Generate()
    {
        var faker = new AuthViewModelFaker();
        return faker.Generate();
    }
}

public class ExternalIdProviderFaker : Faker<ExternalIdProviderViewModel>
{
    public ExternalIdProviderFaker()
    {
        RuleFor(c => c.AuthenticationScheme, f => f.Lorem.Word());
        RuleFor(c => c.DisplayName, f => f.Lorem.Word());
    }
}

public class AuthViewModelFaker : Faker<AuthViewModel>
{
    public AuthViewModelFaker()
    {
        RuleFor(v => v.ExternalIdProviders, f => f.Make(3, () => new ExternalIdProviderFaker().Generate()));
        RuleFor(v => v.Login, f => f.Lorem.Word());
        RuleFor(v => v.ReturnUrl, f => f.Lorem.Word());
    }
}