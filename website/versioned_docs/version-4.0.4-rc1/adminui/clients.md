# Managing Clients

In the Administration UI, you can easily create, update, or add clients.

A wizard is displayed to facilitate the creation of the clients; they are grouped by template.

There are three templates :

1. `Standard`: This template assists in creating an application that adheres to the standard security protocols, such as `WS-Federation` or `OPENID`.
2. `FAPI2.0`  This template assists in creating an application used in the Financial Domain. This application must be highly secure and must adhere to a set of security practices.
3. `Credential Issuer` : This template assists in creating an application capable of issuing credentials in a trusted and secure manner.

Each template contains a set of clients.

## Standard

* `User Agent Based Application` : A client-side application running in a browser (Angular, EmberJS, VueJS etc...). Client secret and/or refresh tokens cannot be stored by these applications because there is a security risk.
* `Machine` : Machine-to-machine (M2M) applications, such as CLIs, daemons, or services running on your back-end, the system authenticates and authorizes the app rather than a user.
* `Web Application` : Web application executed on server (ASP.NET CORE, SPRING etc...).
* `Mobile`: A desktop or mobile application running on a user's device.
* `WS-Fed Relying Party` : A WS-Federation relying party commonly used by older Microsoft applications.
* `Device` : An IoT application or otherwise browserless or input constrained device.

## FAPI2.0

* `Highly secure Web Application`: The web application is executed on a server (ASP.NET CORE, SPRING, etc.) and implements all the security requirements proposed by FAPI 2.0.
* `Grant Management`: The web application is executed on a server (ASP.NET CORE, SPRING, etc.) and implements all the security requirements proposed by FAPI 2.0. Additionally, it supports grant management.
* `External Device Authentication`: Authentication is performed via an Authentication Device by the user who also consents (if required) to the request.