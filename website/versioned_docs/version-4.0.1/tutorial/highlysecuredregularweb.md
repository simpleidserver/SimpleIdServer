# Highly Secured regular Web Application

In this tutorial, we will provide a comprehensive explanation of the process involved in creating a highly secure regular Web Application that adheres to all the security recommendations outlined in the FAPI (Financial-grade API) specifications. The FAPI specifications can be found at [https://openid.net/specs/fapi-2_0-baseline.html#name-requirements-for-clients](https://openid.net/specs/fapi-2_0-baseline.html#name-requirements-for-clients) :

* The client shall support MTLS as mechanism for sender-constrained access tokens.
* The client shall include `request` or `request_uri` parameter as defined in Section 6 of [OIDC](https://openid.net/specs/openid-connect-core-1_0.html) in the authentication request.
* If the Authorization Request is too large for example a [Rich Authorization Request](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-rar-23), then it is recommended to use [Pushed Authorization Request (PAR)](https://datatracker.ietf.org/doc/html/rfc9126).
* [JWT-Secured OAUTH2.0 authorisation response](https://openid.net/specs/openid-financial-api-jarm.html) (JARM) is used to sign and / or encrypt the authorisation response, it protects against replay, credential leaks and mix-up attacks.
* The PS256 or ES256 algorithms must be used.

The website will be configured with the following settings:

| Configuration                            | Value           |
| ---------------------------------------- | --------------- |
| Client Authentication Method             | tls_client_auth |
| Authorization Signed Response Algorithm  | ES256           | 
| Identity Token Signed Response Algorithm | ES256           |
| Request Object Signed Response Algorithm | ES256           |
| Pushed Authorization Request             | Yes             |
| Response Mode                            | jwt             |

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/HighlySecuredServersideWebsite).
:::

To incorporate all the FAPI recommendations into your regular web application, it is necessary to follow the following steps:

## 1. Configure client certificate

Utilize the administration UI to create a client certificate.

1. Open the IdentityServer website at [https://localhost:5002](https://localhost:5002).
2. In the Certificate Authorities screen, choose a Certificate Authority from the available options. Remember that the selected Certificate Authority should be trusted by your machine. You can download the certificate and import it into the appropriate Certificate Store.
3. Click on the `Client Certificates` tab and then proceed to click on the `Add Client Certificate` button.
4. Set the value of the Subject Name to `CN=websiteFAPI` and click on the `Add` button.
5. Click on the `Download` button located next to the certificate.

## 2. Configure an application

1. Open the IdentityServer website at [https://localhost:5002](https://localhost:5002).
2. On the Clients screen, click on the `Add client` button.
3. Select `Web application` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Property                | Value                             |
| ----------------------- | --------------------------------- |
| Identifier              | protectedServersideApp            |
| Secret                  | password                          |
| Name                    | protectedServersideApp            |
| Redirection URLs        | http://localhost:7000/signin-oidc |
| Compliant with FAPI 1.0 | true                              |
| Subject Name            | CN=websiteFAPI                    |

5. The generated JSON Web Key will be displayed. Copy the corresponding value and save it in a text file. It is used by the Authorization Server to construct a signed JWT Authorization Response.

Now your client is ready to be used, you can develop the regular website.

## 3. Create ASP.NET CORE Application

Finally, create and configure an ASP.NET CORE Application.

1. Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir HighlySecuredServersideWebsite
cd HighlySecuredServersideWebsite
mkdir src
dotnet new sln -n HighlySecuredServersideWebsite
```

:::info
This NuGet Package includes support for all the features provided by the official `Microsoft.AspNetCore.Authentication.OpenIdConnect` NuGet Package.
Additionally, it introduces new features such as the `tls_client_auth` Client Authentication Method and supports new authorization response types including `jwt`, `query.jwt`, `fragment.jwt`, `form_post.jwt`, `fragment.jwt`, and `Pushed Authorization Request (PAR)`.
:::

```
cd src
dotnet new mvc -n Website
cd Website
dotnet add package SimpleIdServer.OpenIdConnect
```

2. Add the `Website` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/Website/Website.csproj
```

3. In the `Program.cs` file, make the following modifications to configure the OPENID authentication: Replace the `JWK` variable with the content of the file you copied earlier (step 2.5). Ensure that you replace the certificate `CN=websiteFAPI.pfx` with the one you downloaded earlier (step 1.5)..

| Configuration                            | Value           |
| ---------------------------------------- | --------------- |
| Client Authentication Method             | tls_client_auth |
| Authorization Signed Response Algorithm  | ES256           | 
| Identity Token Signed Response Algorithm | ES256           |
| Request Object Signed Response Algorithm | ES256           |
| Pushed Authorization Request             | Yes             |
| Response Mode                            | jwt             |

```
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

const string JWK = "{\"alg\":\"ES256\",\"crv\":\"P-256\",\"d\":\"mf1MvmivRY_TdH-J7gAt7ak4DYGnyLIqIZ3dgHL5NHk\",\"kid\":\"keyId\",\"kty\":\"EC\",\"use\":\"sig\",\"x\":\"MdwuTbn0TCQYgsER0-NeE3vtSx3H4HD9sSD7Zfkxt8k\",\"y\":\"ec27GOT5l3Mu8pzZsj6doPBNbCIp_5afjoP66qPfu4o\"}";
var jsonWebKey = JsonExtensions.DeserializeFromJson<JsonWebKey>(JWK);
var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "CN=websiteFAPI.pfx"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "sid";
})
    .AddCookie("Cookies")
    .AddCustomOpenIdConnect("sid", options =>
    {
        options.SignInScheme = "Cookies";
        options.ResponseType = "code";
        options.ResponseMode = "jwt";
        options.Authority = "https://localhost:5001/master";
        options.RequireHttpsMetadata = false;
        options.ClientId = "websiteFAPI";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
        options.MTLSCertificate = null;
        options.ClientAuthenticationType = SimpleIdServer.OpenIdConnect.ClientAuthenticationTypes.TLS_CLIENT_AUTH;
        options.RequestType = SimpleIdServer.OpenIdConnect.RequestTypes.PAR;
        options.MTLSCertificate = certificate;
        options.SigningCredentials = new SigningCredentials(jsonWebKey, jsonWebKey.Alg);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always
});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

4. Add a `ClaimsController` controller with one protected operation.

```
public class ClaimsController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }
}
```

5. Create a view `Views\Claims\Index.cshtml` with the following content. This view will display all the claims of the authenticated user.

```
<ul>
    @foreach (var claim in User.Claims)
    {
        <li>@claim.Type : @claim.Value</li>
    }
</ul>
```

6. In a command prompt, navigate to the directory `src\Website` and launch the application.

```
dotnet run --urls=http://localhost:5003
```

Finally, browse the following URL: [http://localhost:5003/claims](http://localhost:5003/claims). The User-Agent will be automatically redirected to the OpenID server.
Submit the following credentials and confirm the consent. You will be redirected to the screen where your claims will be displayed

| Credential | Value         |
| ---------- | ------------- |
| Login      | administrator |
| Password   | password      |