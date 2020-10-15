# Simpleidserver core

[![Build status](https://ci.appveyor.com/api/projects/status/shtqlxhbda6gtdag?svg=true)](https://ci.appveyor.com/project/simpleidserver/simpleidserver)
[![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.OpenID.svg)](http://myget.org/gallery/advance-ict)
[![Documentation Status](https://readthedocs.org/projects/simpleidserver/badge/?version=latest)](https://simpleidserver.readthedocs.io/en/latest/)

SimpleIdServer is an open source framework enabling the support of OPENID, OAUTH2.0, SCIM2.0 and UMA2.0. It streamlines development, configuration and deployment of custom access control servers. 
Thanks to its modularity and extensibility, SimpleIdServer can be customized to the specific needs of your organization for authentication, authorization and more.

For project documentation, please visit [readthedocs](https://simpleidserver.readthedocs.io/en/latest/).

## Packages

|                         			 						|      																															  																					|																																								|																																								|
| --------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `SimpleIdServer.OpenID` 			 						| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.OpenID.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.OpenID)												| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.OpenID.svg)](https://nuget.org/packages/SimpleIdServer.OpenID) 										| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.OpenID.svg)](https://nuget.org/packages/SimpleIdServer.OpenID) 										|
| `SimpleIdServer.OpenID.Bootstrap4` 						| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.OpenID.Bootstrap4.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.OpenID.Bootstrap4)							| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.OpenID.Bootstrap4.svg)](https://nuget.org/packages/SimpleIdServer.OpenID.Bootstrap4) 					| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.OpenID.Bootstrap4.svg)](https://nuget.org/packages/SimpleIdServer.OpenID.Bootstrap4) 				|
| `SimpleIdServer.UI.Authenticate.LoginPassword.Bootstrap4` | [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.UI.Authenticate.LoginPassword.Bootstrap4.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.OpenID.Bootstrap4)	| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.OpenID.Bootstrap4.svg)](https://nuget.org/packages/SimpleIdServer.OpenID.Bootstrap4) 					| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.OpenID.Bootstrap4.svg)](https://nuget.org/packages/SimpleIdServer.OpenID.Bootstrap4) 				|
| `SimpleIdServer.OAuth`  			 						| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.OAuth.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.OAuth) 													| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.OAuth.svg)](https://nuget.org/packages/SimpleIdServer.OAuth) 											| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.OAuth.svg)](https://nuget.org/packages/SimpleIdServer.OAuth) 										|
| `SimpleIdServer.Scim`   			 						| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim) 													| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.svg)](https://nuget.org/packages/SimpleIdServer.Scim) 											| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.svg)](https://nuget.org/packages/SimpleIdServer.Scim) 											|
| `SimpleIdServer.Scim.Persistence.EF`   		 			| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.Persistence.EF.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim.Persistence.EF) 						| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.Persistence.EF.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.EF) 				| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.Persistence.EF.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.EF)				|
| `SimpleIdServer.Scim.Persistence.MongoDB`   				| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.Persistence.MongoDB.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim.Persistence.MongoDB) 			| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.Persistence.MongoDB.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.MongoDB) 	| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.Persistence.MongoDB.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Persistence.MongoDB)	|
| `SimpleIdServer.Scim.SqlServer`			   				| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.SqlServer.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim.SqlServer) 								| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.SqlServer.svg)](https://nuget.org/packages/SimpleIdServer.Scim.SqlServer) 						| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.SqlServer.svg)](https://nuget.org/packages/SimpleIdServer.Scim.SqlServer)						|
| `SimpleIdServer.Scim.Swashbuckle`			   				| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Scim.Swashbuckle.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Scim.Swashbuckle) 							| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Scim.Swashbuckle.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Swashbuckle) 					| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Scim.Swashbuckle.svg)](https://nuget.org/packages/SimpleIdServer.Scim.Swashbuckle)					|
| `SimpleIdServer.Uma.Bootstrap4`   			 			| [![MyGet (dev)](https://img.shields.io/myget/advance-ict/v/SimpleIdServer.Uma.Bootstrap4.svg)](https://www.myget.org/feed/advance-ict/package/nuget/SimpleIdServer.Uma.Bootstrap4) 								| [![NuGet](https://img.shields.io/nuget/v/SimpleIdServer.Uma.Bootstrap4.svg)](https://nuget.org/packages/SimpleIdServer.Uma.Bootstrap4) 						| [![NuGet](https://img.shields.io/nuget/dt/SimpleIdServer.Uma.Bootstrap4.svg)](https://nuget.org/packages/SimpleIdServer.Uma.Bootstrap4)						|

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Live demo

Live demo : [http://simpleidserver.northeurope.cloudapp.azure.com/simpleidserver](http://simpleidserver.northeurope.cloudapp.azure.com/simpleidserver/).

Administrator credentials :

| Property      |      Value      |
|---------------|-----------------|
| login         | administrator   |
| value         | password        |