# Realm

In modern identity management, flexibility and isolation are key. The realm concept addresses these needs by allowing multiple independent workspaces to operate on the same identity server. This approach is particularly useful when managing environments such as TEST, VAL, and PRD on a single server instance.

## What is a Realm?

A realm represents an isolated workspace within an identity server. Each realm can be configured independently, meaning it has its own set of clients, users, and groups. This isolation ensures that environments can operate without interfering with one another, which is especially beneficial in development and production settings.

## Key Features of a Realm

* **Isolation**: Each realm maintains its own configuration and security policies.
* **Flexibility**: Different environments (such as TEST, VAL, and PRD) can be managed under one server instance.
* **Scalability**: Organizations can easily scale and manage various environments without the overhead of multiple identity servers.

## Enabling the Realm Feature

To activate the realm functionality, developers can use the `EnableRealm ` function provided in the fluent API. This function configures the identity server to support multiple independent workspaces, making it straightforward to manage separate environments on a single server.

## Code Example in C#

Below is an example of a C# program that demonstrates how to configure an identity server with realm support enabled. The code sets up a simple identity server with a single API scope, a client, and the realm functionality activated.

```csharp  title="Program.cs"
var scope = ScopeBuilder.CreateApiScope("api1", false).Build();
var clients = new List<Client>
{
    ClientBuilder.BuildApiClient("client", "secret").AddScope(scope).Build()
};
var scopes = new List<Scope>
{
    scope
};
builder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryClients(clients)
    .AddInMemoryScopes(scopes)
    .EnableRealm();

var app = builder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();
```

For more detailed reference, you can check out the example project available on GitHub: [SimpleIdServer Sample](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverRealm).