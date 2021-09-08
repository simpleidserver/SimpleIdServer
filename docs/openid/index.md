# Installation

An OPENID server with bootstrap4 theme can be hosted in ASP.NET CORE project. 
There is one Nuget package per UI theme, at the moment only Bootstrap4 library is supported: 

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new openidinmembs4 -n OpenId
```

The following files will be created : 

* *Program.cs* and *OpenIdStartup.cs* : application entry point.
* *OpenIdDefaultConfiguration.cs* : Resources of the OPENID server like : Client, Users and Scopes.
* *Views* and *Areas*.
* *Resources* : translations of the application.

In case the Visual Studio Support is needed, a solution can be created :

```
cd ..
dotnet new sln -n QuickStart
```

Add the OPENID server into the solution :

```
dotnet sln add ./src/OpenId/OpenId.csproj
```

Run the OPENID server and verify JSON is returned when you browse the following url : [https://localhost:5001/.well-known/openid-configuration](https://localhost:5001/.well-known/openid-configuration). 

```
cd src/OpenId
dotnet run
```

The JSON returned should look like to something like this :

![Well Known Configuration](images/openid-1.png)

Check if the authentication flow is working:

1. Browse the url : [https://localhost:5001/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid profile&state=state](https://localhost:5001/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid profile&state=state).
2. Authenticate with the credentials - Login : `sub`, Password : `password`.
3. Confirm the consent.
4. User agent will be redirected to the callback url `https://localhost:60001`, the authorization code is passed in the query.

# Authentication Methods

## Authentication Context Class Reference (ACR) 

The Authentication Context Class Reference (ACR) specifies a set of business rules that the authentications are being requested to satisfy. 
These rules can often be satisfied by using a number of different authentication methods, either singly or in combination.

In short, the Authentication Context Class Reference (ACR) ensures the correctness of the user identity. 
In SimpleIdServer, the ACR is similar to the Level Of Assurance. The higher your Level Of Assurance, the better the identity of a user can be trusted.  

It's up to the Relying Party to specify the ACR value. This value is passed in the "acr_values" parameter with the authorization query.

**Example** : If you want to authenticate with a Level Of Assurance equals to 2, navigate to the following URL : [https://localhost:5001/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid%20profile&state=state&acr_values=sid-load-02&prompt=login](https://localhost:5001/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid%20profile&state=state&acr_values=sid-load-02&prompt=login).
During the authentication flow, the user agent will be redirected to the `Password` and `Sms` authentication window.

The list of Authentication Context Class Reference (ACR) can be configured in the `OpenIdDefaultConfiguration.cs` file. By default, there are two ACR `sid-load-01` and `sid-load-02`.

```
public static List<AuthenticationContextClassReference> AcrLst => new List<AuthenticationContextClassReference>
{
    new AuthenticationContextClassReference
    {
        DisplayName = "First level of assurance",
        Name = "sid-load-01",
        AuthenticationMethodReferences = new List<string>
        {
            "pwd"
        }
    },
    new AuthenticationContextClassReference
    {
        DisplayName = "Second level of assurance",
        Name = "sid-load-02",
        AuthenticationMethodReferences = new List<string>
        {
            "pwd",
            "sms"
        }
    }
};
```

The list of available authentication methods are described in the next chapters.

| Authentication Method | Name                          | 
| --------------------- | ----------------------------- |
| pwd                   | Login Password Authentication |
| email                 | Email Authentication          |
| sms                   | Sms Authentication            |

## Login Password Authentication

**Authentication method** : pwd

The Login Password authentication is included in the SimpleIdServer template. There is no need to install a specific nuget package.

## Email Authentication

**Authentication method** : email

Email authentication can be configured on the OPENID server like this :

* Open a command prompt and navigate to the directory `src\OpenID`.
* Install the Nuget package `SimpleIdServer.UI.Authenticate.Email.Bootstrap4`.

```
dotnet add package SimpleIdServer.UI.Authenticate.Email.Bootstrap4
```

* Open the `OpenIdStartup.cs` and add the following code after `AddLoginPasswordAuthentication` call. Replace the `SMTPUSERNAME`, `SMTPPASSWORD`, `FROMEMAIL`, `SMTPHOST`, `SMTPPORT` with the correct values.

```
AddEmailAuthentication(opts =>
{
    opts.SmtpUserName = "<<SMTPUSERNAME>>";
    opts.SmtpPassword = "<<SMTPPASSWORD>>";
    opts.FromEmail = "<<FROMEMAIL>>";
	opts.SmtpHost = "<<SMTPHOST>>";
	opts.SmtpPort = <<SMTPPORT>>
})
```

| Property     | Description                                              | Default Value  |
| ------------ | -------------------------------------------------------- | -------------- |
| SmtpUserName | Email is used to authenticate against the SMTP server    |                |
| SmtpPassword | Password is used to authenticate against the SMTP server |                |
| FromEmail    | Sender of the email                                      |                |
| SmtpHost     | SMTP Host                                                | smtp.gmail.com |
| SmtpPort     | SMTP Port                                                | 587            |

* Edit the `OpenIdDefaultConfiguration.cs` file and add a new ACR :

```
new AuthenticationContextClassReference
{
    DisplayName = "Second level of assurance",
    Name = "sid-load-02-1",
    AuthenticationMethodReferences = new List<string>
    {
        "pwd",
        "email"
    }
}
```

* Always in the `OpenIdDefaultConfiguration.cs` file, update the EMAIL with yours :

```
new UserClaim(Jwt.Constants.UserClaims.Email, "<<EMAIL>>")
```

* Run the application

```
dotnet run
```

* Navigate to this URL [https://localhost:5001/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid%20profile&state=state&acr_values=sid-load-02-1&prompt=login](https://localhost:5001/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid%20profile&state=state&acr_values=sid-load-02-1&prompt=login)
* Submit the credentials - Login : `sub`, Password : `password`.
* Submit the confirmation code received on your email.

![Confirmation code](images/openid-4.png)


## SMS Authentication

**Authentication method** : sms

SimpleIdServer is using [Twilio](https://www.twilio.com/) to send confirmation code to phones.

SMS authentication can be configured on the OPENID server like this :

* Open a command prompt and navigate to the directory `src\OpenId`.
* Install the Nuget package `SimpleIdServer.UI.Authenticate.Sms.Bootstrap4`.

```
dotnet add package SimpleIdServer.UI.Authenticate.Sms.Bootstrap4
```

* Open the `OpenIdStartup.cs` and add the following code after `AddLoginPasswordAuthentication` call. Replace the `ACCOUNTSID`, `AUTHTOKEN` and `FROMPHONENUMBER` with the correct values, for more information refer to the [official website](https://support.twilio.com/hc/en-us/articles/223136027-Auth-Tokens-and-How-to-Change-Them). 

```
AddSMSAuthentication(opts =>
{
    opts.AccountSid = "<<ACCOUNTSID>>";
    opts.AuthToken = "<<AUTHTOKEN>>";
    opts.FromPhoneNumber = "<<FROMPHONENUMBER>>";
})
```

![Twilio Configuration](images/openid-2.png)

* Open the `OpenIdDefaultConfiguration.cs` file, and update the PHONENUMBER with yours :

```
new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumber, "<<PHONENUMBER>>")
```

* Run the application.

```
dotnet run
```

* Navigate to this URL [https://localhost:5001/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid%20profile&state=state&acr_values=sid-load-02&prompt=login](https://localhost:5001/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid%20profile&state=state&acr_values=sid-load-02&prompt=login)
* Submit the credentials - Login : `sub`, Password : `password`.
* Submit the confirmation code received on your phone.

![Confirmation code](images/openid-3.png)

# Persistence

By default, all the assets like "Clients", "Scopes", "Users" and "JSON Web Keys" are stored in memory. The following data storage can be used. 

## SQLServer

!!! note
    There is a SimpleIdServer template to create a new project with EF support, using `dotnet new openidefbs4`. 

**Pre-requisite** : OPENID server must be installed in the Visual Studio Solution.

SQL Server data storage can be configured like this : 

* In a command prompt, navigate to the directory `src\OpenId`.
* Install the Nuget package `SimpleIdServer.OpenID.EF`.

```
dotnet add package SimpleIdServer.OpenID.EF
```

* Install the Nuget package `Microsoft.EntityFrameworkCore.SqlServer`

```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```
* Install the Nuget package `Microsoft.EntityFrameworkCore.Design`.

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

* Create an `OpenIdMigration` class and replace the content with the following code. The CONNECTIONSTRING must be updated :

```
public class OpenIDMigration : IDesignTimeDbContextFactory<OpenIdDBContext>
{
    public OpenIdDBContext CreateDbContext(string[] args)
    {
        var migrationsAssembly = typeof(OpenIdStartup).GetTypeInfo().Assembly.GetName().Name;
        var builder = new DbContextOptionsBuilder<OpenIdDBContext>();
        builder.UseSqlServer("<<CONNECTIONSTRING>>", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
        return new OpenIdDBContext(builder.Options);
    }
}
```

* Execute the following command line, the migration scripts will be created

```
dotnet ef add migrations Init
```

* Execute the following command line, the tables will be created in the database :

```
dotnet ef database update`
```

* Open the `Startup.cs` file, replace any existing calls to `AddClients`, `AddAcrs`, `AddUsers`, `AddJsonWebKeys` with `AddOpenIDEF`. The CONNECTIONSTRING must be updated.

```
services
    .AddSIDOpenID()
    .AddOpenIDEF(opt => opt.UseSqlServer("<<CONNECTIONSTRING>>", o => o.MigrationsAssembly(typeof(OpenIdStartup).GetTypeInfo().Assembly.GetName().Name)))
    .AddLoginPasswordAuthentication();
```

# Protect application from undesirable users

## Recommended flow by application type

There are different grant-types to get tokens, the choice depends on the type of application.

![Choose grant-type](images/openid-5.png)

| Applications                                 | Recommended Configuration                                                    |
| -------------------------------------------- | ---------------------------------------------------------------------------- |
| Server-Side (Web application) - ASP.NET CORE | **Grant-Type** : authorization code                                          |
| Single Page Application (SPA) - Angular      | **Grant-Type** : authorization code, **Client Authentication Method** : PKCE |
| Native - Mobile, WPF application             | **Grant-Type** : authorization code, **Client Authentication Method** : PKCE |
| Trusted                                      | **Grant-Type** : password                                                    |

## Server-Side application

**Example** : ASP.NET CORE application.

Server-Side application should use *authorization code* grant-type.

!!! warning
    Before you start, Make sure there is a Visual Studio Solution with a configured OpenId server.
	
### Source Code

The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ProtectApplicationFromUndesirableUsers/AspNetCore).
 
### Configure OpenId Server

The first step consists to configure the OPENID client.

* Open the Visual Studio Solution and edit the `OpenIdDefaultConfiguration.cs` file.
* Add a new OpenId client :

```
new OpenIdClient
{
    ClientId = "website",
    ClientSecret = "websiteSecret",
    ApplicationKind = ApplicationKinds.Web,
    TokenEndPointAuthMethod = "client_secret_post",
    ApplicationType = "web",
    UpdateDateTime = DateTime.UtcNow,
    CreateDateTime = DateTime.UtcNow,
    TokenExpirationTimeInSeconds = 60 * 30,
    RefreshTokenExpirationTimeInSeconds = 60 * 30,
    TokenSignedResponseAlg = "RS256",
    IdTokenSignedResponseAlg = "RS256",
    AllowedScopes = new List<OAuthScope>
    {
        SIDOpenIdConstants.StandardScopes.OpenIdScope,
        SIDOpenIdConstants.StandardScopes.Profile,
        SIDOpenIdConstants.StandardScopes.Email
    },
    GrantTypes = new List<string>
    {
        "authorization_code",
    },
    RedirectionUrls = new List<string>
    {
        "https://localhost:7000/signin-oidc"
    },
    PreferredTokenProfile = "Bearer",
    ResponseTypes = new List<string>
    {
        "token",
        "id_token"
    }
}
```

* Run the OPENID server.

```
cd src\OpenId
dotnet run
```

### Create ASP.NET CORE application

The last step consists to create and configure an ASP.NET CORE project.

* Open a command and navigate to the `src` subfolder of your project.
* Create a directory `AspNetCore` and create an ASP.NET CORE project in it :

```
mkdir AspNetCore

dotnet new mvc -n AspNetCore
```

* Navigate to the directory `AspNetCore` and install the Nuget package `Microsoft.AspNetCore.Authentication.OpenIdConnect`.

```
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
```

* Add the `AspNetCore` project into your Visual Studio solution.

```
cd ..\..
dotnet sln add ./src/AspNetCore/AspNetCore.csproj
```

* Edit the `Startup.cs` file and configure the OpenId authentication. In the `ConfigureServices` procedure, add the following code :

```
services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "sid";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("sid", options =>
    {
        options.SignInScheme = "Cookies";

        options.Authority = "http://localhost:5000";
        options.RequireHttpsMetadata = false;

        options.ClientId = "website";
        options.SaveTokens = true;
    });
```

* To ensure the authentication services execute on each request, add `UseAuthentication` in the `Configure` procedure. The procedure should look like to something like this :

```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	app.UseHttpsRedirection();
	app.UseStaticFiles();
	
	app.UseRouting();
	
	app.UseAuthentication();
	app.UseAuthorization();
	
	app.UseEndpoints(endpoints =>
	{
		endpoints.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");
	});
}
```

* Add a `ClaimsController` with one protected operation :

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

* Create a new view `Views\Claims\Index.cshtml`. It will display all the claims of the authenticated user.

```
<ul>
    @foreach (var claim in User.Claims)
    {
        <li>@claim.Type : @claim.Value</li>
    }
</ul>
```

* In a command prompt, navigate to the directory `src\AspNetCore` and run the application under the port `7000`.

```
dotnet run --urls=https://localhost:7000
```

* Browse this URL [https://localhost:7000/claims](https://localhost:7000/claims), the User-Agent is automatically redirected to the OPENID server. 
  Submit the credentials - login : `sub`, password : `password` and confirm the consent. You'll be redirected to the following screen where your claims will be displayed.

![Claims](images/openid-6.png)