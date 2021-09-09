# Introduction

The SimpleIdServer architecture is made of 11 blocks :

* **Provisioning** : Windows service which maintains data consistency between APIS. A common scenario consists to remove user from OPENID and SCIM. 
* **BPMN** : REST.API service which executes BPMN processes in background. The workflows used to perform credential provisioning are modeled with BPMN. Thanks to this standard the list of steps participating in the workflow can easily be defined by a developer. 
* **HumanTask** : Human Task REST.API service based on the WS-HumanTask implementation. This API is used by BPMN in order to have human actors participating in the process for example : Update password. 
* **OPENID** : OPENID identity provider.
* **SAMLIDP** : SAML2.0 identity provider.
* **UMA** : User Management Access (V2.0) REST.API. The User-Managed Access protocol framework defines a mechanism to allow a resource owner to delegate access to a protected resource for a client application used by requesting party (identify by a set of claims), optionally limited by a set of scopes. 
* **SCIM2.0** : SCIM2.0 REST.API. The System for Cross-domain Identity Management (SCIM) specification is designed to make managing user identities in cloud-based applications and services easier. 
* **Gateway** : Single entry point for all the clients. 
* **Website** : Administration portal where users with administrator role can manage clients, users and other assets.
* **Message Broker** : Events like “User is added”, “User is removed” are sent to the message broker. It is mostly used by SimpleIdServer to do provisioning.
* **Database** : SQL Server Database where assets are stored.

![Architecture](images/big-picture-1.png)