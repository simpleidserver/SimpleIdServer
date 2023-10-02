# Storage

The [Identity Provider](../glossary) can utilize one of the following storage options.

For each type of storage, there is a corresponding NuGet package containing all the migration scripts. These scripts encompass the logic necessary for creating the data structure, such as tables or schemas. Migration scripts will be applied during the startup of the [Identity Provider](../glossary).

| Storage    | Nuget package                                                                                                              |
| ---------  | -------------------------------------------------------------------------------------------------------------------------- |
| SQL Server | [SimpleIdServer.IdServer.SqlServerMigrations](https://www.nuget.org/packages/SimpleIdServer.IdServer.SqlServerMigrations)  |
| PostGreSQL | [SimpleIdServer.IdServer.PostgreMigrations](https://www.nuget.org/packages/SimpleIdServer.IdServer.PostgreMigrations)                                                                              |

If your preferred storage option is not listed, please [contact-us](../contactus). We will make every effort to accommodate your requirements.

Alternatively, if you are comfortable with our solution, you can make modifications in the `Program.cs` file of the [Identity Provider](../glossary) solution.

## SQL Server

To utilize SQL Server, edit the `appsettings.json` file and modify the following values :

| Json Path                               | Value         |
| --------------------------------------- | ------------- |
| $.StorageConfiguration.ConnectionString |               |
| $.StorageConfiguration.Type             | SQLSERVER     |

## POSTGRESQL

To utilize Postgresql, edit the `appsettings.json` file and modify the following values :

| Json Path                               | Value         |
| --------------------------------------- | ------------- |
| $.StorageConfiguration.ConnectionString |               |
| $.StorageConfiguration.Type             | POSTGRE       |