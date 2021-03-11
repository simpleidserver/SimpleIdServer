Feature: Authorization
	Check /authorization endpoint

Scenario: Check s_hash & sub is returned in id_token
	When build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                          | Value                             |
	| token_endpoint_auth_method   | tls_client_auth                   |
	| response_types               | [token,code,id_token]             |
	| grant_types                  | [client_credentials]              |
	| scope                        | accounts                          |
	| redirect_uris                | [https://localhost:8080/callback] |
	| tls_client_auth_san_dns      | firstMtlsClient                   |
	| id_token_signed_response_alg | PS256                             |
	| token_signed_response_alg    | PS256                             |
	| request_object_signing_alg   | RS256                             |
	| jwks                         | $jwks_json$                       |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body
	And add authorized Account Access Consent '$client_id$'

	And use '1' JWK from 'jwks' to build JWS and store into 'request'
	| Key           | Value                                                                               |
	| response_type | code id_token                                                                       |
	| client_id     | $client_id$                                                                         |
	| state         | state                                                                               |
	| response_mode | fragment                                                                            |
	| scope         | accounts                                                                            |
	| redirect_uri  | https://localhost:8080/callback                                                     |
	| nonce         | nonce                                                                               |
	| exp           | $tomorrow$                                                                          |
	| aud           | https://localhost:8080                                                              |
	| claims        | { id_token: { openbanking_intent_id: { value: "$consentId$", essential : true } } } |
		
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value         |
	| response_type | code id_token |
	| client_id     | $client_id$   |
	| request       | $request$     |
	| scope         | accounts      |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'
	
	Then token claim 's_hash'='S6aXNcpTdl7WpwnttWxuog'
	Then token claim 'sub'='$consentId$'