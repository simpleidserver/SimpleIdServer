# Load balancing

Load balancing is a method used to distribute incoming network traffic across multiple servers. 
This ensures that no single server becomes overwhelmed, leading to better reliability, improved performance, and high availability. 
When deploying an Identity Server behind a load balancer, special considerations must be taken to ensure consistent behavior across all server instances.

The following configuration steps are essential for running the Identity Server in a load-balanced environment.

## 1. Persist Cryptographic Keys

The cryptographic keys used for encrypting/decrypting cookies and session storage data must be **persisted**. 
This is crucial to ensure that all instances of the server can access the same keys, allowing users to seamlessly interact with different server nodes.

To persist these keys, you should use the `UseEfStore` method from the fluent API.

Here is an example in C#, where cryptographic keys are stored in a SQL Server database:

```csharp  title="Program.cs"
const string connectionString = "";
...
.AddDeveloperSigningCredential()
.UseEfStore(e =>
{
    e.UseSqlServer(connectionString, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.SqlServerMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
}, e =>
{
    e.UseSqlServer(connectionString, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.IdServer.SqlServerMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});
```

## 2. Configure a Persistent Distributed Cache

The distributed cache should not rely on in-memory storage. It must be persistent and accessible by all Identity Server instances. 
This cache is used to store:
* The state of the user registration process.
* Temporary codes such as OTPs or refresh tokens with a limited lifespan.

Below is an example of using SQL Server as a distributed cache:

```csharp  title="Program.cs"
services.AddDistributedSqlServerCache(opts =>
{
    opts.ConnectionString = conf.ConnectionString;
    opts.SchemaName = "dbo";
    opts.TableName = "DistributedCache";
});
```

By correctly configuring both cryptographic key storage and the distributed cache, you ensure that your Identity Server behaves consistently and securely in a load-balanced setup.