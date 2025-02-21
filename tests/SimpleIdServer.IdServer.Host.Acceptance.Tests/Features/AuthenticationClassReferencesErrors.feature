Feature: AuthenticationClassReferencesErrors
	Check errors returned by acrs API
	
Scenario: name parameter is required
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | sixtyTwoClient     |
	| client_secret | password           |
	| scope         | acrs   	         |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'
	
	When execute HTTP POST JSON request 'http://localhost/acrs'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter name'

Scenario: acr must be unique
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | sixtyTwoClient     |
	| client_secret | password           |
	| scope         | acrs   	         |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'
	
	When execute HTTP POST JSON request 'http://localhost/acrs'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |
	| name          | sid-load-01           |
	| amrs          | ["pwd"]               |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='an acr with the same name already exists'

Scenario: cannot remove unknown acr
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | sixtyTwoClient     |
	| client_secret | password           |
	| scope         | acrs   	         |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'
	
	When execute HTTP DELETE request 'http://localhost/acrs/id'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |

	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'error'='unknown_acr'
	Then JSON 'error_description'='the acr id doesn't exist'
