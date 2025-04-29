# Email Authentication

In this article, we will walk through the process of adding email configuration to your identity server project by integrating the `SimpleIdServer.IdServer.Email` NuGet package. This package allows you to implement email-based authentication, which can be used for user registration or generating one-time codes for authentication. We'll cover installing the package, adding dependencies, configuring the library, and provide practical examples to guide you through the setup.

## Installing the NuGet Package

To begin, you need to install the `SimpleIdServer.IdServer.Email` NuGet package into your project. This package provides the tools necessary to enable email authentication functionality.

```bash  title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.Email
```

Once installed, you're ready to add the required dependencies to your project.

## Adding Dependencies with AddEmailAuthentication()

To integrate email authentication into your identity server, you need to add the necessary dependencies by calling the `AddEmailAuthentication()` method in your Program.cs file. This method is part of the fluent API provided by the identity server framework.

The `AddEmailAuthentication()` function accepts an optional boolean parameter that determines whether email authentication should be the default method for your identity server. Here’s an example:

```csharp  title="Program.cs"
webApplicationBuilder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryUsers(users)
    .AddInMemoryLanguages(Config.Languages)
    .AddEmailAuthentication(true); // Sets email as the default authentication method
```

In this case, passing `true` makes email authentication the default. If you pass `false` or omit the parameter, it will be available but not the default method.

## Configuring the Library in appsettings.json

The email authentication library is configured by editing your `appsettings.json` file. 
You’ll need to add a section named `IdServerEmailOptions` and define the following properties:

| Property | Value |
| -------- | ----- |
| SmtpPort | The SMTP port number (e.g., 587 for TLS) |
| SmtpHost | The SMTP server address (e.g., "smtp.gmail.com") |
| SmtpUserName | The username for SMTP server authentication |
| SmtpPassword | The password for the SMTP user. |
| Subject | The subject line of the email sent to users (e.g., "Confirmation code"). |
| HttpBody | The email message content, with placeholders like {0} for dynamic values such as the OTP code. |
| FromEmail | The sender’s email address. |
| SmtpEnableSsl | A boolean to enable or disable SSL for the SMTP connection. |
| OTPType | The type of OTP algorithm, either "TOTP" (Time-based OTP) or "HOTP" (HMAC-based OTP). |
| OTPValue | The secret key used to generate OTP codes. |
| OTPCounter | (HOTP only) A counter that increments with each new code generation. |
| HOTPWindow | (HOTP only) A window size for validating HOTP codes, allowing a range of counter values to account for desynchronization. |
| TOTPStep | (TOTP only) The time interval (in seconds) during which a TOTP code is valid (e.g., 30 seconds). |

These properties enable the library to send emails and generate OTP codes, which are useful for verifying a user’s email during registration or providing one-time authentication codes.

## Example of appsettings.json

Here’s an example configuration for the IdServerEmailOptions section in appsettings.json:

```json title="appsettings.json"
{
  "IdServerEmailOptions": {
    "SmtpPort": 587,
    "SmtpHost": "smtp.gmail.com",
    "SmtpUserName": "",
    "SmtpPassword": "",
    "Subject": "Confirmation code",
    "HttpBody": "The confirmation code is {0}",
    "FromEmail": "",
    "SmtpEnableSsl": true,
    "OTPType": "TOTP",
    "OTPValue": "OGFBIDG3Y42LUH7VPSWCX35HY3TS3L6T",
    "TOTPStep": "30"
  }
}
```

In this configuration:
* The SMTP server is set to Gmail’s server with port 587 and SSL enabled.
* The email subject is "Confirmation code," and the body includes a placeholder {0} for the OTP code.
* The OTP type is TOTP with a 30-second validity period.

Replace the empty fields (`SmtpUserName`, `SmtpPassword`, `FromEmail`) with your actual SMTP credentials and sender email address.

## Understanding OTP Properties

The OTP-related properties are key to generating and validating one-time passwords. Here’s a quick breakdown of the two OTP types:

* **TOTP (Time-based OTP)**: Generates a code valid for a set time period, controlled by TOTPStep. For example, a TOTPStep of 30 means the code refreshes every 30 seconds.
* **HOTP (HMAC-based OTP)**: Generates a code based on a counter (OTPCounter). The HOTPWindow defines a range of valid counter values, which helps if the client and server counters are slightly out of sync.

The library uses these OTP codes to verify email addresses or provide one-time authentication, enhancing security during user registration or login.

## Example of C# Code with Email Authentication

Below is an example of C# code that sets up email authentication in your identity server:

```csharp  title="Program.cs"
var users = new List<User>
{
    UserBuilder.Create("administrator", "password", "Administrator")
        .SetEmail("adm@mail.com")
        .SetFirstname("Administrator")
        .Build()
};

webApplicationBuilder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

webApplicationBuilder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryUsers(users)
    .AddInMemoryLanguages(Config.Languages)
    .AddEmailAuthentication(true);

var app = webApplicationBuilder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();
```