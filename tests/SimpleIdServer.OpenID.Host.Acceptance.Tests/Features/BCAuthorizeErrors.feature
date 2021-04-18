Feature: BCAuthorizeErrors
	Check errors returned by /mtls/bc-authorize endpoint

Scenario: Error is returned when hint is missing (get auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | mtlsClient.crt |
	| client_id            | $client_id$    |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='only one hint can be passed in the request'

Scenario: Error is returned when scope is missing (get auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | mtlsClient.crt |
	| client_id            | $client_id$    |
	| id_token_hint        | idtokenhint    |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter scope'

Scenario: Error is returned when scope is not valid (get auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | mtlsClient.crt |
	| client_id            | $client_id$    |
	| id_token_hint        | idtokenhint    |
	| scope                | scope2         |

	And extract JSON from body

	Then JSON 'error'='invalid_scope'
	Then JSON 'error_description'='invalid scopes : scope2'

Scenario: Error is returned when client_notification_token is missing (get auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | mtlsClient.crt |
	| client_id            | $client_id$    |
	| id_token_hint        | idtokenhint    |
	| scope                | scope1         |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter client_notification_token'

Scenario: Error is returned when client_notification_token is < 128 bits (get auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value          |
	| X-Testing-ClientCert      | mtlsClient.crt |
	| client_id                 | $client_id$    |
	| id_token_hint             | idtokenhint    |
	| scope                     | scope1         |
	| client_notification_token | 1              |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='client_notification_token must contains at least 128 bytes'

Scenario: Error is returned when id_token_hint is invalid (get auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value                                |
	| X-Testing-ClientCert      | mtlsClient.crt                       |
	| client_id                 | $client_id$                          |
	| id_token_hint             | idtokenhint                          |
	| scope                     | scope1                               |
	| client_notification_token | 04bcf708-dfba-4719-a3d3-b213322e2c38 |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='id_token_hint is invalid'

Scenario: Error is returned when hint is missing (confirm auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key                  | Value          |
	| X-Testing-ClientCert | mtlsClient.crt |
	| client_id            | $client_id$    |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='only one hint can be passed in the request'

Scenario: Error is returned when id_token_hint is invalid (confirm auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key                       | Value                                |
	| client_id                 | $client_id$                          |
	| id_token_hint             | idtokenhint                          |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='id_token_hint is invalid'	

Scenario: Error is returned when auth_req_id is missing (confirm auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key                  | Value          |
	| client_id            | $client_id$    |
	| login_hint           | administrator  |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter auth_req_id'

Scenario: Error is returned when authtorization request doesn't exist (confirm auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key                  | Value          |
	| client_id            | $client_id$    |
	| login_hint           | administrator  |
	| auth_req_id          | authreqid      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='auth_req_id doesn't exist'