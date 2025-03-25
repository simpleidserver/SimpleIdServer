# Docker

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