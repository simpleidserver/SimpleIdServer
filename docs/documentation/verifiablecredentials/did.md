# Decentralized Identifier (DID)

A Decentralized Identifier (DID) is a globally unique identifier that is designed to provide self-sovereign and decentralized digital identity. It is a URI (Uniform Resource Identifier) that serves as a persistent and globally resolvable identifier for an entity or subject.

DIDs are designed to be independent of centralized identity authorities or registries. They enable individuals, organizations, or things to have control over their own identifiers and associated digital identities. DIDs can be created, managed, and resolved using distributed ledger technologies (such as blockchain) or other decentralized systems.

For more information about the Decentralized Identitier please read the documentation https://github.com/decentralized-identity/universal-resolver

SimpleIdServer supports two types of Decentralized Identifiers (DIDs).

| Type | Description                                                                                        | Url                                                                                            |
| ---- | -------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |              
| key  | Creating a did:key value consists of creating a cryptographic key pair and encoding the public key | https://w3c-ccg.github.io/did-method-key/                                                      |
| ethr | utilize a deployed smart contract on the Ethereum blockchain for reading the DID Document          | https://github.com/decentralized-identity/ethr-did-resolver/blob/master/doc/did-method-spec.md |