Feature: ClientPrivateKeyJwtAuthenticationErrors
	Check errors returned during the 'private_key_jwt' authentication

Scenario: Error is returned when client_assertion doesn't contain iss claim
	Given build JWS by signing with a random RS256 algorithm and store the result into 'clientAssertion'
	| Key  | Value |
	| user | user  |

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertion$                                      |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='client_id cannot be extracted from client_assertion'

Scenario: Error is returned when issuer present in the client_assertion is not a valid client
	Given build JWS by signing with a random RS256 algorithm and store the result into 'clientAssertion'
	| Key  | Value |
	| iss  | bad   |
	
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertion$                                      |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='unknown client bad'

Scenario: Error is returned when client_assertion is not signed by a known json web key (JWK)
	Given build JWS by signing with a random RS256 algorithm and store the result into 'clientAssertion'
	| Key  | Value       |
	| iss  | sevenClient |
	
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertion$                                      |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='client assertion is not signed by a known Json Web Key'

Scenario: Error is returned when iss != sub
	Given build JWS by signing with the key 'seventClientKeyId' coming from the client 'sevenClient' and store the result into 'clientAssertion'
	| Key  | Value       |
	| iss  | sevenClient |
	| sub  | sub         |
	
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertion$                                      |	

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client assertion issuer'

Scenario: Error is returned when audience is invalid
	Given build JWS by signing with the key 'seventClientKeyId' coming from the client 'sevenClient' and store the result into 'clientAssertion'
	| Key  | Value       |
	| iss  | sevenClient |
	| sub  | sevenClient |
	| aud  | invalid     |
	
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertion$                                      |	

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client assertion audiences'

Scenario: Error is returned when client_assertion is expired
	Given build expired JWS by signing with the key 'seventClientKeyId' coming from the client 'sevenClient' and store the result into 'clientAssertion'
	| Key  | Value                        |
	| iss  | sevenClient                  |
	| sub  | sevenClient                  |
	| aud  | https://localhost:8080/token |
	
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertion$                                      |	

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='client assertion is expired'