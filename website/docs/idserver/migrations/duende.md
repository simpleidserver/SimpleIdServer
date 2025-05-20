# Migrating from Duende to SimpleIdServer

If you're currently using Duende IdentityServer version 7, migrating to SimpleIdServer is a smooth and straightforward process.
With the help of our migration tool, you can quickly transition your identity server without hassle.

## How to Migrate

To begin the migration, you will need to install the `SimpleIdServer.IdServer.Migrations.Duende` NuGet package on your identity server.
This package provides the necessary tools to facilitate the migration from Duende to SimpleIdServer.

```cmd title="cmd.exe"
dotnet add package SimpleIdServer.IdServer.Migrations.Duende
```

Once the NuGet package is installed, follow these steps to configure your migration:

Edit the Program.cs file; Add two essential functions:

* **EnableMigration**: This function registers the dependencies needed to execute the migration.

* **AddDuendeMigration** : This function registers the dependencies for migrating data from a Duende database to a SimpleIdServer database. It takes a lambda expression as a parameter, where you can specify the database connection string.

Here's an example of how to configure the migration:

```csharp title="Program.cs"
builder.AddSidIdentityServer()
    .EnableMigration()
    .AddDuendeMigration(a =>
    {
        a.UseSqlServer(""); // Add your connection string here
    });
```

## Running the Migration

Once the migration is set up, you can execute it in two ways:

1. **Manual Execution via Admin Portal**: You can trigger the migration manually by navigating to the `Migrations` tab in the administration portal of your identity server.

![Authenticate](./imgs/migrations.png)

2. **Automatic Execution on Server Startup**: If you want the migration to run automatically when the identity server starts, you can call the `LaunchMigration` function. This function takes a parameter, the realm, which determines where the data will be migrated. By default, this value is set to `master`.

Example :

```csharp title="Program.cs"
app.Services.LaunchMigration();
```

## Entities Migrated

The following entities will be migrated from Duende IdentityServer to SimpleIdServer:

| Duende Entity     | SimpleIdServer Entity |
| ----------------- | --------------------- |
| ApiScopes         | Scope                 |
| IdentityResources | Scope                 |
| ApiResource       | ApiResource           |
| Client            | Client                |
| Groups            | Groups                |
| Users             | Users                 |

With this migration process, your identity server's data is transferred seamlessly to SimpleIdServer, ensuring minimal disruption and a smooth transition.