How to setup a UMA server ?
===========================

A UMA2.0 server with bootstrap4 theme can be hosted in ASP.NET CORE project.
There is one Nuget package per UI theme, at the moment only the Bootstrap4 library is supported :

1) Create an empty ASP.NET CORE project.

2) Install the Nuget package **SimpleIdServer.Uma.Bootstrap4**.

3) Run the application and verify JSON is returned when you browse the following url : https://localhost:<sslPort>/.well-known/oauth-authorization-server.