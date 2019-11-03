How to protect REST.API service from undesirable clients ?
==========================================================

**client_credentials** is the simplest grant-type which has been introduced by OAUTH2.0. It is mostly used to protect operations of a REST.API service against undesirable clients for example : website, services (REST.API) and desktop applications.

Imagine a REST.API service with two operations, one is used to add a user and the second returns a given user. Only authorized clients should be able to use both operations. 
In this scenario, the **client_credentials** grant-type is a good choice because only a limited set of clients is allowed to execute the operations.

A sample project exists and can be downloaded `here`_, it shows how to use SimpleIdServer to protect a REST.API service.

The project contains three components :

1. OAUTH2.0 server.

2. Users REST.API service.

3. Console application.

.. image:: images/protect-api-undesirable-clients-1.png
   :align: center

To see the project in action :

1. Download the sample project and open the **samples\\ProtectAPIFromUndesirableClients\\ProtectAPIFromUndesirableClients.sln** solution with Visual Studio.

2. Run all the projects, three console applications should be launched where each represents a different web application.

========================  ===========================
URL			  Website
------------------------  ---------------------------
https://localhost:5002	  Users REST.API service
https://localhost:60001	  OAUT2.0
========================  ===========================

Once everything is running, open the console application and press the **Enter** key.
The console application will get an access token from the OAUTH2.0 server and uses it to add and get the user.

.. image:: images/protect-api-undesirable-clients-2.png
   :align: center

1. The console application is using the **client_credentials** grant-type to get an access token valids on the scope **get_user** and **add_user**.

2. The console application is passing the access token in the Authorization header to get and add users.

.. _here: https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/ProtectAPIFromUndesirableClients