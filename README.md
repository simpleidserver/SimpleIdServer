# Simpleidserver core

[![Build status](https://ci.appveyor.com/api/projects/status/shtqlxhbda6gtdag?svg=true)](https://ci.appveyor.com/project/simpleidserver/simpleidserver)

SimpleIdServer is an open source framework enabling the support of OPENID, OAUTH2.0, SCIM2.0, UMA2.0, FAPI and CIBA. It streamlines development, configuration and deployment of custom access control servers. 
Thanks to its modularity and extensibility, SimpleIdServer can be customized to the specific needs of your organization for authentication, authorization and more.

For project documentation, please visit [docs](https://simpleidserver.github.io/SimpleIdServer/).

## Packages

|                         			 						|      																															  																					|																																								|																																								|
| --------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `SimpleIdServer.OpenID` 			 						| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.OpenID.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.OpenID)												| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.OpenID.svg)](https://nuget.org/packages/SimpleIdServer.OpenID) 										| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.OpenID.svg)](https://nuget.org/packages/SimpleIdServer.OpenID) 										|
| `SimpleIdServer.OAuth`  			 						| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.OAuth.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.OAuth) 													| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.OAuth.svg)](https://nuget.org/packages/SimpleIdServer.OAuth) 											| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.OAuth.svg)](https://nuget.org/packages/SimpleIdServer.OAuth) 										|
| `SimpleIdServer.Scim`   			 						| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim) 													| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.svg)](https://nuget.org/packages/SimpleIdServer.Scim) 											| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.svg)](https://nuget.org/packages/SimpleIdServer.Scim) 											|
| `SimpleIdServer.Scim.Persistence.EF`   		 			| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.Persistence.EF.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim.Persistence.EF) 						| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.Persistence.EF.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.EF) 				| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.Persistence.EF.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.EF)				|
| `SimpleIdServer.Scim.Persistence.MongoDB`   				| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.Persistence.MongoDB.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim.Persistence.MongoDB) 			| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.Persistence.MongoDB.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.MongoDB) 	| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.Persistence.MongoDB.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.MongoDB)	|
| `SimpleIdServer.Scim.SqlServer`			   				| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.SqlServer.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim.SqlServer) 								| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.SqlServer.svg)](https://nuget.org/packages/SimpleIdServer.Scim.SqlServer) 						| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.SqlServer.svg)](https://nuget.org/packages/SimpleIdServer.Scim.SqlServer)						|
| `SimpleIdServer.Scim.Swashbuckle`			   				| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.Swashbuckle.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim.Swashbuckle) 							| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.Swashbuckle.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Swashbuckle) 					| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.Swashbuckle.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Swashbuckle)					|
| `SimpleIdServer.OpenBankingApi`							| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.OpenBankingApi.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.OpenBankingApi) 								| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.OpenBankingApi.svg)](https://nuget.org/packages/SimpleIdServer.OpenBankingApi) 						| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.OpenBankingApi.svg)](https://nuget.org/packages/SimpleIdServer.OpenBankingApi)						|
| `SimpleIdServer.OpenBankingApi.Domains`					| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.OpenBankingApi.Domains.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.OpenBankingApi.Domains) 				| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.OpenBankingApi.Domains.svg)](https://nuget.org/packages/SimpleIdServer.OpenBankingApi.Domains)		| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.OpenBankingApi.Domains.svg)](https://nuget.org/packages/SimpleIdServer.OpenBankingApi.Domains)		|
| `SimpleIdServer.Saml.Idp`									| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Saml.Idp.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Saml.Idp) 											| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Saml.Idp.svg)](https://nuget.org/packages/SimpleIdServer.Saml.Idp)									| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.SimpleIdServer.Saml.Idp.svg)](https://nuget.org/packages/SimpleIdServer.Saml.Idp)					|
| `SimpleIdServer.Saml.Sp`									| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Saml.Sp.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Saml.Sp) 												| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Saml.Sp.svg)](https://nuget.org/packages/SimpleIdServer.Sp.Idp)										| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.SimpleIdServer.Saml.Sp.svg)](https://nuget.org/packages/SimpleIdServer.Sp.Idp)						|
| `SimpleIdServer.Templates` 			 					| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Templates.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Templates)												| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Templates.svg)](https://nuget.org/packages/SimpleIdServer.Templates) 										| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Templates.svg)](https://nuget.org/packages/SimpleIdServer.Templates) 										|

## Getting started

### Install IdServer

TODO
Install the template
Create INMEMORY server

### Install website

TODO
Install the template
Execute ... to build website

### Docker

TODO

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.