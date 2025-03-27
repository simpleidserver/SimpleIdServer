# Login & password authentication

L'authentification par login et mot de passe est présente dans tous les templates dotnet du serveur d'identité mais pas dans le template `idserverempty`.

l'authentification par login et mot de passe est présente dans le nuget package `SimpleIdServer.IdServer.Pwd`.
ses dépendances sont enregistrée dans le fichier `program.cs`, en appelant la fonction  `AddPwdAuthentication()`.
cette fonction accepte un paramètre, qui permet de choisir ou non l'authentification par login & mot de passe comme celle par défaut, pour s'authentifier sur le serveur d'identité.

// EXPLIQUER COMMENT FONCTIONNE LA CONFIGURATION AUTOMATIQUE !!!
l'authentification par login et mot de passe est 