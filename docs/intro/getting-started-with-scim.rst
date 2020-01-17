How to setup a SCIM server ?
============================

A system for Cross-domain Identity Management (SCIM2.0) can be hosted in ASP.NET CORE project :

1)	Create an empty ASP.NET CORE project.

2)	Install the Nuget package **SimpleIdServer.Scim**.

3)	In the Startup.cs file, insert the following line at the end of the **ConfigureServices** method : **services.AddSIDScim()**.

4) 	In the Startup.cs file, use the function **AddAuthentication** to add authentication services and use the authentication scheme **SCIMConstants.AuthenticationScheme** for example : *services.AddAuthentication(SCIMConstants.AuthenticationScheme).AddCookie(SCIMConstants.AuthenticationScheme);*

5)  In the Startup.cs file, use the function **AddAuthorization** to add authorization rules for example : *services.AddAuthorization(opts => opts.AddDefaultSCIMAuthorizationPolicy());*.

6)	In the Startup.cs file, update the **Configure** method to configure the routing engine.

7)	Run the application and verify JSON is returned when you browse the following url : https://localhost:<sslPort>/Schemas.

By default SCIM schemas are stored in memory, default values can be overridden like this::

    services.AddSIDScim().AddSchemas();

A helper **SCIMSchemaBuilder** is exposed by the package to facilitate the creation of SCIM schema for example::

	var schema = SCIMSchemaBuilder.Create("urn:dog")
					.AddStringAttribute("name")
					.AddDateTimeAttribute("birthdate")
					.AddComplexAttribute("attributes", callback: c =>
					{
						c.AddStringAttribute("name");
						c.AddStringAttribute("value");
					}, multiValued: true)
					.Build();

SCIM settings can also be overridden by manipulating the **SCIMHostOptions** option. 
For more information please refer to the reference.