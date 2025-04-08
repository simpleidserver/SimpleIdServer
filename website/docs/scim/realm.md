# Realm

A realm on a SCIM server functions as an isolated domain where users, groups, and other identities can be managed independently. This separation means that operations performed on one realm do not interfere with those on another. By partitioning identity data in this manner, developers and system administrators can tailor access control and authorization policies on a per-realm basis.

## Default Configuration

Out of the box, the SCIM server is pre-configured with two default realms:

* **idserver**
* **azuread**

Both realms are independently accessible via URLs structured as follows:

```
https://localhost:5003/<realm>/Users
```

For example, to interact with the `idserver` realm, you would use:

```
https://localhost:5003/idserver/Users
```

The independence of realms means that each realm maintains its own set of users, groups, and metadata.

## Activating Realms: No Additional NuGet Package Required

One of the convenient aspects of this SCIM implementation is that activating realms does not require the installation of an extra NuGet package. The functionality is directly built into the SCIM services.

To enable the realm functionality, you need to edit your Program.cs file. You simply invoke the `EnableRealm` method when configuring the SCIM services. Here is an example of a minimal setup:

```csharp title="Program.cs"
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScim()
    .EnableRealm();
var app = builder.Build();
app.UseScim();
app.Run();
```

By executing this configuration, the SCIM server automatically sets up the two default realms (idserver and azuread).

## Securing Realm Operations with API Keys

For security, each realm operation requires the client to pass an authentication key via the HTTP Authorization header. The default configuration includes these API keys:

| Realm | Value |
| ----- | ----- |
| idserver | ba521b3b-02f7-4a37-b03c-58f713bf88e7 |
| azuread | 1595a72a-2804-495d-8a8a-2c861e7a736a |

For instance, a typical HTTP GET request to retrieve the list of users from the idserver realm would look like this:

```
HTTP GET
Target : https://localhost:5003/idserver/Users
Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7
```

The API key corresponding to the realm must be provided as a bearer token in the HTTP headers, ensuring that only authorized requests can interact with the realm’s resources.

## Adding Custom Realms

While the default realms may suffice for many use cases, you can also configure your own realms to fit your organizational needs. 
This flexibility is achieved by first registering a realm in the store using the `AddInMemoryRealms` method. Once your realm is registered, you can define its access keys using the UpdateApiKeys method.

The following code snippet demonstrates how to configure a custom realm named test along with its corresponding API key:

```csharp title="Program.cs"
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScim().AddInMemoryRealms(new List<Realm>
{
    new Realm
    {
        Name = "test",
        Owner = "test"
    }
}).UpdateApiKeys(new ApiKeysConfiguration
{
    ApiKeys = new List<ApiKeyConfiguration>
    {
        new ApiKeyConfiguration
        {
            Owner = "test",
            Value = "accesskey",
            Scopes = ApiKeysConfiguration.AllScopes
        }
    }
}).EnableRealm();
var app = builder.Build();
app.UseScim();
app.Run();
```

In this example, the test realm is registered in memory with an owner labeled "test". An API key with the value `accesskey` is then configured to authenticate requests targeted at this realm. This approach offers granular control over identity access across different realms.

For more detailed reference, you can check out the example project available on GitHub: [SimpleIdServer Sample](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ScimRealm).