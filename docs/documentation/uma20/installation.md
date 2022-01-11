# Installation

A User-Managed Access (UMA2.0) API can be hosted in ASP.NET CORE project :

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new umainmembs4  -n UmaHost
```

The following files will be created :

* *Program.cs* and *UmaStartup.cs* : application entry point.
* *UmaDefaultConfiguration.cs* : UMA Server assets for example : Uma resource, Pending request and default clients.
* *Views*
* *Resources* : UI translations in english and french.

In case the Visual Studio Support is needed, a solution can be created :

```
cd ..
dotnet new sln -n QuickStart
```

Add the UMA server into the solution :

```
dotnet sln add ./src/UmaHost/UmaHost.csproj
```

Run the UMA server and verify JSON is returned when you browse the following url: http://localhost:60003/.well-known/oauth-authorization-server

```
cd src/UmaHost
dotnet run --urls=http://localhost:60003
```

The JSON returned should look like to something like this :

![Well Known Configuration](images/uma-1.png)