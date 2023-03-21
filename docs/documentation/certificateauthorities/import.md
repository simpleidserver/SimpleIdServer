# Import Certificate Authority

In the Administration UI, you can import Certificate Authorities.
Before being able to do it, you must ensure that the Technical Account used to run the Administration UI, has access to the Certificate Store.

To import a Certificate Authority :

* Open the IdentityServer website [http://localhost:5002](http://localhost:5002).
* In the `Certificate Authorities` screen, click on `Add Certificate Authority`.
* Select `Certificate Store` and click on next.

![Generate](images/import-1.png)

* The Certificate that you want to import, must have an exportable Private Key, it is used to create Client Certificates. Fill-in the form and click on the `Import` button. If the import is successful then the Subject Name and Validity dates are displayed.

![Import](images/import-2.png)