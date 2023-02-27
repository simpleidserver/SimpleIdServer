# Protect REST.API using ASP.NET CORE

In this tutorial, we are going to explain how to protect a REST.API using ASP.NET CORE.

Prepare the solution

```
mkdir ProtectRESTApi
cd ProtectRESTApi
mkdir src
dotnet new sln -n ProtectRESTApi
```

Create a REST.API project and install the `Microsoft.AspNetCore.Authentication.JwtBearer` nuget package

```
cd src
dotnet new webapi -n ShopApi
cd ShopApi
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

Add the project into the Visual Studio solution

```
cd ..\..
dotnet sln add ./src/ShopApi/ShopApi.csproj
```