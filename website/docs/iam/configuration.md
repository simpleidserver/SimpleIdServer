# Configuration

Different modules within SimpleIdServer utilize a distributed configuration storage to store their settings, including the Authentication and Provisioning modules.

By default, SQL Server is used to store the configuration, but you also have the option to use the No-SQL Key-Value data store [Consul](https://developer.hashicorp.com/consul).