Feature: ClientAuthenticationErrors
	Check errors returned during the client authentication

Scenario: Error is returned when client_assertion_type is not supported
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value              |
	| grant_type            | client_credentials |
	| scope                 | scope              |
	| client_assertion_type | invalid            |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='client assertion type invalid is not supported'

Scenario: Error is returned when client_assertion_type is specified but client_assertion is missing
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='client assertion is missing'

Scenario: Error is returned when client_assertion is invalid
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | invalid                                                |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='client_assertion is not a valid JWT token'

Scenario: Error is returned when client_assertion doesn't contain iss claim
	Given build JWS and sign with a random RS256 algorithm and store into 'clientAssertion'
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
	Given build JWS and sign with a random RS256 algorithm and store into 'clientAssertion'
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
	Given build JWS and sign with a random RS256 algorithm and store into 'clientAssertion'
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