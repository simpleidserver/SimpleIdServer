# Swagger

There is a problem when swagger is installed in a SCIM2.0 project. All the properties of SCIM representations are documented in the SCIM Schema and by default Swagger is not able to fetch them  : 

1. Install the Nuget package `SimpleIdServer.Scim.Swashbuckle`.
2. Open the “Startup.cs” file, copy and past the code below into `ConfigureService` :

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

3. Browse the URL `https://localhost:<sslPort>/swagger`, the documentation is now fetched from the SCIM Schemas. 