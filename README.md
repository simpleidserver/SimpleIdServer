# Simpleidserver core

[![Build status](https://ci.appveyor.com/api/projects/status/shtqlxhbda6gtdag?svg=true)](https://ci.appveyor.com/project/simpleidserver/simpleidserver)

SimpleIdServer is an open source framework enabling the support of OPENID, OAUTH2.0, SCIM2.0, UMA2.0, FAPI and CIBA. It streamlines development, configuration and deployment of custom access control servers. 
Thanks to its modularity and extensibility, SimpleIdServer can be customized to the specific needs of your organization for authentication, authorization and more.

For project documentation, please visit [docs](https://simpleidserver.github.io/SimpleIdServer/).

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

## Installation via dotnet new template

Install the dotnet new template :

```
dotnet new -i SimpleIdServer.Templates
```

### Create new IdServer project

By default Entity Framework is configured to use SQLServer database.

```
dotnet new idserver --name IdServer --connectionString "<<ENTER>>"
```

### Launch IdServer project

```
cd IdServer
dotnet run --urls=http://localhost:5001
```

### Create new IdServer website project

```
dotnet new idserverwebsite --name IdServerWebsite --connectionString "<<ENTER>>"
```

### Launch IdServer website project

```
cd IdServerWebsite
dotnet run --urls=http://localhost:5002
```

## IdServer website preview

The website uses Radzen.

TODO

## IdServer preview

The website uses Bootstrap 5.

TODO

### Running via Docker

It is possible to run IdServer through the Docker.

TODO

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.