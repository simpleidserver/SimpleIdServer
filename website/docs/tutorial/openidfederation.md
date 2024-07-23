# Openid Federation

The OPENID federation specification has officially been published by the OPENID connect Working group, the 31 may 2024.

The objective of this new specification is to establish a trust relationship between the OPENID Server and the relying parties.
Therefore the manual provisioning of the Relying Parties via a web portal or a dedicated REST.API will not be needed anymore.
There are some advantages to use OPENID federation :

* Less human administration.
* Relying Parties can manage their properties such as the redirect_uri.
* Easily establish a trust relationship between the OPENID server and the relying party.

The OPENID federation is already required by other technologies such as the issuance of [Verifiable Credentials](https://openid.github.io/OpenID4VP/openid-4-verifiable-presentations-wg-draft.html#section-11.2).
In this context, the OPENID federation is used to establish a trust relationship between an electronical wallet / verifier and a credential issuer. 
Suppose both applications adhere to a known trust scheme, for example `BSc chemistry degree`, the electronical wallet will be able to call its federation API to determine if the Credential Issuer is indeed a member of the federation/trust scheme that it says it is.
For more information about the interactions between the electronical wallet and the credential issuer, you can refer to the [official documentation](https://openid.github.io/OpenID4VP/openid-4-verifiable-presentations-wg-draft.html).

This specification takes all its concepts from the Public Key Infrastructure (PKI), but there are some differences between both :
* The Public Key Infrastructure are using certificates, and the Certificate Authority are installed in the Trusted Root Certificate Authorities certificate store. It contains the root certificates of all CAs that Windows trusts.
* The OPENID federation is using Entity Statement. Each entity involved in the trust chain has a REST.API which expose some operations describe in the [specification](https://openid.net/specs/openid-federation-1_0.html).

Before going further, we are going to explain the Public Key Infrastructure.

## Chain of trust in Public Key Infrastructure (PKI)

The purpose of a PKI is to facilitate the secure electronic transfer of information for a range of network activities such as e-commerce, internet banking and confidential email.

PKI uses cryptographic public keys that are connected to a digital certificate, which authenticates the device or user sending the digital communication.
Digital certificates are issued by a trusted source, a certificate authority (CA), and act as a type of digital passport to ensure that the sender is who they say they are.

The client who receive a Digital certificate, for example a browser who visit a secure website, the client validates if the issuer of this certificate exists in its list of trusted root certificates. If there is no match, the client tries to resolve the chain of trust by finding the trusted root certificate authority which has signed the issuing CA certificate.

The chain of trust is an important concept because it proves that the certificate comes from a trusted source. The usage of certificate store is sufficient to resolve a chain of trust.

There are three basic types of entities that comprise a valid chain of trust :

* Root CA certificate : The Root CA certificate is a self-signed X.509 certificate. This certificate acts as a trust anchor, used by all the Relying Parties as the starting point for path validation. The Root CA private key is used to sign the Intermediate CA certificates.

* Intermediate CA certificate : the intermediate CA certificate sits between the Root CA certificate and the end entity certificate. The intermediate CA certificate signs the end entity certificates.

* End-Entity certificate : The end entity certificate is the server certificate that is issued to the website domain.

![Public Key Infrastructure](./images/pki.png)

This chain of trust is also present in the Openid federation specification.

## Chain of trust in OPENID federation

The chain of trust in the Openid federation is made of more than two Entity Statements.

An entity statement is a signed Json Web Token (JWT). The subject of the JWT is the Entity itself. The issuer of the JWT is the party that issued the Entity Statement.
All entities in a federation publish an Entity Statement about themselves called an Entity configuration.

Entities whose statements build a trust chain are categorized as :

* Trust anchor : An Entity that represents a trusted third party.

* Leaf : In an Openid connect identify federation, a Relying Party or a protected resource.

* Intermediate : Neither e leaf entity nor a trust anchor.

![Openid federation trust chain](./images/openidfederation.png)

The resolution of the trust chain is more complex than the one present in the public key infrastructure.

Considering the following entities :
* Relying party : http://localhost:7001

* Trust anchor : http://localhost:7000

The algorithm used to fetch the trust chain is made of the following actions :

1. Retrieve the entity configuration from the endpoint `http://localhost:7001/.well-known/openid-federation`.

2. Store the Json Web Token into the trust chain.

3. Parse the Json Web Token and retrieve the list of `authority_hints` from the payload.

4. For each record in the `authority_hints`, execute the following actions :

   4.1. Retrieve the entity configuration from the `authority_hint` (`<authority_hint>/.well-known/openid-federation`).

   4.2. Parse the Json Web Token and extract the `federation_fetch_endpoint`.

   4.3. Fetch the entity configuration of the relying party `http://localhost:7001` (`<authority_hint>/<federation_fetch_endpoint>?sub=http://localhost:7000`) and store the result into the trust chain.

5. The last entity configuration coming from the `/.well-known/openid-federation` is the trust-anchor and must be stored into the trust chain.

At the end, the trust-chain must contains three records.

## Difference between PKI and OPENID federation

The structure of the trust chain between both technologies is similar and is made of the same components. 
The difference resides in the terminology of the entity used and their nature.
In PKI, an entity is a certificate, however in OPENID federation, an entity is represented as an Entity Statement.

| PKI                         | Openid federation |
| --------------------------- | ----------------- |
| Root CA certificate         | Trust anchor      |
| Intermediate CA certificate | Intermediate      |
| End-entity certificate      | Leaf              |

The trust chain algorithm proposed by OPENID federation is more complex than the one used by PKI.
In OPENID federation, a set of HTTP request are executed to retrieve a list of Entity Statements, but in PKI only the certificate store is used.

Now you understand the differences between PKI and the OPENID federation, we are going to explain how a Relying Party can register itself against an OPENID Identity Server.

## Client registration

AUTOMATIC or EXPLICIT

## Demo

# Resources

https://www.keyfactor.com/blog/certificate-chain-of-trust/