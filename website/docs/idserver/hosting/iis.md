# IIS hosting

This article outlines the process of deploying an identity server on IIS, offering a clear roadmap from publishing your application to troubleshooting common issues. 
The guide is divided into several chapters for easy navigation.

Deploying an identity server on IIS can streamline your authentication process for web applications. This guide provides detailed steps for deploying the server, ensuring your environment is correctly configured and any issues are promptly addressed. For further technical details, visiting official documentation such as [MSDN](https://learn.microsoft.com/en-us/aspnet/core/tutorials/publish-to-iis?view=aspnetcore-9.0&tabs=visual-studio) is recommended.

## Publishing the Identity Server

Begin by publishing your identity server. Open a command prompt and execute the following command:

```bash title="cmd.exe"
dotnet publish
```

Make sure to note the output path where the files are published, as this will be needed later when setting up your IIS site.

## Configuring the web.config file

If the published output does not include a `web.config` file, you need to create one. 
This configuration file is essential for directing IIS on how to handle the application. A sample `web.config` file should look similar to the following:

```xml title="web.config"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified"/>
    </handlers>
    <aspNetCore processPath="dotnet" arguments=".\IdserverIIS.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess"/>
  </system.webServer>
</configuration>
```

This configuration tells IIS to use the ASP.NET Core module to process incoming requests and correctly locate your application assembly.

## Installing the .NET CORE Hosting bundle

Ensure that the .NET Core Hosting Bundle is installed on your server. This bundle is critical for hosting ASP.NET Core applications on IIS. 
If it is not already installed, download and install it from the [provided link](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-9.0.3-windows-hosting-bundle-installer).

## Creating the IIS Site

With the application published and the necessary files in place, proceed to configure IIS:

1. Open the IIS Manager
2. Create a new website, ensuring that the physical path points to the output directory obtained during the publishing step.

This setup prepares your identity server for incoming requests.

## Troubleshooting CryptographicException Issues

During deployment, you might encounter an error related to the cryptographic library, particularly when the application attempts to execute `CngKey.Import`. 
An error message such as the following may be displayed:

```
Microsoft.AspNetCore.Server.IIS.Core.IISHttpServer[2]
Connection ID "XXXX", Request ID "XXXX": An unhandled exception was thrown by the application.
System.Security.Cryptography.CryptographicException: The system cannot find the file specified.
```

To resolve this issue:

* **Check Application Pool Settings**: Verify in the application pool properties that the option Load User Profile is enabled.
* **Verify User Permissions**: Ensure that the application pool user is a member of the Cryptographic Operators group.

These adjustments should prevent the cryptographic exception from occurring.

:::note

If the application is deployed on Azure, add the application setting WEBSITE_LOAD_PROFILE and set its value to 1.

:::

## Enabling Blazor Server Requirements on IIS

Since the identity server utilizes a Blazor library to render authentication forms and is configured to operate in Blazor Server mode, the WebSocket protocol must be enabled on IIS. 
Without WebSocket support, you may encounter unexpected exceptions, such as a `NULL HttpContext`.
Make sure that WebSocket support is installed and enabled to ensure smooth operation of your Blazor application components.

For more detailed reference, you can check out the example project available on GitHub: [SimpleIdServer Sample](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverIIS).