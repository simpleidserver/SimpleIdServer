# Credential issuer

OpenID for Verifiable Credential Issuance is an extension of the OpenID Connect protocol that enables the issuance and presentation of verifiable credentials within an identity ecosystem. 
Verifiable credentials are digital representations of claims or attributes that are cryptographically signed by a trusted issuer and can be independently verified.

Here are the key components and concepts of OpenID for Verifiable Credential Issuance:

* Credential Issuer : An entity that issues Verifiable Credentials. Also called Issuer. In the context of this specification, the Credential Issuer acts as an OAuth 2.0 Authorization Server. It can be an organization, a government agency, or any trusted party that can issue digitally signed credentials.
* Credential : A set of one or more claims about a subject made by a Credential Issuer. 
* Wallet : n entity used by the Holder to receive, store, present, and manage Verifiable Credentials and key material. 

SimpleIdServer utilizes Decentralized Identifiers (DIDs) to sign credentials and ensure that only trusted wallets are able to obtain the credential.