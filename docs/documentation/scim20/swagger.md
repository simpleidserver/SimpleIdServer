# Swagger

There is a problem when swagger is installed in a SCIM2.0 project. All the properties of SCIM representations are documented in the SCIM Schema and by default Swagger is not able to fetch them.

## Source Code

The source code of this project can be found [here](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/UseSCIMSwagger)

## Installation

> [!WARNING]
> A SimpleIdServer template exists to create SCIM server with Swagger support. Execute the command line `dotnet new scimswagger -n ScimHost`.

**Pre-requisite** : [SCIM server must be installed](/documentation/scim20/installation.html) in the Visual Studio Solution.

Swagger can be configured like this : 

* In a command prompt, navigate to the directory `src\ScimHost`.
* Install the Nuget package `SimpleIdServer.Scim.Swashbuckle`. 

```
dotnet add package SimpleIdServer.Scim.Swashbuckle
```

* Install the Nuget package `Swashbuckle.AspNetCore` version `5.5.0`.

```
dotnet add package Swashbuckle.AspNetCore --version 5.5.0
```

* Edit the `Startup.cs` file and configure Swagger. Copy and paste the following code into `ConfigureService`.

```
services.AddSwaggerGen(c =>
{
    var currentAssembly = Assembly.GetExecutingAssembly();
    var xmlDocs = currentAssembly.GetReferencedAssemblies()
        .Union(new AssemblyName[] { currentAssembly.GetName() })
        .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
        .Where(f => File.Exists(f)).ToArray();
    Array.ForEach(xmlDocs, (d) =>
    {
        c.IncludeXmlComments(d);
    });
});
services.AddSCIMSwagger();
```

* Enable Swagger API and its UI. Copy and paste the following code into `Configure`.

```
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SCIM API V1");
});
```

* Run the SCIM server and check swagger portal is displayed when you browse the following url : [http://localhost:60002/swagger](http://localhost:60002/swagger).

```
cd src/ScimHost
dotnet run --urls=http://localhost:60002
```

![SCIM2.0 swagger](images/scim20-1.png)