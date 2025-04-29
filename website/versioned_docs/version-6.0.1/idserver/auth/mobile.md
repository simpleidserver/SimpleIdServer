# Mobile Authentication

Connecting to the identity server through our mobile application is a streamlined process. 
To get started, you first need to install the NuGet package `SimpleIdServer.IdServer.Fido`. 
This package is essential for enabling mobile authentication features within your server environment.

```bash  title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.Fido
```

## Integrating the Package with Your Identity Server

Once the NuGet package is installed, the next step is to integrate it into your identity server. 
This is done by invoking the `AddMobileAuthentication` function from the fluent API. The function accepts two parameters:

1. **Configuration Override**: The first parameter allows you to override the default configuration settings for the [Fido2NetLib](https://github.com/passwordless-lib/fido2-net-lib) library.
2. **Default Authentication Method**: The second parameter specifies whether mobile authentication should be set as the default method for your identity server.

Below is a sample C# code snippet that demonstrates how to add mobile authentication to your server:

```csharp  title="Program.cs"
webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
webApplicationBuilder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryRealms(Config.Realms)
    .AddInMemoryUsers(Config.Users)
    .AddInMemoryLanguages(Config.Languages)
    .AddMobileAuthentication(null, true);

var app = webApplicationBuilder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();
```

## Configuring Mobile Options

The mobile authentication library is configurable via the `appsettings.json` file. 
By adding a `MobileOptions` property, you can customize specific aspects of the authentication process. One key configuration is the expiration time for the QR code, controlled by the property:

| Property | Description |
| -------- | ----------- |
| U2FExpirationTimeInSeconds | The validity duration of the QR code |

Here is an example configuration for appsettings.json:

```json title="appsettings.json"
"MobileOptions": {
    "U2FExpirationTimeInSeconds": "20"
}
```

## Mobile Application Availability

Our mobile application, designed to work as a FIDO device, is available on both iOS and Android platforms. Although it is no longer available on the App Store, users can still download it via Google Play:

[Download on Google Play](https://play.google.com/store/apps/details?id=com.simpleidserver.mobile&pcampaignid=web_share)

This application acts as a powerful authentication device, allowing users to leverage FIDO-based security measures for accessing the identity server.

## Authentication workflow

When users launch the mobile application, they are greeted by an intuitive authentication window. The first field is designated for entering the user’s login credentials. After inputting their details, users can simply click on the `generate the qr code` button to produce a QR code that facilitates the authentication process.

Below is a screenshot of the authentication interface:

![Authenticate](./imgs/mobile.png)
