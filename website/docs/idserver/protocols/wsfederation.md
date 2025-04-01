# Integrating WS-Federation into Your Identity Server

WS-Federation is a protocol that enables secure identity federation across disparate systems by allowing users to authenticate once and access multiple applications and services. 
Essentially, it allows identity providers to share authentication tokens with service providers, thus supporting a seamless Single Sign-On (SSO) experience even across different security realms.

## Installing the WS-Federation Package

To enable your identity server to act as a WS-Federation server, you must first install the NuGet package `SimpleIdServer.IdServer.WsFederation`. 
This package provides the necessary components to integrate WS-Federation into your server, ensuring that the proper authentication mechanisms are available.

```bash  title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.WsFederation
```

## Configuring WS-Federation

Once the package is installed, you need to register the WS-Federation authentication method. 
To do this, edit your `Program.cs` file and call the `AddWsFederation` function from the fluent API. 
This method accepts a parameter that allows you to adjust various properties of the `IdServerWsFederationOptions`. 
Key configurable properties include:

| Property | Description |
| -------- | ----------- |
| DefaultKid | Key identifier used to retrieve the key that signs the response. |
| DefaultTokenType | Specifies the SAML2.0 assertion format. The default is set to `urn:oasis:names:tc:SAML:2.0:assertion`. |
| DefaultNameIdentifierFormat | Specifies the format of the NameID property. The default is `urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified`.  |

## Creating a WS-Federation Client

After configuring WS-Federation, use the `ClientBuilder` to create a WS-Federation client. 
This client will represent a service that uses WS-Federation for authentication. An example of creating a client is shown below:

```csharp
var clients = new List<Client>
{
    WsClientBuilder.BuildWsFederationClient("urn:samplewebsite")
        .SetClientName("WsFederationClient")
        .Build()
};
```

## Complete Code Example

Below is a complete C# code example demonstrating how to set up an identity server with WS-Federation support. 
This example creates an administrator user, configures a WS-Federation client, registers the `SAMLProfile` scope, and builds the identity server.

```csharp  title="Program.cs"
var users = new List<User>
{
    UserBuilder.Create("administrator", "password", "Administrator").SetEmail("adm@mail.com").SetFirstname("Administrator").Build()
};
var clients = new List<Client>
{
    WsClientBuilder.BuildWsFederationClient("urn:samplewebsite").SetClientName("WsFederationClient").Build()
};
var scopes = new List<Scope>
{
    DefaultScopes.SAMLProfile
};
var builder = WebApplication.CreateBuilder(args);
builder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryClients(clients)
    .AddInMemoryScopes(scopes)
    .AddInMemoryUsers(users)
    .AddPwdAuthentication(true)
    .AddWsFederation();
var app = builder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();
```

For more detailed reference, you can check out the example project available on GitHub: [SimpleIdServer Sample](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverWsfederation).