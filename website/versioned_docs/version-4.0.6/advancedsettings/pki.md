# Public Key Infrastructure (PKI)

Here are the key components of SimpleIdServer's PKI. :

1. **Certificate Authority (CA)** : The Certificate Authority is a trusted entity responsible for issuing and managing client certificates.
2. **Client Certificates** : Client certificates are used by OAuth 2.0 clients, for example during the "tls_client_auth" authentication.

In the Administration UI, you can manage the Certificate Authorities (CAs). They can be generated and stored in the database or imported from the Certificate Store.
You can download one of them and install it into the appropriate certificate store.

A Certificate Authority can be used to generate one or more client certificates.