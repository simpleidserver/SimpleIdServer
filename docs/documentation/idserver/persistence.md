# Persistence

## Configuration data

Entity Framework Core is used to store all the configuration data (Clients, Users, Resources etc...). By default, SQLServer database is used.
The database can easily be switched by updating the `UseEFStore` function.

Use `SQLServer` 

```
UseEFStore(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("IdServer"), o =>
    {
        o.MigrationsAssembly(name);
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
})
```

Use `PostGreSQL`

```
UseEFStore(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString("IdServer"), o =>
    {
        o.MigrationsAssembly(name);
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
})
```

## Cache

Sometimes, the IdentityServer needs to store records with an expiration date for examples `Refresh Token`, `Authorization Code` or `UMA Permission Ticket`.
The `IDistributedCache` interface is used to store them. 

By default, the in memory implementation is configured.
However, if you are working in a load balancing environment with more than one IdentityServer instance. You must use a Distributed caching service like `Redis`, `NCache` or `SQL Server Cache`.
For more information, please refer to the official documentation : https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-7.0