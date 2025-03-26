# Choose entity framework as the persistence layer

Ecrire un article en anglais qui explique comment configurer le serveur d'identité avec entity framework.

Tout d'abord, il est nécessaire d'avoir un serveur d'identité configuré sur une application ASP.NET CORE.

Pour chaque type de base de données, il existe un nuget package avec les fichiers de migration.

Pour supporter sql sevrver, installer le nuget package  `SimpleIdServer.IdServer.SqlServerMigrations`.
Dans le fichier Program.cs, ajouter les lignes suivantes