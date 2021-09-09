# Installation

A system for Cross-Domain Identity Management (SCIM2.0) can be hosted in ASP.NET CORE project:

1. Create an empty ASP.NET CORE project. 
2. Install the Nuget package `SimpleIdServer.SCIM`.
3. In the Startup.cs file, insert the following line at the end of the ConfigureServices method : `services.AddSIDScim()`.
4. In the Startup.cs file, use the function `AddAuthentication` to add authentication services and use the authentication scheme`“SCIMConstants.AuthenticationScheme` for example : 

```
services.AddAuthentication(SCIMConstants.AuthenticationScheme)
	.AddCookie(SCIMConstants.AuthenticationScheme);
``` 

5. In the Startup.cs file, use the function `AddAuthorization` to add authorization rules for example : 

```
services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());
```

6. Run the application and verify JSON is returned when you browse the following url : `https://localhost:<<sslPort>/Schemas`. 

# Configure the Schemas

Now the SCIM2.0 API is installed in the Visual Studio Solution, the SCIM Schemas must be configured. There is one Schema per Resource, it defines the list of properties present in a representation. Each property have one or more parameters like : Type, name, uniqueness etc…  

In SimpleIdServer, there are two ways to create a SCIM Schema : 

1. Load a file :
1.1. Open the project and create a subfolder “Schemas”.
1.2. Download one or more Schemas from the [URL](https://github.com/simpleidserver/SimpleIdServer/tree/master/src/Scim/SCIMSchemas) into the “Schemas” subfolder.
1.3. Open the Startup.cs file and read the file :  

```
var basePath = Path.Combine(_webHostEnvironment.ContentRootPath, “Schemas”); 
var schema = SCIMSchemaExtractor.Extract(Path.Combine(basePath, “<<SCHEMA>>.json”), SCIMConstants.Endpoints.<<SCHEMA>>, true); 
```

2. Using helper class : The helper class “SCIMSchemaBuilder” is available and it can be used to create SCIM Schema.

```
var schema = SCIMSchemaBuilder.Create("urn:dog")
	.AddStringAttribute("name") 
	.AddDateTimeAttribute("birthdate")
	.AddComplexAttribute("attributes", callback: c => 
		{ 
			c.AddStringAttribute("name"); 
			c.AddStringAttribute("value"); 
		}, multiValued: true) 
	.Build();
```