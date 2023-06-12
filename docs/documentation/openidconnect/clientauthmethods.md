# Client authentication methods

According to [RFC 6749](https://datatracker.ietf.org/doc/html/rfc6749#section-2.3), *if the client type is confidential, the client and authorization server establish a client authentication method suitable for the security requirements of the authorization server. The authorization server MAY accept any form of client authentication meeting its security requirements.*

SimpleIdServer supports multiple authentication methods for clients.

| Method                      |
| --------------------------- |
| client_secret_basic         | Clients that have received a client_secret value from the Authorization Server authenticate with the Authorization Server using the HTTP Basic Authentication scheme                                                                                            |
| client_secret_post          | Clients that have received a client_secret value from the Authorization Server authenticate with the Authorization Server by including the Client Credentials in the request body                                                                               |
| private_key_jwt             | Clients that have registered a public key sign a JWT using that key.                                                                                                                                                                                            |
| client_secret_jwt           | Clients that have received a client_secret value from the Authorization Server create a JWT using an HMAC SHA algorithm, such as HMAC SHA-256. The HMAC is calculated using the octets of the UTF-8 representation of the client_secret as the shared key.      |
| none                        | The Client does not authenticate itself at the Token Endpoint, either because it uses only the Implicit Flow or because it is a Public Client with no Client Secret or other authentication mechanism.                                                          |
| self_signed_tls_client_auth | Indicates that client authentication to the authorization server will occur using mutual TLS with the client utilizing a self-signed certificate                                                                                                                |
| tls_client_auth             | Indicates that client authentication to the authorization server will occur with mutual TLS utilizing the PKI method of associating a certificate to a client.                                                                                                  |