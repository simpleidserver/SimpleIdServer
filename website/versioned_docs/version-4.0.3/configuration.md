# Configuration

## IdServer

### Cache

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

### Storage

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

## Website

### Storage

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

## Scim

### Storage

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