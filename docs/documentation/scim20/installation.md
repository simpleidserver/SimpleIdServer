# Installation

A System for Cross-domain Identity Management (SCIM2.0) can be hosted in ASP.NET CORE project:

```
mkdir QuickStart
cd QuickStart

mkdir src
cd src

dotnet new sciminmem -n ScimHost
```

The following files will be created :

* *Program.cs* and *Startup.cs* : application entry point.
* *Schemas\UserSchema.json* and *Schemas\GroupSchema.json* : SCIM schemas coming from the [RFC 7643](https://datatracker.ietf.org/doc/html/rfc7643). Those files are used to initialize the store.

In case the Visual Studio Support is needed, a solution can be created :

```
cd ..
dotnet new sln -n QuickStart
```

Add the SCIM2.0 server into the solution :

```
dotnet sln add ./src/ScimHost/ScimHost.csproj
```

Run the SCIM server and verify JSON is returned when you browse the following url : [http://localhost:60002/Schemas](http://localhost:60002/Schemas).

```
cd src/ScimHost
dotnet run --urls=http://localhost:60002
```