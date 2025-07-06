# Sms Authentication

In this article, we’ll walk through how to add SMS authentication to your application using the `SimpleIdServer.IdServer.Sms` NuGet package. This package leverages [Twilio](https://www.twilio.com/) to deliver SMS messages to end users, ensuring a secure and user-friendly verification process.

## Overview

SMS authentication is an effective way to enhance the security of your application by verifying user identity through a one-time password (OTP) sent via SMS. The `SimpleIdServer.IdServer.Sms` package simplifies the integration process by providing out-of-the-box support for SMS-based OTP verification. The package can be seamlessly configured to match the default authentication approach on your site.

## Installation

To begin, you need to install the NuGet package:

```bash  title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.Sms
```

This package uses Twilio’s services for sending SMS messages. Make sure you have a Twilio account ready for use.

## Configuring SMS Authentication

### 1. Update Your Configuration File

To configure the SMS authentication library, open your appsettings.json file and add the property `IdServerSmsOptions`. This configuration section includes the following parameters:

| Parameter | Description |
| --------- | ----------- |
| AccountSid | Your Twilio Account SID. For more details on obtaining this value, visit this [link](https://help.twilio.com/articles/14726256820123-What-is-a-Twilio-Account-SID-and-where-can-I-find-it-). |
| AuthToken | Your Twilio authentication token. More information on how to retrieve this token can be found [here](https://help.twilio.com/articles/223136027-Auth-Tokens-and-How-to-Change-Them). |
|  FromPhoneNumber | The phone number configured in your Twilio account from which messages will be sent. |
| Message | The SMS message template sent to users. The OTP code can be embedded using string formatting (e.g., "The confirmation code is {0}"). |
| OTPType | The type of OTP algorithm, either "TOTP" (Time-based OTP) or "HOTP" (HMAC-based OTP). |
| OTPValue | The secret key used to generate OTP codes. |
| OTPCounter | (HOTP only) A counter that increments with each new code generation. |
| HOTPWindow | (HOTP only) A window size for validating HOTP codes, allowing a range of counter values to account for desynchronization. |
| TOTPStep | (TOTP only) The time interval (in seconds) during which a TOTP code is valid (e.g., 30 seconds). |

### Example appsettings.json Configuration

```json title="appsettings.json"
"IdServerSmsOptions": {
  "AccountSid": "",
  "AuthToken": "",
  "FromPhoneNumber": "",
  "Message": "The confirmation code is {0}",
  "OTPType": "TOTP",
  "OTPValue": "PBJ777ZITHOPF7AVR7I47VRSNQYVFFNY",
  "TOTPStep": "30"
}
```

### 2. Integrating SMS Authentication in Your Application

After setting up the configuration, you need to add the SMS authentication service within your application’s startup code (typically in `Program.cs`).

```csharp  title="Program.cs"
webApplicationBuilder.Configuration.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{webApplicationBuilder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
webApplicationBuilder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryUsers(Config.Users)
    .AddInMemoryLanguages(Config.Languages)
    .AddSmsAuthentication(true);

var app = webApplicationBuilder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();
```

In the code above, the function `AddSmsAuthentication(true)` activates SMS authentication with the default settings provided on the site. The dependencies for the library are saved within Program.cs as part of the fluent API configuration.

Example of the Authentication Window:

![Authenticate](./imgs/sms.png)