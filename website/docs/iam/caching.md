# Distributed caching

All elements with a temporary lifetime, such as access, refresh, identity tokens, or temporary codes, are stored in a Distributed Cache. When they expire, they are removed from the Data Store.

By default, the distributed caching is configured to use an SQL Server database. However, it is also possible to use the NoSQL Key-Value Pair [REDIS](https://redis.io/).

## SQL Server

To use SQL Server as a distributed caching, edit the `appsettings.json` file and modify the following values :

| Json Path                                        | Value     |
| ------------------------------------------------ | --------- |
| $.DistributedCacheConfiguration.ConnectionString |           |
| $.DistributedCacheConfiguration.Type             | SQLSERVER |

## Redis

To use SQL Server as a distributed caching, edit the `appsettings.json` file and modify the following values :

| Json Path                                        | Value     |
| ------------------------------------------------ | --------- |
| $.DistributedCacheConfiguration.ConnectionString |           |
| $.DistributedCacheConfiguration.Type             | REDIS     |