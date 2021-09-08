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

The bootstrap4 theme will come in the next release. 

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