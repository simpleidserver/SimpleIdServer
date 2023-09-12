# Configuration

SimpleIdServer uses two different types of configurations: `static` and `dynamic`. `Static` configurations are loaded only once by the application during runtime, while `dynamic` configurations can be changed and are editable on the administrative website. 

Database and caching configurations are considered `static` and can be edited in the appsettings.json file.

However, external identity provider and identity provisioning configurations are `dynamic` and can be changed on the fly on the administrative website.

# Dynamic configuration

By default, dynamic configuration can be stored in a relational database. Other storage options, such as an In-Memory Key-Value Data Structure store like `Redis`, can be used, but the implementation does not currently exist.

# Static configuration

### IdServer

#### Cache

Here is an example of configuration to use Redis as a caching service. For more information about the different properties, refer to the [official document](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-7.0#distributed-redis-cache).

```
{
    "DistributedCacheConfiguration": {
        "Type": "REDIS",
        "ConnectionString": "",
        "InstanceName": ""
    }
}
```

Here is an example of configuration to use SQL Server as a caching service:

```
{
    "DistributedCacheConfiguration": {
        "Type": "SQLSERVER",
        "ConnectionString": ""
    }
}
```

#### Storage

Here is an example of configuration to use SQL Server as a database:

```
{
  "StorageConfiguration": {
    "ConnectionString": "",
    "Type": "SQLSERVER"
  }
}
```

Here is an example of configuration to use POSTGRE as a database:

```
{
  "StorageConfiguration": {
    "ConnectionString": "",
    "Type": "POSTGRE"
  }
}
```

### Website

#### Storage

Here is an example of configuration to use SQL Server as a database:

```
{
  "StorageConfiguration": {
    "ConnectionString": "",
    "Type": "SQLSERVER"
  }
}
```

Here is an example of configuration to use POSTGRE as a database:

```
{
  "StorageConfiguration": {
    "ConnectionString": "",
    "Type": "POSTGRE"
  }
}
```

### Scim

#### Storage

Here is an example of configuration to use SQL Server as a database:

```
{
  "StorageConfiguration": {
    "ConnectionString": "",
    "Type": "SQLSERVER"
  }
}
```

Here is an example of configuration to use POSTGRE as a database:

```
{
  "StorageConfiguration": {
    "ConnectionString": "",
    "Type": "POSTGRE"
  }
}
```

Here is an example of configuration to use MongoDB as a database:

```
{
  "StorageConfiguration": {
    "ConnectionString": "",
    "Type": "MONGODB"
  }
}
```