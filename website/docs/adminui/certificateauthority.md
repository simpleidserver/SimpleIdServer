# Managing Certificate Authorities

From the Administration UI, you have a wide range of actions you can perform to manage Certificate Authorities.

## Generate random Certificate Authority

You can generate a random Certificate Authority and store the result in the Database.

Procedure :

1. Click **Certificate Authorities** in the menu.
2. Click **Add Certificate Authority**.
3. Select **Generate** and click on **Next**.
4. Enter the details for the new Certificate Authority. The Subject Name must start with `CN=`.
5. Click **Save**. After saving the details, the new Certificate Authority will be displayed in the table.

Once the Certificate Authority is generated, you can download it and install it into the appropriate Certificate Store.

## Import Certificate Authority

You can import a Certificate Authority from a Certificate Store.

Procedure  :

1. Click **Certificate Authorities** in the menu.
2. Click **Add Certificate Authority**.
3. Select **Certificate Store** and click on **Next**.
4. Complete the form and click on **Import**. If the certificate is correctly imported, then the Subject Name and Validity Date are displayed..
5. Click **Save**. After saving the details, the new Certificate Authority will be displayed in the table.

## Generate Client Certificate

One or more Client Certificates can be generated using a single Certificate Authority. 
They can be used by OAuth 2.0 clients during the `tls_client_auth` authentication.

Procedure :

1. Click **Certificate Authorities** in the menu.
2. Select the Certificate Authority.
3. Select **Client Certificates**
4. Click on **Add Client Certificate**
5. Complete the form and click on **Save**. The Subject Name must start with `CN=`.

Once the Client Certificate is generated, you can download it and use it during the `tls_client_auth` authentication.