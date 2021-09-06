# Installation

An OPENID server with bootstrap4 theme can be hosted in ASP.NET CORE project. There is one Nuget package per UI theme, at the moment only Bootstrap4 library is supported: 

1. Create an empty ASP.NET CORE project.
2. Install the Nuget package `SimpleIdServer.OpenID.Bootstrap4`.
3. Run the application and verify JSON is returned when you browse the following url : `https://localhost:<sslPort>/.well-known/openid-configuration`. 

By default there is no authentication method, they should be installed separately by the developer.  

# Authentication Methods

## Authentication Context Class Reference (ACR) 

The Authentication Context Class Reference (ACR) specifies a set of business rules that the authentications are being requested to satisfy. These rules can often be satisfied by using a number of different authentication methods, either singly or in combination.

In short, the Authentication Context Class Reference (ACR) ensures the correctness of the user identity. In SimpleIdServer, the ACR is similar to the Level Of Assurance. The higher your Level Of Assurance, the better the identity of a user can be trusted.  

Examples of ACR configurations : 

**LOA equals to 1** :

```
new AuthenticationContextClassReference 
{ 

 DisplayName = “First Level of Assurance”, 
 Name = “sid-load-01”, 
 AuthenticationMethodReferences = new List<string> 
 { 
  “pwd” 
 }
}
```

**LOA equals to 2** :

```
new AuthenticationContextClassReference 
{ 
 DisplayName = “Second Level of Assurance”, 
 Name = “sid-load-02”, 
 AuthenticationMethodReferences = new List<string> 
 { 
  “pwd”; 
  ‘sms”
 }
} 
```

It’s up to the Relying Party to specify the ACR value, the value is passed in the parameter “acr_values” inside the authorization query.


## Login Password Authentication

**Authentication method** : pwd

Login password authentication with Bootstrap4 theme can be installed like this : 

1. Install the Nuget package `SimpleIdServer.UI.Authenticate.LoginPassword.Bootstrap4`.
2. In the Startup.cs file, insert the following line at the end of ConfigureServices method : `services.AddSIDOpenID().AddLoginPasswordAuthentication()`.
3. Run the application and browse the URL : `https://localhost:<sslPort>/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid profile&state=state`.
4. Authenticate with the credentials :

| Key      | Value    |
| -------- | -------- |
| Login    | sub      |
| Password | password |

5. Confirm the consent.
6. User agent will be redirected to the callback url `https://localhost:60001`, the authorization code is passed in the query.

## Email Authentication

**Authentication method** : email

The bootstrap4 theme will come in the next release. 

## SMS Authentication

**Authentication method** : sms

The bootstrap4 theme will come in the next release. 

# Persistence

By default, all the assets like “Clients”, “Scopes”, “Users” and “JSON Web Keys” are stored in memory. The following data storage can be used. 

## SQLServer

**Pre-requisite** : OPENID server must be installed in the Visual Studio Solution.

SQL Server data storage can be configured like this : 

1. Install the Nuget package `SimpleIdServer.OpenID.EF`.
2. Install the Nuget package `Microsoft.EntityFrameworkCore.SqlServer`.
3. Install the Nuget package `Microsoft.EntityFrameworkCore.Design`. 
4. Create a class “OpenIDMigration” and replace the content with the following code. The namespace and connectionstring must be updated. 

```
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Design; 
using SimpleIdServer.OpenID.EF; 
using System.Reflection; 
namespace <<NAMESPACE>>
{ 
    public class OpenIDMigration : IDesignTimeDbContextFactory<OpenIdDBContext> 
    { 
        public OpenIdDBContext CreateDbContext(string[] args) 
        { 
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name; 
            var builder = new DbContextOptionsBuilder<OpenIdDBContext>(); 
            builder.UseSqlServer("<<CONNECTIONSTRING>>",  optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly)); 
            return new OpenIdDBContext(builder.Options); 
        } 
    } 

}
```

5. Open a command prompt, navigate to the directory of your project and execute the command line : `dotnet ef add Init`. The migration scripts will be created. 
6. Execute the command line `dotnet ef database update`. The tables will be created in the database. 
7. Open the “Startup.cs” file, inside the “ConfigureServices” procedure add the following line : 

```
services
	.AddSIDOpenID()
	.AddOpenIDEF(opt => opt.UseSqlServer(“<<CONNECTIONSTRING>>”, o => o.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name)); 
```