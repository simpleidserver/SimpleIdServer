# Implementing a minimal SCIM server

System for Cross-domain Identity Management (SCIM) 2.0 is a modern, open standard designed to simplify the management of user identities in cloud-based applications and services. 
SCIM 2.0 provides a standardized protocol for provisioning and de-provisioning user accounts, making it easier for organizations to automate user management across multiple domains. 
Its purpose is to streamline administrative tasks associated with identity lifecycle management, ensuring consistency and security through a well-defined API.

There are two primary approaches to install a SCIM server in an ASP.NET CORE project. Depending on your needs, you may opt for either a manual installation using a NuGet package or an automatic installation through a .NET template.

## Automatic Installation Using the .NET Template

For a quick start, you can generate a fully configured SCIM-ready ASP.NET CORE project using a predefined template. Simply execute the following command in your command line interface:

```batch title="cmd.exe"
dotnet new scimempty
```

This command creates a new ASP.NET CORE project that comes pre-installed with SCIM server configuration, allowing you to jump right into the development process.

## Manual Installation Using the NuGet Package

If you prefer integrating SCIM functionalities into an existing ASP.NET CORE solution, you can manually install the required NuGet package.
The package you need is `SimpleIdServer.Scim`. Here’s how you can set it up step by step:

1. **Install the NuGet Package** : Add the SimpleIdServer.Scim package to your project. You can do this either via the NuGet Package Manager in Visual Studio or by running the following command in your terminal:

```batch title="cmd.exe"
dotnet add package SimpleIdServer.Scim
```

1. **Configure the SCIM Server:** : In your ASP.NET CORE solution, locate and open the `Program.cs` file. You need to register all necessary dependencies by calling the `AddScim` method on the service container. After building the application, you can set up the routing rules by invoking the `UseScim` method. Below is a snippet that shows the essential steps:

```csharp title="Program.cs"
builder.Services.AddScim();
var app = builder.Build();
app.UseScim();
app.Run();
```

* The `AddScim` function registers the SCIM services with the ASP.NET CORE dependency injection system.
* The `UseScim` extension method is responsible for configuring the middleware, ensuring that SCIM requests are handled appropriately by the server.
* Finally, the application is started with `app.Run()`, making your SCIM API live and ready for handling identity management tasks.