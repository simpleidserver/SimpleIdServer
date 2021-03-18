Feature: BCAuthorizeErrors
	Check errors returned by /mtls/bc-authorize endpoint

Scenario: Error is returned when hint is missing
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

Scenario: Error is returned when scope is missing
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

Scenario: Error is returned when scope is not valid
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

Scenario: Error is returned when client_notification_token is missing
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

Scenario: Error is returned when client_notification_token is < 128 bits
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

Scenario: Error is returned when id_token_hint is invalid
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