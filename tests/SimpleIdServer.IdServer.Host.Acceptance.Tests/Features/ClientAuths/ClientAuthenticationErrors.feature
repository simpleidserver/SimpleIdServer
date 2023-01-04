Feature: ClientAuthenticationErrors
	Check common errors returned during the client authentication

Scenario: Error is returned when client_id is missing
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value              |
	| grant_type            | client_credentials |
	| scope                 | scope              |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='missing client_id'

Scenario: Error is returned when client_id is invalid
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value              |
	| grant_type | client_credentials |
	| scope      | scope  	          |
	| client_id  | c                  |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='unknown client c'

Scenario: Error is returned when client_secret is missing
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value              |
	| grant_type | client_credentials |
	| scope      | scope  	          |
	| client_id  | firstClient        |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client credential'

Scenario: Error is returned when client_secret is invalid
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| grant_type    | client_credentials |
	| scope         | scope  	         |
	| client_id     | firstClient        |
	| client_secret | bad                |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client credential'

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