# Integrating SAML 2.0 into Your Identity Server

[SAML 2.0 (Security Assertion Markup Language 2.0)](https://www.oasis-open.org/standard/saml/) is an XML-based open standard that facilitates secure, cross-domain communication for exchanging authentication and authorization data. 
It is widely used in federated identity scenarios, enabling single sign-on (SSO) across various services. 
By leveraging SAML 2.0, organizations can improve security and streamline user access, reducing the need for multiple logins while maintaining robust identity verification.

## Adding SAML 2.0 Support to Your Identity Server

To enable SAML 2.0 capabilities on your identity server, you need to install the NuGet package `SimpleIdServer.IdServer.Saml.Idp`. 
This package equips your server with the necessary functionalities to act as a SAML 2.0 identity provider.

```bash  title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.Saml.Idp
```

## Configuring the SAML 2.0 Identity Provider

After installing the package, update your `Program.cs` file to call the `AddSamlIdp` function available in the fluent API. 
This function accepts a parameter that allows you to configure various properties of the `SamlIdpOptions` class. Below is a breakdown of the key properties:

| Property | Description |
| -------- | ----------- |
| SignAuthnRequest | Indicates whether the authentication requests are signed and validated |
| RevocationMode | Specifies the mode used to check for the revocation of an X509 certificate |
| CertificateValidationMode | Defines the method for validating the certificate |
| ContactPersons | A list of contact persons; this list is returned by the `/saml/metadata` endpoint |

## Deploying Your SAML 2.0 Identity Server

Once the identity server is configured to operate as a SAML 2.0 server, you can proceed to build and add a SAML client using the `ClientBuilder`. 
The following C# example demonstrates how to configure your identity server with SAML 2.0 support:

```csharp  title="Program.cs"
var users = new List<User>
{
    UserBuilder.Create("administrator", "password", "Administrator").SetEmail("adm@mail.com").SetFirstname("Administrator").Build()
};
var clients = new List<Client>
{
    SamlSpClientBuilder.BuildSamlSpClient("samlSp", "http://localhost:5125/Metadata").Build()
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
    .AddSamlIdp();

var app = builder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();
```