# Before you start

Make sure you have Docker installed.

# Start SimpleIdServer

Download the [docker-compose.yml](https://raw.githubusercontent.com/simpleidserver/SimpleIdServer/master/conf/docker/2.0.1/docker-compose.yml) file and execute the command line `docker-compose up`. 

This will start [SimpleIdServer](http://localhost:4200) exposed on the port 4200. It will also create an initial admin user with username `administrator` and password `password`. 

# Template

Install SimpleIdServer template :

```
dotnet new --install SimpleIdServer.Templates
```

| Command line              | Description                                                |
| ------------------------- | ---------------------------------------------------------- |
| dotnet new openidefbs4    | OPENID server with SQLServer store and Bootstrap4 theme    |
| dotnet new openidinmembs4 | OPENID server with InMemory store and Bootstrap4 theme     |
| dotnet new scimef         | SCIM2.0 server with EF store                               |
| dotnet new sciminmem      | SCIM2.0 server with InMemory store                         |
| dotnet new scimongodb     | SCIM2.0 server with MongoDB store                          |
| dotnet new scimswagger    | SCIM2.0 server with Swagger support                        |