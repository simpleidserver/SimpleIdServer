var builder = DistributedApplication.CreateBuilder(args);

var idserver = builder.AddProject<Projects.SimpleIdServer_IdServer_Startup>("idserver")
	.WithHttpsEndpoint(5001, name: "idserver-https")
	.WithHttpEndpoint(5000, name: "idserver-http");

var idserverui = builder.AddProject<Projects.SimpleIdServer_IdServer_Website_Startup>("idserverui");

//var scim = builder.AddProject<Projects.ScimEF>("scim");

builder.Build().Run();
