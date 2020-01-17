How to setup an OPENID server ?
===============================

An OPENID server with bootstrap4 theme can be hosted in ASP.NET CORE project. 
There is one Nuget package per UI theme, at the moment only the Bootstrap4 library is supported:

1)  Create an empty ASP.NET CORE project.

2)	Install the Nuget package **SimpleIdServer.OpenID.Bootstrap4**.

3)	Run the application and verify JSON is returned when you browse the following url : https://localhost:<sslPort>/.well-known/openid-configuration.

By default there is no authentication method, they should be installed separately by the developer. 
For example, login password authentication with Bootstrap4 theme can be installed like this:

1)	Install the Nuget package **SimpleIdServer.UI.Authenticate.LoginPassword.Bootstrap4**.

2)	In the **Startup.cs** file, import the namespace **SimpleIdServer.OpenID**.

3)	In the **Startup.cs** file, insert the following line at the end of **ConfigureServices** method : **services.AddSIDOpenID().AddLoginPasswordAuthentication()**. 

4)	Run the application and browse the URL https://localhost:<sslPort>/authorization?client_id=umaClient&redirect_uri=https://localhost:60001/signin-oidc&response_type=code&scope=openid profile&state=state.

6)	Authenticate with the credentials : Login : umaUser, Password : password

7)	Confirm the consent.

8)	User agent will be redirected to the callback url https://localhost:60001, the authorization code is passed in the query.

OPENID server is similar to OAUTH2.0 server, in fact by default clients, scopes, users and JSON Web Keys are stored in memory.
OPENID introduces the concept of **Authentication Context Class Reference** (ACR), default values can be overridden like this::

    services.AddSIDOAuth().AddAcrs();

OPENID settings can also be overridden by manipulating the **OpenIDHostOptions** option. 
For more information please refer to the reference.