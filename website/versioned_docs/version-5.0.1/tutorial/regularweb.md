# Regular Web Application

To protect a regular web application, the recommended OpenID grant type is the **Authorization Code Flow**. This grant type provides a secure and robust mechanism for authentication and authorization.

Here are the reasons why the Authorization Code Flow is suitable for protecting a regular web application:

1. **Secure Token Exchange**: The Authorization Code Flow ensures that sensitive tokens, such as access tokens and refresh tokens, are exchanged securely between the web application and the OpenID provider's token endpoint. This flow mitigates the risk of token exposure since tokens are exchanged through server-side communication.
2. **Enhanced Security**: The Authorization Code Flow involves a server-to-server token exchange, which reduces the exposure of access tokens in the browser and helps protect against cross-site scripting (XSS) attacks.
3. **Refresh Token Support**: This flow allows the web application to obtain a refresh token, which can be used to obtain a new access token without requiring user interaction. This feature improves user experience by reducing the need for frequent re-authentication.
4. **Compliance with Standards**: The Authorization Code Flow is a widely adopted and standardized OAuth 2.0 flow. It ensures compatibility and interoperability across different OpenID providers and client applications.

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ProtectWebsiteServerside).
:::

To implement the Authorization Code Flow in a regular web application, you'll need to follow the following steps.

## 1. Configure an application

Utilize the administration UI to configure a new OpenID client :

1. Open the IdentityServer website at [https://localhost:5002/master/clients](https://localhost:5002/master/clients).
2. On the Clients screen, click on the `Add client` button.
3. Select `web application` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter        | Value                              |
| ---------------- | ---------------------------------- |
| Identifier       | protectedServersideApp             |
| Secret           | password                           |
| Name             | protectedServersideApp             |
| Redirection URLS | http://localhost:7000/signin-oidc  |

Now your client is ready to be used, you can develop the regular website.

## 2. Create ASP.NET CORE Application

Finally, create and configure an ASP.NET CORE Application.

1. Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir ProtectWebsiteServerside
cd ProtectWebsiteServerside
mkdir src
dotnet new sln -n ProtectWebsiteServerside
```

2. Create a web project named `Website` and install the `Microsoft.AspNetCore.Authentication.OpenIdConnect` NuGet package.

```
cd src
dotnet new mvc -n Website
cd Website
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
```

3. Add the `Website` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/Website/Website.csproj
```

4. Edit the `Program.cs` file and configure the OpenID authentication. 

```
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "sid";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("sid", options =>
    {
        options.SignInScheme = "Cookies";
        options.ResponseType = "code";
        options.Authority = "https://localhost:5001/master";
        options.RequireHttpsMetadata = false;
        options.ClientId = "protectedServersideApp";
        options.ClientSecret = "password";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
    });
...
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always
});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
...
```

5. Add a `ClaimsController` controller with one protected operation.

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

6. Create a view `Views\Claims\Index.cshtml` with the following content. This view will display all the claims of the authenticated user.

```
<ul>
    @foreach (var claim in User.Claims)
    {
        <li>@claim.Type : @claim.Value</li>
    }
</ul>
```

7. In a command prompt, navigate to the `src\Website` directory and launch the application.

```
dotnet run --urls=http://localhost:7000
```

Finally, browse the following URL: [http://localhost:7000/claims](http://localhost:7000/claims). The User-Agent will be automatically redirected to the OpenID server.
Submit the following credentials and confirm the consent. You will be redirected to the screen where your claims will be displayed

| Credential | Value         |
| ---------- | ------------- |
| Login      | administrator |
| Password   | password      |