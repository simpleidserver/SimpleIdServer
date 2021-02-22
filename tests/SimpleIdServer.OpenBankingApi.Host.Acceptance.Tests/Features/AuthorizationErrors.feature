Feature: AuthorizationErrors
	Check errors returned by authorization

Scenario: Error is returned when consentid doesn't exist
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token,code,id_token]   |
	| grant_types                | [client_credentials]    |
	| scope                      | accounts                |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | mtlsClient              |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body
		
	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                                                                         |
	| response_type | id_token                                                                      |
	| client_id     | $client_id$                                                                   |
	| state         | state                                                                         |
	| response_mode | query                                                                         |
	| scope         | openid accounts                                                               |
	| redirect_uri  | http://localhost:8080                                                         |
	| nonce         | nonce                                                                         |
	| claims        | { id_token: { openbanking_intent_id: { value: "value", essential : true } } } |

	And extract JSON from body

	Then HTTP status code equals to '400'