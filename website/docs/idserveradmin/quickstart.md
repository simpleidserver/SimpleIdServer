# Implementing a minimal Administration website

Ecrire un article en anglais en s'insipirant du contenu suivant

Le site d'administration de la solution SimpleIdServer, offre un ensemble complet de fonctionnalités qui permettent de gérer tous les aspects de l'identity management, allant de la création et configurations d'un ou plusieurs clients OAUTH2.0, à l'import d'utilisateurs venant de LDAP vers le serveur d'identité.

Il existe deux façons, pour déployer le site d'administration sur une application ASP.NET CORE.

De façon automatique, en exécutant la ligne de commande suivante :

```
dotnet new idserveradminempty
```

Cette commande créera un project ASP.NET CORE déjà pré-configuré avec une implémentation minimaliste du site d'administration.

Ou de façon manuelle, en créant un projet ASP.NET CORE et en y installant le nuget package `SimpleIdServer.IdServer.Website`.


Pour déployer le site d'administration sur une application ASP.NET CORE, il exiil faut installer le Nuget package 
Le site d'administration de la solution SimpleIdServer, offre un ensemble complet de fonctionnalités 