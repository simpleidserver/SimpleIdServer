# Distributed caching

All the elements with a temporary lifetime for examples Access/Refresh/Identity tokens or temporary codes are stored in a Distributed Cache. When they expired, they are removed from the Data Store.

By default, the distributed cahcing is configured to use an SQLServer database. But it is also possible to use the No-SQL Key Value Pair REDIS.

## SQL Server

To utilize SQL Server, edit the `appsettings.json` file and modify the following values :

| Json Path                                        | Value     |
| ------------------------------------------------ | --------- |
| $.DistributedCacheConfiguration.ConnectionString |           |
| $.DistributedCacheConfiguration.Type             | SQLSERVER |

## Redis

To utilize Redis, edit the `appsettings.json` file and modify the following values :

| Json Path                                        | Value     |
| ------------------------------------------------ | --------- |
| $.DistributedCacheConfiguration.ConnectionString |           |
| $.DistributedCacheConfiguration.Type             | REDIS     |