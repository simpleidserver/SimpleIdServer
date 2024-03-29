# Client-Initiated Backchannel Authentication (CIBA)

Client-Initiated Backchannel Authentication (CIBA) is an authentication flow where the client application initiates the authentication process on behalf of the end-user. 
It is designed for scenarios where the end-user may not have a browser or cannot actively participate in the authentication process.

According to the specification found at [https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html](https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html), Client-Initiated Backchannel Authentication (CIBA) is not applicable to Public Clients. Only Confidential Clients, such as Console Applications, Websites, or REST APIs, are eligible to use CIBA.

In this tutorial, we will explain how to create a Console Application that initiates a CIBA Authentication Request and obtains an Access Token by polling the `Token` endpoint.

The client will have the following configuration:

| Configuration                            | Value           |
| ---------------------------------------- | --------------- |
| Client Authentication Method             | tls_client_auth |

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/DeviceUseCIBA).
:::

To implement the CIBA in a console application, you'll need to follow the following steps.

## 1. Configure a client certificate

Utilize the administration UI to create a client certificate.

1. Open the IdentityServer website at [https://localhost:5002](https://localhost:5002).
2. In the Certificate Authorities screen, choose a Certificate Authority from the available options. Remember that the selected Certificate Authority should be trusted by your machine. You can download the certificate and import it into the appropriate Certificate Store.
3. Click on the `Client Certificates` tab and then proceed to click on the `Add Client Certificate` button.
4. Set the value of the Subject Name to `CN=client` and click on the `Add` button.
5. Click on the `Download` button located next to the certificate.

## 2. Configure an application

Utilize the administration UI to configure a new OpenID client :

1. Open the IdentityServer website at [https://localhost:5002](https://localhost:5002).
2. In the Clients screen, click on `Add client` button.
3. Select `Device` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter    | Value           |
| ------------ | --------------- |
| Identifier   | cibaConformance |
| Name         | cibaConformance |
| Subject Name | CN=client       |

## 3. Create a console application

Finally, create and configure a console application.

1. Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir DeviceUseCIBA
cd DeviceUseCIBA
mkdir src
dotnet new sln -n DeviceUseCIBA
```

2. Create a web project named `ConsoleApp` and install the `IdentityModel` NuGet package.

```
cd src
dotnet new console -n ConsoleApp
cd ConsoleApp
dotnet add package IdentityModel
```

3. Add the `ConsoleApp` project into your Visual Studio solution.

```
cd ..
dotnet sln add ./src/ConsoleApp/ConsoleApp.csproj
```

4. Edit the `Program.cs` file and copy the following code. 


```
using IdentityModel.Client;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "CN=client.pfx"));
var req = new BackchannelAuthenticationRequest()
{
    Address = "https://localhost:5001/master/mtls/bc-authorize",
    ClientId = "cibaConformance",
    Scope = "openid profile",
    LoginHint = "user",
    BindingMessage = "Message",
    RequestedExpiry = 200
};
var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
handler.CheckCertificateRevocationList = false;
handler.ClientCertificateOptions = ClientCertificateOption.Manual;
handler.SslProtocols = SslProtocols.Tls12;
handler.ClientCertificates.Add(certificate);
var client = new HttpClient(handler);
var response = await client.RequestBackchannelAuthenticationAsync(req);

bool cont = true;
while(cont)
{
    var tokenResponse = await client.RequestBackchannelAuthenticationTokenAsync(new BackchannelAuthenticationTokenRequest
    {
        Address = "https://localhost:5001/master/mtls/token",
        ClientId = "cibaConformance",
        AuthenticationRequestId = response.AuthenticationRequestId
    });
    if(tokenResponse.IsError)
        Console.WriteLine(tokenResponse.Error);
    else
    {
        Console.WriteLine(tokenResponse.AccessToken);
        cont = false;
    }
}
```

5. Replace the `CN=client.pfx` certificate with the one you have previously downloaded.

When you run the application, a green message will be displayed in the Identity Server instance.
Copy the URL from the browser and authenticate using the following credentials:

| Credential | Value    |
| ---------- | -------- |
| Login      | user     |
| Password   | password |


Once the consent is granted, the access token will be displayed by the console application.