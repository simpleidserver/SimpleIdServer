# Protect server-side application compliant with FAPI using ASP.NET CORE

> [!WARNING]
> Before you start, Make sure you have an [up and running IdentityServer and IdentityServer website](/documentation/gettingstarted/index.html).

In this tutorial, we are going to explain how to create a highly secured ASP.NET CORE application, which respects all the security recommendations from FAPI (https://openid.net/specs/openid-financial-api-part-2-1_0.html#confidential-client)[https://openid.net/specs/openid-financial-api-part-2-1_0.html#confidential-client] :

* The client shall support MTLS as mechanism for sender-constrained access tokens.
* The client shall include `request` or `request_uri` parameter as defined in Section 6 of [OIDC](https://openid.net/specs/openid-connect-core-1_0.html) in the authentication request.
* If the Authorization Request is too large for example a [Rich Authorization Request](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-rar-23), then it is recommended to use [Pushed Authorization Request (PAR)](https://datatracker.ietf.org/doc/html/rfc9126).
* [JWT-Secured OAUTH2.0 authorisation response](https://openid.net/specs/openid-financial-api-jarm.html) (JARM) is used to sign and / or encrypt the authorisation response, it protects against replay, credential leaks and mix-up attacks.
* The PS256 or ES256 algorithms must be used.

The website will have the following configuration :

| Configuration                            | Value           |
| ---------------------------------------- | --------------- |
| Client Authentication Method             | tls_client_auth |
| Authorization Signed Response Algorithm  | ES256           | 
| Identity Token Signed Response Algorithm | ES256           |
| Request Object Signed Response Algorithm | ES256           |
| Pushed Authorization Request             | Yes             |
| Response Mode                            | jwt             |

## Add a client

* Open the IdentityServer website [http://localhost:5002](http://localhost:5002).
* In the Certificate Authorities screen, select one Certificate Authority. Don't forget, the Certificate Authority must be trusted by your machine. You can download it and import into the correct Certificate Store.
* Click on the `Client Certificates` tabulation, and click on the `Add Client Certificate` button.
* Set the Subject Name value to `CN=websiteFAPI` and click on `Add`.

![Add Client Certificate](images/fapi-1.png)

* Click on the `Download` button next to the certificate.
* Navigate to the Clients screen and click on the `Add client` button.
* Select `Web application` and click on Next.

![Select client type](images/fapi-3.png)

* Fill-in the form like this. You can specify any password, it will not be used because we are using `tls_client_auth`. Click on `Save` button.

![Fill-in client form](images/fapi-4.png)

* The generated JSON Web Key will be displayed, copy the value into a text file.

![Generated Website](images/fapi-5.png)

Now your client is ready to be used, you can develop the ASP.NET CORE website.

## Create ASP.NET CORE application

The last step consists to create and configure an ASP.NET CORE project.

* Open a command prompt, run the following commands to create the directory structure for the solution.

```
mkdir HighlySecuredServersideWebsite
cd HighlySecuredServersideWebsite
mkdir src
dotnet new sln -n HighlySecuredServersideWebsite
```


* Create a web project named `Website` and install the `SimpleIdServer.OpenIdConnect` nuget package.

> [!WARNING]
> A full working example will be described in the next SimpleIdServer release. 
> At the moment, we describe how to create a confidential client compliant with the FAPI specification.

In the Administration UI, a Server Side Application compliant with FAPI, for example an ASP.NET CORE website, can easily be created.

* Open the IdentityServer website [http://localhost:5002](http://localhost:5002).
* In the Clients screen, click on `Add client` button.
* Select `Web application` and click on next.

![Choose client](images/fapi-1.png)

* Check the checkbox named `Compliant with FAPI1.0`, enter Subject Name of the Client Certificate, for example : `CN=firstClient`.

![Compliant with FAPI](images/fapi-2.png)

* Click on the `Save` button.

A new client is created, it is configured to use the `tls_client_auth` authentication method.

A client certificate can easily be generated in the `Certificate Authorities` Web Page.