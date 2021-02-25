Feature: Authorization
	Check /authorization endpoint

Scenario: Check s_hash & sub is returned in id_token
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                          | Value                             |
	| token_endpoint_auth_method   | tls_client_auth                   |
	| response_types               | [token,code,id_token]             |
	| grant_types                  | [client_credentials]              |
	| scope                        | accounts                          |
	| redirect_uris                | [https://localhost:8080/callback] |
	| tls_client_auth_san_dns      | mtlsClient                        |
	| id_token_signed_response_alg | PS256                             |
	| token_signed_response_alg    | PS256                             |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body
	And add authorized Account Access Consent
		
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                                                                               |
	| response_type | id_token                                                                            |
	| client_id     | $client_id$                                                                         |
	| state         | state                                                                               |
	| response_mode | query                                                                               |
	| scope         | accounts                                                                            |
	| redirect_uri  | https://localhost:8080/callback                                                     |
	| nonce         | nonce                                                                               |
	| state         | MTkCNSYlem                                                                          |
	| claims        | { id_token: { openbanking_intent_id : { value: "$consentId$", essential: true } } } |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'
	
	Then token claim 's_hash'='SE4Dquo5iR6tdijU71HDQg'
	Then token claim 'sub'='$consentId$'