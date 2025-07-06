# Windows authentication

Integrating Windows authentication into your identity server can streamline user login processes by leveraging existing Windows credentials. In this article, we’ll walk through the steps required to configure Windows authentication using the `Microsoft.AspNetCore.Authentication.Negotiate` NuGet package, along with code examples to help you get started.

## 1. Installing the nuget package

The first step is to add the `Microsoft.AspNetCore.Authentication.Negotiate` package to your project. This package provides support for Windows-based authentication protocols. You can install it via the command line:

```bash  title="cmd.exe"
dotnet add package Microsoft.AspNetCore.Authentication.Negotiate
```

This command downloads and integrates the package into your project, enabling you to use its functionality in your identity server.

## 2. Creating the NegotiateOptionsLite Class

Once the package is installed, you need to create a lightweight class named `NegotiateOptionsLite`. This class extends `IDynamicAuthenticationOptions<NegotiateOptions>` and is responsible for converting your dynamic options into a standard NegotiateOptions object. Even though the class itself is empty, it serves as a placeholder for further customizations if needed.

Here’s the C# implementation:

```csharp  title="NegotiateOptionsLite.cs"
public class NegotiateOptionsLite : IDynamicAuthenticationOptions<NegotiateOptions>
{
    public NegotiateOptions Convert()
    {
        return new NegotiateOptions();
    }
}
```

## 3. Defining the Authentication Scheme

After creating the options class, you must define a new authentication scheme. This definition acts as a blueprint for how Windows authentication should behave within your identity server. You can build the scheme using the following code:

```csharp
AuthenticationSchemeProviderDefinition Negotiate = AuthenticationSchemeProviderDefinitionBuilder
    .Create("negotiate", "Negotiate", typeof(NegotiateHandler), typeof(NegotiateOptionsLite))
    .Build();
```

This code creates a new provider definition that links the scheme name with its handler and options class.

## 4. Creating an Instance of the Authentication Scheme

With the definition in place, the next step is to create an instance of the authentication scheme. This instance maps the scheme to the specific authentication logic and determines how user claims are extracted. Use the following code to create the instance:

```csharp
AuthenticationSchemeProviderBuilder
    .Create(Negotiate, "Negotiate", "Negotiate", "Negotiate")
    .SetSubject(ClaimTypes.Name)
    .Build();
```

By calling SetSubject(ClaimTypes.Name), you specify that the authenticated user’s name should be used as the subject claim.

## 5. Configuring appsettings.json

Although the NegotiateOptionsLite class does not contain any properties, it is still recommended to update your configuration file. Adding a section for Windows authentication in your `appsettings.json` file prepares your server to recognize and apply any future custom settings. Here’s an example of the configuration:

```json  title="appsettings.json"
"Negotiate": {
  "NegotiateOptionsLite": {

  }
}
```

This configuration ensures that your identity server is aware of the settings related to the Windows authentication mechanism.

## 6. Activating Windows Authentication on the Identity Server

The final step is to integrate your authentication scheme into the identity server’s startup configuration. This is done by registering both the scheme definition and instance in the authentication store via the `AddInMemoryAuthenticationSchemes` function. Below is an example in `Program.cs` that demonstrates a complete setup, including user registration and additional configuration:

```csharp title="Program.cs"
var users = new List<User>
{
    UserBuilder.Create("administrator", "password", "Administrator")
               .SetEmail("adm@mail.com")
               .SetFirstname("Administrator")
               .Build()
};

var builder = WebApplication.CreateBuilder(args);
builder.AddSidIdentityServer()
       .AddDeveloperSigningCredential()
       .AddInMemoryUsers(users)
       .AddInMemoryLanguages(DefaultLanguages.All)
       .AddInMemoryAuthenticationSchemes(Config.AuthenticationSchemes, Config.AuthenticationSchemeDefinitions)
       .AddPwdAuthentication(true);

var app = builder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();
```

When you run your application, Windows authentication will be presented on the login window, allowing users to log in with their Windows credentials.

You can download the source code from the [SimpleIdServer Windows Authentication sample](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverWindowsauth).
