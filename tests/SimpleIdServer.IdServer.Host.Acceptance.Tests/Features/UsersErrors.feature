Feature: UsersErrors
	Check errors returned by the /users endpoint

Scenario: access token must be passed (HTTP POST)
	When execute HTTP POST JSON request 'https://localhost:8080/users'
	| Key | Value |

	And extract JSON from body

	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: access token must be passed (HTTP GET)
	When execute HTTP GET request 'https://localhost:8080/users/id'
	| Key | Value |

	And extract JSON from body

	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: access token must be passed (HTTP DELETE)
	When execute HTTP DELETE request 'https://localhost:8080/users/id'
	| Key | Value |

	And extract JSON from body

	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: access token must be passed (HTTP PUT Credentials)
	When execute HTTP PUT JSON request 'https://localhost:8080/users/id/credentials'
	| Key | Value |

	And extract JSON from body

	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: name is required
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP POST JSON request 'https://localhost:8080/users'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |	

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='missing parameter name'

Scenario: name must be unique
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP POST JSON request 'https://localhost:8080/users'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |	
	| name            | user                  |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the user user already exists'

Scenario: get an unknown user
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP GET request 'https://localhost:8080/users/id'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		
	
	Then HTTP status code equals to '404'

Scenario: remove an unknown user
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP DELETE request 'https://localhost:8080/users/id'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		
	
	Then HTTP status code equals to '404'

Scenario: credential type is required
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP PUT JSON request 'https://localhost:8080/users/id/credentials'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='missing parameter type'

Scenario: credential value is required
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP PUT JSON request 'https://localhost:8080/users/id/credentials'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		
	| type            | type                  |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='missing parameter value'

Scenario: credential type must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP PUT JSON request 'https://localhost:8080/users/id/credentials'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		
	| type            | type                  |
	| value           | value                 |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the credential type type is not supported'

Scenario: cannot update credential when user doesn't exist
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP PUT JSON request 'https://localhost:8080/users/id/credentials'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		
	| type            | pwd                   |
	| value           | value                 |
	
	Then HTTP status code equals to '404'