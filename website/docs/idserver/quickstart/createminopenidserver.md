# Implementing a minimal OpenID Server

In this article, we'll explore how to quickly set up an OpenID server using the `SimpleIdServer.IdServer` NuGet package in an ASP.NET Core application. This solution is perfect for developers who need a lightweight identity server implementation.

## Quick Start with a Pre-Configured Template

If you prefer an even faster setup, you can generate a fully pre-configured solution by simply running the following command:

```batch title="cmd.exe"
dotnet new idserverempty
```

This command creates a new ASP.NET Core project that is already configured as a minimal OpenID server. It saves you the steps of manually installing the NuGet package and setting up the configuration.

## Getting Started Manually

For those who wish to understand the details or customize the implementation, follow these steps:

### Step 1: Create the Project and Install the Package

First, create a new ASP.NET Core project and install the required NuGet package:

```batch title="cmd.exe"
dotnet add package SimpleIdServer.IdServer
```

### Step 2: Configure the OpenID Server

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

### Code Explanation

1. **API Scope Creation**: An API scope named `api1` is created.
2. **Client Configuration**: A client is set up with the ID `client` and secret `secret`, and the API scope is added to its configuration.
2. **Server Setup**: The identity server is configured to use in-memory clients and scopes.
4. **Development Signing Credential**: A developer signing credential is added for testing purposes.

## Usage

With this configuration, you can now obtain access tokens using the client credentials flow. The client is configured with the `api1` scope, allowing it to request access tokens for that specific API scope using the `client_credentials` grant type.

