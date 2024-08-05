# Regular Web Application (SAML2.0)

:::info
The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/SamlRpWebsite).
:::

To implement SAML2.0 in a regular web application, you'll need to follow the following steps.

## 1. Configure an application

Utilize the administration UI to configure a new SAML2.0 SP (Service Provider) client :

1. Open the IdentityServer website at [https://localhost:5002/master/clients](https://localhost:5002/master/clients).
2. On the Clients screen, click on the `Add client` button.
3. Select `SAML SP` and click on next.
4. Fill-in the form like this and click on the `Save` button to confirm the creation.

| Parameter        | Value                              |
| ---------------- | ---------------------------------- |
| Identifier       | samlSp                             |
| Name             | samlSp                             |
| Metadata URL     | http://localhost:5125/Metadata     |

If the checkbox `Use Artifact` is checked, then the Artifact binding will be used; if it is not checked, the Post Binding is used by default.

The Public and Private keys are displayed, keep those values into a file, they will be used later during the configuration of the website.

The Public and Private keys are displayed; please save these values in a file as they will be used later during the website configuration.

## 2. Create ASP.NET CORE Application

Finally, create and configure an ASP.NET CORE Application.

1. Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir SamlRpWebsite
cd SamlRpWebsite
mkdir src
dotnet new sln -n SamlRpWebsite
```

2. Create a web project named `Website` and install the `SimpleIdServer.IdServer.Saml.Sp` NuGet package.

```
cd src
dotnet new mvc -n Website
cd Website
dotnet add package SimpleIdServer.IdServer.Saml.Sp
```

3. Add the `Website` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/Website/Website.csproj
```

4. Edit the `Program.cs` file and configure the SAML2.0 authentication. 

```
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "samlSp";
})
    .AddCookie("Cookies")
    .AddSamlSp("samlSp", options =>
    {
        options.SPId = "samlSp";
        options.IdpMetadataUrl = "https://localhost:5001/master/saml/metadata";
        var currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        options.SigningCertificate = X509Certificate2.CreateFromPemFile(Path.Combine(currentPath, "sidClient.crt"), Path.Combine(currentPath, "sidClient.key"));
    });
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

7. "Copy the `Private Key` obtained from the first step into the `sidClient.key` file, and the `Public Key` into the `sidClient.crt` file.

8. In a command prompt, navigate to the `src\Website` directory and launch the application.

```
dotnet run --urls=http://localhost:5125
```

Finally, browse the following URL: [http://localhost:5125/claims](http://localhost:5125/claims). The User-Agent will be automatically redirected to the OpenID server.
Submit the following credentials and confirm the consent. You will be redirected to the screen where your claims will be displayed

| Credential | Value         |
| ---------- | ------------- |
| Login      | administrator |
| Password   | password      |