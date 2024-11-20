# Quick Start

import DocsCards from '@site/src/components/global/DocsCards';
import DocsCard from '@site/src/components/global/DocsCard';

The entire SimpleIdServer solution can be easily installed and deployed through various methods: [Copy and Paste](#copy-and-paste), [Docker](#docker) or [Kubernetes](#kubernetes).

If you want to install only certain parts of the solution or customize specific features like the UI, you can use our [DOTNET Template](#dotnet-template) project.


<DocsCards>
    <DocsCard header="One-shot installation" href="#copy-and-paste">
        <p>Install the entire solution with a single command..</p>
    </DocsCard>
    <DocsCard header="Custom installation" href="dotnettemplate">
        <p>Utilize our DOTNET Template to install specific parts of the solution.</p>
    </DocsCard>
</DocsCards>

## Copy and paste

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

### Windows

Procedure :

1. Download the [zip file](https://github.com/simpleidserver/SimpleIdServer/releases/latest/download/SimpleIdServer-Windows-x64.zip).
2. Extract the contents into a directory.
3. In each subfolder, locate the `appsettings.json` file. Open your preferred text editor and update the Connection String.
4. Open three PowerShell prompts and navigate to the subdirectories: `IdServer`, `Scim`, `IdServerWebsite`, `CredentialIssuer` and `CredentialIssuerWebsite`.
5. Execute the command `run.ps1`.

### Linux

Procedure :

1. Download the [zip file](https://github.com/simpleidserver/SimpleIdServer/releases/latest/download/SimpleIdServer-Linux-x64.zip) using the following command:

`wget https://github.com/simpleidserver/SimpleIdServer/releases/latest/download/SimpleIdServer-Linux-x64.zip`

2. Extract the contents into a directory using the following command:

`unzip SimpleIdServer-Linux-x64.zip -d SimpleIdServer-Linux-x64`

3. In the subdirectories, you will find two scripts. Use `run.sh` to launch the service and `install-daemon.sh` to install the server as a daemon service.

## Docker

It is possible to run the SimpleIdServer solution through Docker.

In this setup, the domain `localhost.com` is used to represent the domain on which the solution is hosted. Therefore, the first step is to ensure that the domain `localhost.com` resolves to the Docker host machine.

To achieve this, edit your hosts file and add the following entry:

```
127.0.0.1 localhost.com scim.localhost.com idserver.localhost.com website.localhost.com credentialissuer.localhost.com credentialissuerwebsite.localhost.com
```

The location of the hosts file varies based on the operating system:

| Operating System | Path                                  |
| ---------------- | ------------------------------------- |
| Linux            | \etc\hosts                            |
| Windows          | C:\Windows\system32\drivers\etc\hosts |

Next, download the [Docker archive](https://github.com/simpleidserver/SimpleIdServer/releases/latest/download/Docker.zip),  extract the contents into a directory, and execute the command `docker-compose up`.

Now, SimpleIdServer is ready to be used, and the services can be accessed through the following URLs:

| Service                     | Url                                               |
| --------------------------- | ------------------------------------------------- |
| IdServer                    | https://idserver.localhost.com/master             |
| IdServerWebsite             | https://website.localhost.com/master/clients      |
| Scim                        | https://scim.localhost.com                        |
| CredentialIssuer            | https://credentialissuer.localhost.com            |
| CredentialIssuerWebsite     | https://credentialissuerwebsite.localhost.com     |

## Kubernetes

It is possible to run the SimpleIdServer solution through Kubernetes.

In this setup, the domain `sid.svc.cluster.local` is used to represent the domain on which the solution is hosted. Therefore, the first step is to ensure that the domain `sid.svc.cluster.local` resolves to the Docker host machine.

To achieve this, edit your hosts file and add the following entry:

```
127.0.0.1 sid.svc.cluster.local scim.sid.svc.cluster.local idserver.sid.svc.cluster.local website.sid.svc.cluster.local credentialissuer.sid.svc.cluster.local credentialissuerwebsite.sid.svc.cluster.local
```

Next, ensure that you have `Minikube` installed on your local machine. You can download it from [Minikube](https://minikube.sigs.k8s.io/docs/start/).

Download the [Kubernetes archive file](https://github.com/simpleidserver/SimpleIdServer/releases/latest/download/Kubernetes.zip) and extract its contents into a directory.
 Open a command prompt and navigate to this directory. Execute the following commands to start the solution:

```
minikube start
minikube addons enable ingress
eval $(minikube -p minikube docker-env)
kubectl apply -f sid-kubernetes.yaml
minikube tunnel
```

Now, SimpleIdServer is ready to be used, and the services can be accessed through the following URLs:

| Service                     | Url                                                       |
| --------------------------- | --------------------------------------------------------- |
| IdServer                    | https://idserver.sid.svc.cluster.local/master             |
| IdServerWebsite             | https://website.sid.svc.cluster.local/master/clients      |
| Scim                        | https://scim.sid.svc.cluster.local                        |
| CredentialIssuer            | https://credentialissuer.sid.svc.cluster.local            |
| CredentialIssuerWebsite     | https://credentialissuerwebsite.sid.svc.cluster.local     |