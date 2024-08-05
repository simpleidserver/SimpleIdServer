# Administration website

## Nuget Packages

| Name                                 | Description                          |
| ------------------------------------ | ------------------------------------ |
| SimpleIdServer.IdServer.Website      | Administration website               |

## Quick Start

Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir AdminWebsite
cd AdminWebsite
mkdir src
dotnet new sln -n AdminWebsite
```

Create a web project named AdminWebsite and install the `SimpleIdServer.IdServer.Website` Nuget Package.

```
cd src
dotnet new blazorserver -n AdminWebsite
cd AdminWebsite
dotnet add package SimpleIdServer.IdServer.Website
```

Add the `AdminWebsite` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/AdminWebsite/AdminWebsite.csproj
```

Open the Visual Studio Solution and edit the `Program.cs` file.
Register the dependencies and configure the URL of the Identity Server.

``` 
builder.Services.AddSIDWebsite(o =>
{
    o.IdServerBaseUrl = "https://localhost:5001";
    o.IsReamEnabled = false;
});
```

Register the OPENID authentication handler.

```
builder.Services.AddDefaultSecurity(builder.Configuration);
```

Edit the AppBuilder to enable the `Authentication` and `Authorization`.

```
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(edps =>
{
    edps.MapBlazorHub();
    edps.MapFallbackToPage("/_Host");
    edps.MapControllers();
});
app.Run();
```

Edit the `appsettings.json` file, and add the OPENID security.

```
  "IdServerBaseUrl": "https://localhost:5001",
  "DefaultSecurityOptions": {
    "Issuer": "https://localhost:5001",
    "ClientId": "SIDS-manager",
    "ClientSecret": "password",
    "Scope": "openid profile",
    "IgnoreCertificateError": false
  }
```

Edit the `App.razor` page and replace the content with the following code.

```
@using SimpleIdServer.IdServer.Website
<IdServerRouter />
```

Edit the `Pages/_Host.cshtml` and relace the content with the following code.

```
@page "/"
@using Microsoft.AspNetCore.Components.Web
@namespace AdminWebsite.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="~/" />
    <link rel="stylesheet" href="_content/SimpleIdServer.IdServer.Website/css/bootstrap/bootstrap.min.css" />
    <link rel="stylesheet" href="_content/SimpleIdServer.IdServer.Website/website.css" />
    <link rel="icon" type="image/png" href="_content/SimpleIdServer.IdServer.Website/favicon.png" />
    <script src="_content/SimpleIdServer.IdServer.Website/website.js"></script>
    <script src="_content/Radzen.Blazor/Radzen.Blazor.js"></script>
    <component type="typeof(HeadOutlet)" render-mode="ServerPrerendered" />
</head>
<body class="orange">
    <component type="typeof(App)" render-mode="ServerPrerendered" />

    <div id="blazor-error-ui">
        <environment include="Staging,Production">
            An error has occurred. This application may no longer respond until reloaded.
        </environment>
        <environment include="Development">
            An unhandled exception has occurred. See browser dev tools for details.
        </environment>
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>

    <script src="_framework/blazor.server.js"></script>
</body>
</html>
```

Run the application, and navigate to the [administration website](https://localhost:5002/master/clients).

```
dotnet run --urls https://*:5002
```