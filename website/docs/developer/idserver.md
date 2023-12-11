# Identity Server

## Nuget Packages

| Name                         | Description                            |
| ---------------------------- | -------------------------------------- |
| SimpleIdServer.IdServer      | Identity Server (OPENID)               |
| SimpleIdServer.IdServer.Pwd  | Login / Password authentication module |
| SimpleIdServer.Configuration | Manage the configuration               |

## Quick Start

Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir IdentityService
cd IdentityService
mkdir src
dotnet new sln -n IdentityService
```

Create a web project named IdentityService and install the  `SimpleIdServer.IdServer`,  `SimpleIdServer.IdServer.Pwd`, `SimpleIdServer.Configuration` Nuget packages.

```
cd src
dotnet new web -n IdentityService
cd IdentityService
dotnet add package SimpleIdServer.IdServer
dotnet add package SimpleIdServer.IdServer.Pwd
dotnet add package SimpleIdServer.Configuration
```

Add the `IdentityService` project to your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/IdentityService/IdentityService.csproj
```

Open the Visual Studio Solution, add a `DefaultConfiguration.cs` file, and copy the following content. 
This class contains the default Scopes, Clients, Users, and Realms.

```
public class DefaultConfiguration
{
    public static Scope CredentialTemplates = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "credential_templates",
        Realms = new List<SimpleIdServer.IdServer.Domains.Realm>
        {
            SimpleIdServer.IdServer.Constants.StandardRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };

    public static ICollection<Scope> Scopes => new List<Scope>
    {
        SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope,
        SimpleIdServer.IdServer.Constants.StandardScopes.Profile,
        SimpleIdServer.IdServer.Constants.StandardScopes.SAMLProfile,
        SimpleIdServer.IdServer.Constants.StandardScopes.GrantManagementQuery,
        SimpleIdServer.IdServer.Constants.StandardScopes.GrantManagementRevoke,
        SimpleIdServer.IdServer.Constants.StandardScopes.Users,
        SimpleIdServer.IdServer.Constants.StandardScopes.Register,
        SimpleIdServer.IdServer.Constants.StandardScopes.Provisioning,
        SimpleIdServer.IdServer.Constants.StandardScopes.Address,
        SimpleIdServer.IdServer.Constants.StandardScopes.Networks,
        SimpleIdServer.IdServer.Constants.StandardScopes.Role,
        SimpleIdServer.IdServer.Constants.StandardScopes.CredentialOffer,
        SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationSchemeProviders,
        SimpleIdServer.IdServer.Constants.StandardScopes.RegistrationWorkflows,
        SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationMethods,
        SimpleIdServer.Configuration.Constants.ConfigurationsScope,
        SimpleIdServer.IdServer.Constants.StandardScopes.ApiResources,
        SimpleIdServer.IdServer.Constants.StandardScopes.Auditing,
        SimpleIdServer.IdServer.Constants.StandardScopes.Scopes,
        SimpleIdServer.IdServer.Constants.StandardScopes.CertificateAuthorities,
        CredentialTemplates,
        SimpleIdServer.IdServer.Constants.StandardScopes.Clients,
        SimpleIdServer.IdServer.Constants.StandardScopes.Realms,
        SimpleIdServer.IdServer.Constants.StandardScopes.Groups,
    };

    public static ICollection<User> Users => new List<User>
    {
        UserBuilder.Create("administrator", "password", "Administrator").SetFirstname("Administrator").SetEmail("adm@email.com").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").GenerateRandomTOTPKey().Build()
    };

    public static ICollection<Realm> Realms = new List<Realm>
    {
        SimpleIdServer.IdServer.Constants.StandardRealms.Master
    };

    public static ICollection<Client> Clients => new List<Client>
    {
        ClientBuilder.BuildTraditionalWebsiteClient("SIDS-manager", "password", null, "https://localhost:5002/*", "https://website.simpleidserver.com/*", "https://website.localhost.com/*", "http://website.localhost.com/*", "https://website.sid.svc.cluster.local/*").EnableClientGrantType().SetRequestObjectEncryption().AddPostLogoutUri("https://localhost:5002/signout-callback-oidc").AddPostLogoutUri("https://website.sid.svc.cluster.local/signout-callback-oidc")
            .AddPostLogoutUri("https://website.simpleidserver.com/signout-callback-oidc")
            .AddAuthDataTypes("photo")
            .SetClientName("SimpleIdServer manager")
            .SetBackChannelLogoutUrl("https://localhost:5002/bc-logout")
            .SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png")
            .AddScope(
                SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope,
                SimpleIdServer.IdServer.Constants.StandardScopes.Profile,
                SimpleIdServer.IdServer.Constants.StandardScopes.Provisioning,
                SimpleIdServer.IdServer.Constants.StandardScopes.Users,
                SimpleIdServer.IdServer.Constants.StandardScopes.Networks,
                SimpleIdServer.IdServer.Constants.StandardScopes.CredentialOffer,
                SimpleIdServer.IdServer.Constants.StandardScopes.Acrs,
                SimpleIdServer.Configuration.Constants.ConfigurationsScope,
                SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationSchemeProviders,
                SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationMethods,
                SimpleIdServer.IdServer.Constants.StandardScopes.RegistrationWorkflows,
                SimpleIdServer.IdServer.Constants.StandardScopes.ApiResources,
                SimpleIdServer.IdServer.Constants.StandardScopes.Auditing,
                SimpleIdServer.IdServer.Constants.StandardScopes.Scopes,
                SimpleIdServer.IdServer.Constants.StandardScopes.CertificateAuthorities,
                SimpleIdServer.IdServer.Constants.StandardScopes.Clients,
                SimpleIdServer.IdServer.Constants.StandardScopes.Realms,
                CredentialTemplates,
                SimpleIdServer.IdServer.Constants.StandardScopes.Groups).Build()
    };
}
``` 

Edit the `Program.cs` file, register the dependencies, add the Scopes, Clients, Users, and Realms into the in-memory database, and enable the Login/Password authentication.

The `Masstransit` and `Hangfire` dependencies must be registered. MassTransit is used to raise Integration Events, such as `UserLoginSuccessEvent`. Hangfire is used to schedule jobs, for example, notifying the clients when a user's session has expired.

``` 
builder.Services.AddHangfire((o) => 
{
    o.UseIgnoredAssemblyVersionTypeResolver();
    o.UseInMemoryStorage();
});
builder.Services.AddHangfireServer();
builder.Services.AddMassTransit(o =>
{
    o.UsingInMemory();
});
builder.AddAutomaticConfiguration(o =>
{

});
builder.Services.AddSIDIdentityServer()
    .UseInMemoryStore(c =>
    {
        c.AddInMemoryClients(DefaultConfiguration.Clients);
        c.AddInMemoryScopes(DefaultConfiguration.Scopes);
        c.AddInMemoryRealms(DefaultConfiguration.Realms);
        c.AddInMemoryUsers(DefaultConfiguration.Users);
        c.AddInMemorySerializedFileKeys(new List<SerializedFileKey> { KeyGenerator.GenerateRSASigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master, "rsa-1") });
    })
    .AddPwdAuthentication();
```

Register the Routes.

``` 
app.UseSID();
```

Run the server and check if you can get the [Well-Known configuration](https://localhost:5001/.well-known/openid-configuration).

```
dotnet run --urls https://*:5001
```

Views are not included in the Nuget packages; you must download them and copy them into your solution:

* [wwwroot](https://github.com/simpleidserver/SimpleIdServer/tree/master/src/IdServer/SimpleIdServer.IdServer.Startup/wwwroot) : Javascript and CSS Files.
* [Views](https://github.com/simpleidserver/SimpleIdServer/tree/master/src/IdServer/SimpleIdServer.IdServer.Startup/Views) : CSHTML files.
* [Areas/Pwd](https://github.com/simpleidserver/SimpleIdServer/tree/master/src/IdServer/SimpleIdServer.IdServer.Startup/Areas/pwd) : CSHTML files of the Login/Password authentication module.
* [Resources](https://github.com/simpleidserver/SimpleIdServer/tree/master/src/IdServer/SimpleIdServer.IdServer.Startup/Resources) : Translations.
* [Helpers](https://github.com/simpleidserver/SimpleIdServer/tree/master/src/IdServer/SimpleIdServer.IdServer.Startup/Helpers) : CSHTML Helpers.

Edit the `appsettings.json` file and add the following section. Each authentication module has its own configuration; its values must ALWAYS be present in the appsettings.json file:

``` 
"IdServerPasswordOptions": {
    "NotificationMode": "email",
    "ResetPasswordTitle": "Reset your password",
    "ResetPasswordBody": "Link to reset your password {0}",
    "ResetPasswordLinkExpirationInSeconds": "30",
    "CanResetPassword": "true"
}
```

Fix all the build issues and launch the project. Navigate to the [Home page](https://localhost:5001/Home/Profile) page and check if the authentication is working:

```
dotnet run --urls https://*:5001
```

## Custom EF Storage

The DOTNET TEMPLATE supports the following storage options:
* POSTGRESQL
* SQLSERVER
* MYSQL

Other databases can be supported; the requirement is to check if Entity Framework supports the database. 
You can find the list of supported databases https://learn.microsoft.com/en-us/ef/

If you want to support SQLITE, execute the steps below:

1. Open a command prompt, navigate to the `Identity Server` project, install `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design` Nuget packages.

```
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

2. Edit the `Program.cs` file and configure the SQLITE storage.

```
.UseEFStore(c =>
{
    c.UseSqlite("sid.db", o =>
    {
        o.MigrationsAssembly("IdentityService");
    });
})
```

3. Create the migration script

```
dotnet ef migrations add InitialCreate
```

4. Apply the migration script

```
dotnet ef database update
```

The IdentityServer solution is now configured to use the SQLITE database.
