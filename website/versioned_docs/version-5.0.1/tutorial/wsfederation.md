# Regular Web Application (WS-Federation)

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/WsFederationWebsite).
:::

To implement WS-Federation in a regular web application, you'll need to follow the following steps.

## 1. Configure an application

Utilize the administration UI to configure a new WS-Federation client :

1. Open the IdentityServer website at [https://localhost:5002/master/clients](https://localhost:5002/master/clients).
2. On the Clients screen, click on the `Add client` button.
3. Select `WS-Fed Relying Party` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter        | Value                              |
| ---------------- | ---------------------------------- |
| Identifier       | urn:samplewebsite                  |
| Name             | samplewebsite                      |

Now your client is ready to be used, you can develop the regular website.

## 2. Create ASP.NET CORE Application

Finally, create and configure an ASP.NET CORE Application.

1. Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir WsFederationWebsite
cd WsFederationWebsite
mkdir src
dotnet new sln -n WsFederationWebsite
```

2. Create a web project named `Website` and install the `Microsoft.AspNetCore.Authentication.WsFederation` NuGet package.

```
cd src
dotnet new mvc -n Website
cd Website
dotnet add package Microsoft.AspNetCore.Authentication.WsFederation
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
    .AddWsFederation("sid", options =>
    {
        options.Wtrealm = "urn:samplewebsite";
        options.MetadataAddress = "https://localhost:5001/master/FederationMetadata/2007-06/FederationMetadata.xml";
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