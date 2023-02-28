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
| dotnet new scimef            | Create SCIM Server with EF support. By default, Entity Framework is configured to use SQLServer  |
| dotnet new scimongodb        | Create SCIM Server with MongoDB support                                                          |

## Create Visual Studio Solution

Open a command prompt, run the following commands to create the directory structure for the solution.

```
mkdir Quickstart
cd Quickstart
mkdir src
dotnet new sln -n Quickstart
```

## Create IdentityServer project

Create a web project named `IdServer` with the `SimpleIdServer.IdServer` package installed and Entity Framework (EF) configured to use SQLServer.

```
cd src
dotnet new idserver -n IdServer --connectionString "Data Source=.;Initial Catalog=IdServer;Integrated Security=True;TrustServerCertificate=True"
```

The following files will be created within a new `src/IdServer` directory :

* `IdServer.csproj` : Project file with the SimpleIdServer.IdServer nuget package added.
* `appsettings.json` : Contains the ConnectionString.
* `Program.cs` : Main application entry point.
* `IdServerConfiguration.cs` : Contains the `Clients`, `Resources`.

Next, add the `IdServer` project into the Visual Studio Solution

```
cd ..
dotnet sln add ./src/IdServer/IdServer.csproj
```

Run the `IdServer` project, it must listens on the url `http://localhost:5001`.

```
cd src/IdServer
dotnet run --urls=http://localhost:5001
```

The IdentityServer is now ready to be used. 

By default, there is one administrator account configured. It is possible to access to his profile by navigating to the url `http://localhost:5001` and authenticate with the following credentials :
* Login : administrator
* Password : password

## IdentityServer UI preview

The IdentityServer UI uses Bootstrap 5.

![IdentityServer](images/IdentityServer-1.png)

## Create IdentityServer website project

Create a web project named `IdServerWebsite` with the `SimpleIdServer.IdServer.Website` package installed and Entity Framework (EF) configured to use SQLServer.

```
cd src
dotnet new idserverwebsite -n IdServerWebsite --connectionString "Data Source=.;Initial Catalog=IdServer;Integrated Security=True;TrustServerCertificate=True"
```

Run the `IdServerWebsite` project, it must listens on the url `http://localhost:5002`.

```
cd src/IdServerWebsite
dotnet run --urls=http://localhost:5002
```

The IdentityServer website is now ready to be used.

Using the website, you can perform configurations of users and clients.

## Identity Server website UI preview

The IdentityServer website UI uses Radzen.

![IdentityServerWebsite](images/IdentityServerWebsite-2.png)

## Create SCIM project with EF support

Create a web project named `ScimEF` with the `SimpleIdServer.Scim.Persistence.EF` package installed and Entity Framework (EF) configured to use SQLServer.

```
cd src
dotnet new scimef -n ScimEF --connectionString "Data Source=.;Initial Catalog=SCIM;Integrated Security=True;TrustServerCertificate=True"
```

Next, add the `ScimEF` project into the Visual Studio Solution

```
cd ..
dotnet sln add ./src/ScimEF/ScimEF.csproj
```

Run the `SCIMEF` project, it must listens on the url `http://localhost:5003`.

```
cd src/SCIMEF
dotnet run --urls=http://localhost:5003
```

Now the SCIM server is running, you can check its Schemas endpoint on [http://localhost:5003/Schemas][http://localhost:5003/Schemas].

## Create SCIM project with MongoDB support

Create a web project named `ScimMongoDB` with the `SimpleIdServer.Scim.Persistence.MongoDB` package installed and with MongoDB support.

```
cd src
dotnet new scimongodb -n ScimMongoDB --connectionString "mongodb://localhost:27017"
```

Next, add the `ScimMongoDB` project into the Visual Studio Solution

```
cd ..
dotnet sln add ./src/ScimMongoDB/ScimMongoDB.csproj
```

Run the `ScimMongoDB` project, it must listens on the url `http://localhost:5003`.

```
cd src/ScimMongoDB
dotnet run --urls=http://localhost:5003
```

Now the SCIM server is running, you can check its Schemas endpoint on [http://localhost:5003/Schemas][http://localhost:5003/Schemas].