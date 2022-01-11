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
* *OpenIdDefaultConfiguration.cs* : OPENID server assets for example : Client, Users and Scopes.
* *Views* and *Areas*.
* *Resources* : UI translations in english and french.

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
dotnet run --urls=https://localhost:5001
```

The JSON returned should look like to something like this :

![Well Known Configuration](images/openid-1.png)

Check if the authentication flow is working:

1. Browse the [URL](https://localhost:5001/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid%20profile&state=state).
2. Authenticate with the credentials - Login : `sub`, Password : `password`.
3. Confirm the consent.
4. User agent will be redirected to the callback url `https://localhost:60001`, the authorization code is passed in the query.