Feature: ClientPkceAuthenticationErrors
	Check errors returned during the 'pkce' authentication

Scenario: Error is returned when code_verifier is missing
	Given authenticate a user
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key                   | Value                 |
	| response_type         | code                  |
	| client_id             | nineClient            |
	| state                 | state                 |
	| redirect_uri          | http://localhost:8080 |
	| response_mode         | query                 |
	| scope                 | secondScope			|
	| code_challenge_method | plain                 |
	| code_challenge        | challenge             |

	And extract parameter 'code' from redirect url

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value              |
	| grant_type            | authorization_code |
	| scope                 | scope              |
	| client_id             | nineClient         |
	| code                  | $code$             |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='missing parameter code_verifier'

Scenario: Error is returned when code is missing
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value              |
	| grant_type            | client_credentials |
	| scope                 | scope              |
	| client_id             | nineClient         |
	| code_verifier         | code               |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client credential'

Scenario: Error is returned when code doesn't exist
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value              |
	| grant_type            | client_credentials |
	| scope                 | scope              |
	| client_id             | nineClient         |
	| code_verifier         | code               |
	| code                  | code               |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client credential'

Scenario: Error is returned when code_verifier is invalid
	Given authenticate a user
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key                   | Value                 |
	| response_type         | code                  |
	| client_id             | nineClient            |
	| state                 | state                 |
	| redirect_uri          | http://localhost:8080 |
	| response_mode         | query                 |
	| scope                 | secondScope			|
	| code_challenge_method | plain                 |
	| code_challenge        | challenge             |

	And extract parameter 'code' from redirect url

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value              |
	| grant_type            | authorization_code |
	| scope                 | scope              |
	| client_id             | nineClient         |
	| code_verifier         | invalid            |
	| code                  | $code$             |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='code_verifier is invalid'