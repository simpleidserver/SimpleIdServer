How to setup an OAUTH2.0 server ?
=================================

An OAUTH2.0 server can be hosted in ASP.NET CORE project by following the following steps:

1)	Create an empty ASP.NET CORE project.

2)	Install the Nuget package **SimpleIdServer.OAuth**.

3)	In the Startup.cs file, insert the following line at the end on the **ConfigureServices** method : **services.AddSIDOAuth()**.

4)	In the Startup.cs file, insert the following line at the end of the **Configure** method : **app.UseSID()**.

5)	Run the application and verify JSON is returned when you browse the following url : https://localhost:<sslPort>/.well-known/oauth-authorization-server. The application URL and SSL port are defined in the launchSettings.json file located in your application Properties folder.

By default OAUTH2.0 clients, scopes and JSON Web keys are stored in memory. Default values can be overridden by one of the operations exposed by **services.AddSIDOAuth()** function.
For example: default OAUTH2.0 clients can be overridden like this::

    services.AddSIDOAuth().AddClients();

OAUTH2.0 settings can also be overridden by manipulating the **OAuthHostOptions** option. For more information please refer to the reference.