# Implementing a minimal OpenID Server

In this article, we'll explore how to quickly set up an OpenID server using the `SimpleIdServer.IdServer` NuGet package in an ASP.NET Core application. This solution is perfect for developers who need a lightweight identity server implementation.

## Getting started

First, create a new ASP.NET Core project and install the required NuGet package:

```batch title="cmd.exe"
dotnet add package SimpleIdServer.IdServer
```

## Configuration

Open your `Program.cs` file and add the following code to configure the OpenID server:

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
webApplicationBuilder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryClients(clients)
    .AddInMemoryScopes(scopes);

var app = webApplicationBuilder.Build();
app.UseSid();
app.Run();
```

Let's break down what this code does:

1. We create an API scope named "api1"
2. We configure a client with ID "client" and secret "secret"
2. We add the scope to the client configuration
4. We set up the identity server with in-memory clients and scopes
5. We add a development signing credential (for testing purposes)

## Usage

With this configuration, you can now obtain access tokens using the client credentials flow. The client is configured with the "api1" scope, allowing it to request access tokens for that specific API scope using the "client_credentials" grant type.