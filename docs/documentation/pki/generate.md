# Generate one Certificate Authority

In the Administration UI, you can generate Certificate Authorities.
Private and Public Keys are extracted in PEM format and stored in the Database.

To generate a Certificate Authority :

* Open the IdentityServer website [http://localhost:5002](http://localhost:5002).
* In the `Certificate Authorities` screen, click on `Add Certificate Authority`.
* Select `Generate` and click on next.

![Generate](images/generate-1.png)

* Enter the Subject Name, Number of days the certificate will be valid, and click on the `Generate` button. The Private and Public Keys will be displayed.

![Generate](images/generate-2.png)

* Confirm the creation by clicking on the `Save` button. The new Certificate will be displayed in the table.

Once the Certificate Authority is generated, you can download it and import it into the CA Certificate Store.

![Download](images/generate-3.png)

This new Certificate is used to generate Client Certificates, those Certificates are used during Multual TLS Client Authentication.