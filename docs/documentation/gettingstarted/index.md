# Before you start

Make sure you have Docker installed.

# Start SimpleIdServer

Download the [docker-compose.yml](https://raw.githubusercontent.com/simpleidserver/SimpleIdServer/master/docker-compose.yml) file and execute the command line `docker-compose up`. 

This will start [SimpleIdServer](http://localhost:4200) exposed on the port 4200. It will also create an initial admin user with username `administrator` and password `password`. 

# Template

Install SimpleIdServer template :

```
dotnet new --install SimpleIdServer.Templates
```

| Command line                 | Description                                                               |
| ---------------------------- | ------------------------------------------------------------------------- |
| dotnet new openidef          | OPENID server with Entity Framework store                                 |
| dotnet new openidinmem       | OPENID server with InMemory store                                         |
| dotnet new openidemail       | OPENID server with Email authentication                                   |
| dotnet new openidsms         | OPENID server with SMS authentication                                     |
| dotnet new openidfull        | OPENID server with Email, SMS and Login password authentication           |
| dotnet new openidumafull     | UMA and OPENID server with Email, SMS and Login password authentication   |
| dotnet new scimef            | SCIM2.0 server with EF store                                              |
| dotnet new sciminmem         | SCIM2.0 server with InMemory store                                        |
| dotnet new scimongodb        | SCIM2.0 server with MongoDB store                                         |
| dotnet new scimswagger       | SCIM2.0 server with Swagger support                                       |
| dotnet new umainmem	       | UMA2.0 server with InMemory store						                   |
| dotnet new umaef             | UMA2.0 server with Entity Framework store			                       |