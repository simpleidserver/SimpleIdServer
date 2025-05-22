# Enabling Swagger

[Swagger](https://swagger.io/) is a popular framework for API documentation and testing. By integrating Swagger into your SCIM server, you can automatically generate interactive documentation for your SCIM APIs, making development, testing, and collaboration much easier.

## Required NuGet Packages

To get started, you first need to install two essential NuGet packages:

* **SimpleIdServer.Scim.SwashbuckleV6**: This package extends the SCIM functionality with Swagger support.
* **Swashbuckle.AspNetCore**: A core package that integrates Swagger into ASP.NET Core applications.

Installing these packages ensures that your project has all the necessary components to add, configure, and display Swagger documentation for your SCIM endpoints.

## Updating the Program.cs File

Once the NuGet packages have been installed, the next step is to update your `Program.cs` file to include the Swagger services and middleware. 
Below is an example of how the code should be modified:

```csharp  title="Program.cs"
using SimpleIdServer.Scim;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScim().EnableSwagger();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SCIM API V1");
});
app.UseScim();
app.Run();
```

In this configuration:

* **EnableSwagger Extension**: The `EnableSwagger` method is chained to `AddScim()`, which not only registers the SCIM services but also adds the necessary Swagger dependencies.
* **Middleware Setup**: The `UseSwagger` method adds the Swagger generator middleware, while UseSwaggerUI sets up the Swagger UI with a designated endpoint (/swagger/v1/swagger.json) labeled as "SCIM API V1".
* **SCIM Routing**: Finally, after setting up Swagger, the `UseScim` middleware is added to ensure that the SCIM endpoints are configured properly.

## Configuring the XML Documentation

To ensure that Swagger can display detailed information about your API operations, it is important to generate and copy XML documentation files. Modify your project file (csproj) to include the following target, which copies the XML documentation to your output directory:

```xml title="ScimSwagger.csproj"
	<Target Name="CopyReferenceFiles" BeforeTargets="Build">
		<ItemGroup>
			<XmlReferenceFiles Condition="Exists('$(OutputPath)%(Filename).dll')" Include="%(Reference.RelativeDir)%(Reference.Filename).xml" />
		</ItemGroup>
		<Message Text="Copying reference files to $(OutputPath)" Importance="High" />
		<Copy SourceFiles="@(XmlReferenceFiles)" DestinationFolder="$(OutputPath)" Condition="Exists('%(RootDir)%(Directory)%(Filename)%(Extension)')" />
	</Target>
```

This configuration ensures that any XML comments you include in your code are available to Swagger during runtime, enhancing the documentation of your API endpoints by providing descriptions, parameter details, and other useful metadata.

For more detailed reference, you can check out the example project available on GitHub: [SimpleIdServer Sample](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ScimSwagger).