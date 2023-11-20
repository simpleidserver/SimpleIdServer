# Distributed configuration

Different modules within SimpleIdServer utilize a distributed configuration storage to store their settings, including the Authentication and Provisioning modules.

By default, SQL Server is used to store the configuration, but you also have the option to use the NoSQL Key-Value data store [Redis](https://redis.io/).

## Redis

To use Redis as a distributed configuration storage, edit the `appsettings.json` file and modify the following values :

| Json Path                                        | Value         |
| ------------------------------------------------ | ------------- |
| $.DistributedCacheConfiguration.Type             | REDIS         |
| $.DistributedCacheConfiguration.ConnectionString |               | 

## SQLServer

To use SQLServer as a distributed configuration storage, edit the  `appsettings.json` file and modify the following values :

| Json Path                                        | Value         |
| ------------------------------------------------ | ------------- |
| $.DistributedCacheConfiguration.Type             | SQLSERVER     |
| $.DistributedCacheConfiguration.ConnectionString |               |

## Postgre

To use Postgresql as a distributed configuration storage, edit the  `appsettings.json` file and modify the following values :

| Json Path                                        | Value         |
| ------------------------------------------------ | ------------- |
| $.DistributedCacheConfiguration.Type             | POSTGRE       |
| $.DistributedCacheConfiguration.ConnectionString |               |

## Mysql

To use Mysql as a distributed configuration storage, edit the  `appsettings.json` file and modify the following values :

| Json Path                                        | Value         |
| ------------------------------------------------ | ------------- |
| $.DistributedCacheConfiguration.Type             | MYSQL         |
| $.DistributedCacheConfiguration.ConnectionString |               |