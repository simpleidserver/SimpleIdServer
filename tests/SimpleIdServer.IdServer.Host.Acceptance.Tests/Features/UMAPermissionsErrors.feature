Feature: UMAPermissionsErrors
	Check errors returned by the /perm	

Scenario: resource_id parameter is required
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter resource_id'