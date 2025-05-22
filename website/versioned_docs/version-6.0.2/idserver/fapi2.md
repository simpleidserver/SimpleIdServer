# Compliant with FAPI2.0

FAPI 2.0 (Financial-grade API) is an open standard designed to enhance the security of APIs in the financial sector. It builds on OAuth 2.0 and OpenID Connect by introducing stricter security requirements such as advanced client authentication methods and mandatory upgrades to more secure TLS versions. By implementing FAPI 2.0 guidelines, financial institutions and other sensitive sectors can significantly reduce the risk of unauthorized access and data breaches.

## Building a FAPI 2.0 Compliant Identity Server

In today's digital landscape, ensuring robust security is not only recommended but essential. FAPI 2.0 provides a set of security best practices designed to protect sensitive financial data during API transactions. One critical component of meeting these standards is the proper implementation of client authentication and secure communication protocols. This article explores how to configure an identity server that complies with FAPI 2.0 security recommendations.

**Key Security Enhancements**

**Advanced Client Authentication**: FAPI 2.0 mandates the use of secure client authentication methods. In the example discussed, two methods are highlighted:

* *tls_client_auth*
* *self_signed_tls_client_auth* : These methods ensure that only trusted clients can initiate secure communications with the server, reducing the likelihood of unauthorized access.

**TLS 1.2 Upgrade**: A vital security measure in the FAPI 2.0 standard is the requirement to use TLS 1.2 or above for HTTPS connections. Upgrading to TLS 1.2 strengthens the encryption and ensures that the data in transit remains secure against modern cyber threats.

## Implementation Overview

To adhere to the FAPI 2.0 security profile, developers can utilize the `EnableFapiSecurityProfile` function from the fluent API. This single call encapsulates the necessary adjustments for both client authentication and secure connection upgrades. By integrating this function into the server setup, developers ensure that the identity server aligns with the rigorous security requirements demanded by FAPI 2.0.

Below is an example of a C# implementation of an identity server that follows FAPI 2.0 recommendations:

```csharp  title="Program.cs"
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
    .AddPwdAuthentication(true)
    .EnableFapiSecurityProfile(); // This call ensures FAPI 2.0 security compliance

var app = builder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();
```

For more detailed reference, you can check out the example project available on GitHub: [SimpleIdServer Sample](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverFapi).