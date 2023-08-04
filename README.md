# Simpleidserver core

<img src="images/logo.svg" alt="SimpleIdServer" style="width:200px;"/>

[![Build status](https://ci.appveyor.com/api/projects/status/shtqlxhbda6gtdag?svg=true)](https://ci.appveyor.com/project/simpleidserver/simpleidserver)
[![Join the chat at https://app.gitter.im/#/room/#simpleidserver:gitter.im](https://badges.gitter.im/repo.svg)](https://app.gitter.im/#/room/#simpleidserver:gitter.im)

SimpleIdServer is an open source framework enabling the support of OPENID, OAUTH2.0, SCIM2.0, UMA2.0, FAPI and CIBA. It streamlines development, configuration and deployment of custom access control servers. 
Thanks to its modularity and extensibility, SimpleIdServer can be customized to the specific needs of your organization for authentication, authorization and more.

[Website](http://simpleidserver.com), [Documentation](https://simpleidserver.com/docs/intro) and [Demo](https://website.simpleidserver.com/).

## Packages

|                         			 						|																																								|																																								|
| --------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `SimpleIdServer.IdServer` 			 					| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.IdServer.svg)](https://nuget.org/packages/SimpleIdServer.IdServer) 									| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.IdServer.svg)](https://nuget.org/packages/SimpleIdServer.IdServer)									|
| `SimpleIdServer.IdServer.Email` 			 				| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.IdServer.Email.svg)](https://nuget.org/packages/SimpleIdServer.IdServer.Email) 						| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.IdServer.Email.svg)](https://nuget.org/packages/SimpleIdServer.IdServer.Email) 						|
| `SimpleIdServer.IdServer.Sms` 			 				| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.IdServer.Sms.svg)](https://nuget.org/packages/SimpleIdServer.IdServer.Sms) 							| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.IdServer.Sms.svg)](https://nuget.org/packages/SimpleIdServer.IdServer.Sms) 							|
| `SimpleIdServer.IdServer.WsFederation`	 				| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.IdServer.WsFederation.svg)](https://nuget.org/packages/SimpleIdServer.IdServer.WsFederation) 			| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.IdServer.WsFederation.svg)](https://nuget.org/packages/SimpleIdServer.IdServer.WsFederation) 		|
| `SimpleIdServer.Templates` 			 					| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Templates.svg)](https://nuget.org/packages/SimpleIdServer.Templates) 									| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Templates.svg)](https://nuget.org/packages/SimpleIdServer.Templates) 								|
| `SimpleIdServer.Scim` 			 						| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.svg)](https://nuget.org/packages/SimpleIdServer.Scim) 											| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.svg)](https://nuget.org/packages/SimpleIdServer.Scim) 											|
| `SimpleIdServer.Scim.Persistence.EF` 			 			| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.Persistence.EF.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.EF) 				| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.Persistence.EF.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.EF) 			|
| `SimpleIdServer.Scim.Persistence.MongoDB`		 			| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.Persistence.MongoDB.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.MongoDB) 	| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.Persistence.MongoDB.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.MongoDB) 	|
| `SimpleIdServer.Scim.Client`		 						| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.Client.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Client) 								| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.Client.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Client) 							|
| `SimpleIdServer.OpenIdConnect`         	 				| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.OpenIdConnect.svg)](https://nuget.org/packages/SimpleIdServer.OpenIdConnect) 	                 		| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.OpenIdConnect.svg)](https://nuget.org/packages/SimpleIdServer.OpenIdConnect) 		                |

# Preparation

Install SimpleIdServer templates.

```
dotnet new --install SimpleIdServer.Templates
```

This will add the following templates

| Command line                 | Description                                                                                      |
| ---------------------------- | ------------------------------------------------------------------------------------------------ |
| dotnet new idserver          | Create Identity Server. By default, Entity Framework is configured to use SQLServer              |
| dotnet new idserverwebsite   | Create Identity Server website. By default, Entity Framework is configured to use SQLServer      |
| dotnet new scim              | Create SCIM Server.                                                                              |

## Create Visual Studio Solution

Open a command prompt and execute the following commands to create the directory structure for the solution.

```
mkdir Quickstart
cd Quickstart
mkdir src
dotnet new sln -n Quickstart
```

## Create IdentityServer project

To create a web project named `IdServer` with the `SimpleIdServer.IdServer` package installed and Entity Framework (EF) configured to use SQLServer, execute the command line :

```
cd src
dotnet new idserver -n IdServer --storageConnectionString "Data Source=.;Initial Catalog=IdServer;Integrated Security=True;TrustServerCertificate=True" --distributedCacheConnectionString "Data Source=.;Initial Catalog=IdServer;Integrated Security=True;TrustServerCertificate=True"
```

The following files will be created within a new `src/IdServer` directory :

* `IdServer.csproj` : Project file with the `SimpleIdServer.IdServer` NuGet package added.
* `appsettings.json` : Contains the ConnectionString.
* `Program.cs` : Main application entry point.
* `IdServerConfiguration.cs` : Contains the `Clients`, `Resources`.

Next, add the `IdServer` project into the Visual Studio Solution

```
cd ..
dotnet sln add ./src/IdServer/IdServer.csproj
```

Run the IdServer project, ensuring that it listens on the URL `https://localhost:5001`.

```
cd src/IdServer
dotnet run --urls=https://localhost:5001
```

The IdentityServer is now ready to be used. 

By default, there is one administrator account configured. You can access their profile by navigating to the URL `https://localhost:5001/master` and authenticate using the following credentials :

* Login : administrator
* Password : password

## IdentityServer UI preview

The IdentityServer UI uses Bootstrap 5.

![IdentityServer](./images/IdentityServer-1.png)

## Create IdentityServer website project

create a web project named `IdServerWebsite` with the `SimpleIdServer.IdServer.Website` package installed and Entity Framework (EF) configured to use SQLServer, execute the command line :

```
cd src
dotnet new idserverwebsite -n IdServerWebsite --storageConnectionString "Data Source=.;Initial Catalog=IdServer;Integrated Security=True;TrustServerCertificate=True"
```

Run the `IdServerWebsite` project, it must listens on the url `https://localhost:5002`.

```
cd src/IdServerWebsite
dotnet run --urls=https://localhost:5002
```

The IdentityServer website is now ready to be used.

The website can be used to manage all aspects of an Identity Server solution, such as managing clients, users, and scopes.

## Identity Server website UI preview

The IdentityServer website UI uses Radzen.

![IdentityServerWebsite](./images/IdentityServerWebsite-2.png)

## SCIM Security

By default SCIM is configured to use API KEY authentication.
For clients to perform any operation, they must include one of those keys in the `HTTP HEADER Authorization Bearer` field.

| Owner    | Value                                |
| -------- | ------------------------------------ |
| IdServer | ba521b3b-02f7-4a37-b03c-58f713bf88e7 |
| AzureAd  | 1595a72a-2804-495d-8a8a-2c861e7a736a |

## Create SCIM project with EF support

Create a web project named `ScimEF` with the `SimpleIdServer.Scim.Persistence.EF` package installed and Entity Framework (EF) configured to use SQLServer, execute the command line :

```
cd src
dotnet new scim -n ScimEF --connectionString "Data Source=.;Initial Catalog=SCIM;Integrated Security=True;TrustServerCertificate=True" -t "SQLSERVER"
```

Next, add the `ScimEF` project into the Visual Studio Solution

```
cd ..
dotnet sln add ./src/ScimEF/ScimEF.csproj
```

Run the ScimEF project, ensuring that it listens on the URL `https://localhost:5003`.

```
cd src/SCIMEF
dotnet run --urls=https://localhost:5003
```

Now that the SCIM server is running, you can check its Schemas endpoint by accessing [https://localhost:5003/Schemas](https://localhost:5003/Schemas).

## Create SCIM project with MongoDB support


To create a web project named ScimMongoDB with the SimpleIdServer.Scim.Persistence.MongoDB package installed and MongoDB support, execute the command line :

```
cd src
dotnet new scim -n ScimMongoDB --connectionString "mongodb://localhost:27017" -t "MONGODB"
```

Next, add the `ScimMongoDB` project into the Visual Studio Solution

```
cd ..
dotnet sln add ./src/ScimMongoDB/ScimMongoDB.csproj
```

Run the ScimMongoDB project, ensuring that it listens on the URL `https://localhost:5003`.

```
cd src/ScimMongoDB
dotnet run --urls=https://localhost:5003
```

Now that the SCIM server is running, you can check its Schemas endpoint by accessing [https://localhost:5003/Schemas](https://localhost:5003/Schemas).

# Running with docker

To execute all the projects in Docker, execute the following commands :

```
psake dockerBuild
psake dockerUp
```

# Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

# Contact us

To contact the team, you can send an email to `agentsimpleidserver@gmail.com` or share your ideas in gitter.im.
The invitation link is https://app.gitter.im/#/room/#simpleidserver:gitter.im
