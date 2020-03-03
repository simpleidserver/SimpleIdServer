Configure EntityFrameworkCore in a SCIM solution
================================================

A sample project can be found `here`_.

``SimpleIdServer.Scim.Persistence.EF`` implements the required stores to use Entity Framework Core in a SCIM solution::

	dotnet add package SimpleIdServer.Scim.Persistence.EF

Configure SqlServer
^^^^^^^^^^^^^^^^^^^

Follow the steps below to install the package for database provider(s) you want to target. This walkthrough uses SQL Server :

1. Install the following package::

	dotnet add package Microsoft.EntityFrameworkCore.SqlServer

2. In the ``ConfigureServices`` method, register the specific stores and configure the database provider::

	services.AddScimStoreEF(options =>
	{
	    options.UseSqlServer(Configuration.GetConnectionString("db"), o => o.MigrationsAssembly((typeof(Startup)).Namespace));
	});

The call ``MigrationsAssembly`` is needed because the project stores the migrations.

Add migrations
^^^^^^^^^^^^^^

The Entity Framework Core CLI must be installed on your machine and the package ``Microsoft.EntityFrameworkCore.Design`` must be installed into your solution::

	dotnet tool install --global dotnet-ef
	dotnet add package Microsoft.EntityFrameworkCore.Design

Add a ``SCIMMigration`` class which implements the interface ``IDesignTimeDbContextFactory``, don't forget to update the connection string by the correct one::

	public class SCIMMigration : IDesignTimeDbContextFactory<SCIMDbContext>
	{
	    public SCIMDbContext CreateDbContext(string[] args)
	    {
	        var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
	        var builder = new DbContextOptionsBuilder<SCIMDbContext>();
	        builder.UseSqlServer("Data Source=.;Initial Catalog=SCIM;Integrated Security=True", optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
	        return new SCIMDbContext(builder.Options);
	    }
	}

Create the migrations::

	dotnet ef migrations add InitialCreate

A ``Migrations`` folder should be present, it should contains two files.

Initialize the database
^^^^^^^^^^^^^^^^^^^^^^^

The database schema should be deployed into your database provider, and the tables must be filled in with default SCIM schemas.

Add an ``InitializeDatabase`` method, and invoke it from the ``Configure`` method::

	private void InitializeDatabase(IApplicationBuilder app)
	{
	    using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
	    {
	        using (var context = scope.ServiceProvider.GetService<SCIMDbContext>())
	        {
	            context.Database.Migrate();
	            if (!context.SCIMSchemaLst.Any())
	            {
	                context.SCIMSchemaLst.Add(SCIMConstants.StandardSchemas.GroupSchema.ToModel());
	                context.SCIMSchemaLst.Add(SCIMConstants.StandardSchemas.UserSchema.ToModel());
	            }
	        }
	    }
	}

.. _here: https://github.com/simpleidserver/SimpleIdServer/tree/master/src/Scim/SimpleIdServer.Scim.SqlServer.Startup