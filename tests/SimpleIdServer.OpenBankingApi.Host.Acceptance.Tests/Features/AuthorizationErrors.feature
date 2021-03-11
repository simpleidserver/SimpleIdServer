Feature: AuthorizationErrors
	Check errors returned by authorization

Scenario: Error is returned when Account Access Consent doesn't exist
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                             |
	| token_endpoint_auth_method | tls_client_auth                   |
	| response_types             | [token,code,id_token]             |
	| grant_types                | [client_credentials]              |
	| scope                      | accounts                          |
	| redirect_uris              | [https://localhost:8080/callback] |
	| tls_client_auth_san_dns    | firstMtlsClient                   |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body
		
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                                                                         |
	| response_type | id_token                                                                      |
	| client_id     | $client_id$                                                                   |
	| state         | state                                                                         |
	| response_mode | fragment                                                                      |
	| scope         | accounts                                                                      |
	| redirect_uri  | https://localhost:8080/callback                                               |
	| nonce         | nonce                                                                         |
	| claims        | { id_token: { openbanking_intent_id: { value: "value", essential : true } } } |

	And extract query parameters into JSON

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='account access consent 'value' doesn't exist'

Scenario: Error is returned when Account Access Consent has been rejected
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                             |
	| token_endpoint_auth_method | tls_client_auth                   |
	| response_types             | [token,code,id_token]             |
	| grant_types                | [client_credentials]              |
	| scope                      | accounts                          |
	| redirect_uris              | [https://localhost:8080/callback] |
	| tls_client_auth_san_dns    | firstMtlsClient                   |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And add rejected Account Access Consent

	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                                                                               |
	| response_type | id_token                                                                            |
	| client_id     | $client_id$                                                                         |
	| state         | state                                                                               |
	| response_mode | fragment                                                                            |
	| scope         | accounts                                                                            |
	| redirect_uri  | https://localhost:8080/callback                                                     |
	| nonce         | nonce                                                                               |
	| claims        | { id_token: { openbanking_intent_id: { value: "$consentId$", essential : true } } } |

	And extract query parameters into JSON

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='Account Access Consent has been rejected'

Scenario: Error is returned when 'exp' claim is missing from the request object
	When build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                             |
	| token_endpoint_auth_method | tls_client_auth                   |
	| response_types             | [token,code,id_token]             |
	| grant_types                | [client_credentials]              |
	| scope                      | accounts                          |
	| request_object_signing_alg | RS256                             |
	| jwks                       | $jwks_json$                       |
	| client_id                  | invalid                           |
	| redirect_uris              | [https://localhost:8080/callback] |
	| tls_client_auth_san_dns    | firstMtlsClient                   |
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	
	And use '1' JWK from 'jwks' to build JWS and store into 'request'
	| Key           | Value                                                                               |
	| response_type | id_token                                                                            |
	| client_id     | $client_id$                                                                         |
	| state         | state                                                                               |
	| response_mode | fragment                                                                            |
	| scope         | accounts                                                                            |
	| redirect_uri  | https://localhost:8080/callback                                                     |
	| nonce         | nonce                                                                               |
	
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value       |
	| response_type | id_token    |
	| client_id     | $client_id$ |
	| request       | $request$   |
	| scope         | accounts    |

	And extract query parameters into JSON

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter exp'

Scenario: Error is returned when request object is expired
	When build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                             |
	| token_endpoint_auth_method | tls_client_auth                   |
	| response_types             | [token,code,id_token]             |
	| grant_types                | [client_credentials]              |
	| scope                      | accounts                          |
	| request_object_signing_alg | RS256                             |
	| jwks                       | $jwks_json$                       |
	| client_id                  | invalid                           |
	| redirect_uris              | [https://localhost:8080/callback] |
	| tls_client_auth_san_dns    | firstMtlsClient                   |
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	
	And use '1' JWK from 'jwks' to build JWS and store into 'request'
	| Key           | Value                           |
	| response_type | id_token                        |
	| client_id     | $client_id$                     |
	| state         | state                           |
	| response_mode | fragment                        |
	| scope         | accounts                        |
	| redirect_uri  | https://localhost:8080/callback |
	| nonce         | nonce                           |
	| exp           | 1613046297                      |
	
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value       |
	| response_type | id_token    |
	| client_id     | $client_id$ |
	| request       | $request$   |
	| scope         | accounts    |

	And extract query parameters into JSON

	Then JSON 'error'='invalid_request_object'
	Then JSON 'error_description'='request object is expired'