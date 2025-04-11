# Copy and paste

Installing SimpleIdServer is as simple as downloading it, unzipping it, and updating the connection string. 
By default, the project is configured to use the SQLServer database. Other databases are supported. For more information, refer to the [configuration](configuration) section.

The archive folder contains three projects: 

| Name                    | Port            | Description               |
| ----------------------- | --------------- | ------------------------- | 
| IdServer                | https://*:5001  | Identity server           |
| IdServerWebsite         | https://*:5002  | Administration website    |
| Scim                    | https://*:5003  | SCIM server               |
| CredentialIssuer        | https://*:5005  | Credential issuer         |
| CredentialIssuerWebsite | https://*:5006  | Credential issuer website |

The technical account used to run the `IdServer` and `Scim` servers must have the privilege to create tables and databases. Otherwise, the application cannot deploy the database.
By default, the development certificate is utilized to host the applications under HTTPS. To install it on your local machine, execute the command line `dotnet dev-certs https`.
For more information, refer to the [documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs).

## Windows

Procedure :

1. Download the [zip file](https://github.com/simpleidserver/SimpleIdServer/releases/latest/download/SimpleIdServer-Windows-x64.zip).
2. Extract the contents into a directory.
3. In each subfolder, locate the `appsettings.json` file. Open your preferred text editor and update the Connection String.
4. Open three PowerShell prompts and navigate to the subdirectories: `IdServer`, `Scim`, `IdServerWebsite`, `CredentialIssuer` and `CredentialIssuerWebsite`.
5. Execute the command `run.ps1`.

## Linux

Procedure :

1. Download the [zip file](https://github.com/simpleidserver/SimpleIdServer/releases/latest/download/SimpleIdServer-Linux-x64.zip) using the following command:

```batch title="cmd.exe"
wget https://github.com/simpleidserver/SimpleIdServer/releases/latest/download/SimpleIdServer-Linux-x64.zip
```

2. Extract the contents into a directory using the following command:

```batch title="cmd.exe"
unzip SimpleIdServer-Linux-x64.zip -d SimpleIdServer-Linux-x64
```

3. In the subdirectories, you will find two scripts. Use `run.sh` to launch the service and `install-daemon.sh` to install the server as a daemon service.