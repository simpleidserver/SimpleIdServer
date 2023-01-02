Feature: RegisterErrors
	Check errors returned during client registration

Scenario: Error is returned when client_id is missing
	When execute HTTP POST request 'https://localhost:8080/register'
	| Key | Value |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter client_id'

Scenario: Error is returned when client_id already exists
	When execute HTTP POST request 'https://localhost:8080/register'
	| Key       | Value       |
	| client_id | firstClient |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='client identifier firstClient already exists'

Scenario: Error is returned when trying to get a client and authorization header is missing
	When execute HTTP GET request 'https://localhost:8080/register/clientid'
	| Key | Value |
	
	Then HTTP status code equals to '401'

Scenario: Error is returned when authorization header is missing
	When execute HTTP GET request 'https://localhost:8080/register/clientid'
	| Key | Value |
	
	Then HTTP status code equals to '401'

Scenario: Error is returned when access token is invalid
	When execute HTTP GET request 'https://localhost:8080/register/clientid'
	| Key           | Value       |
	| Authorization | accesstoken |
	
	Then HTTP status code equals to '401'

Scenario: Error is returned when clientId doesn't exist
	When execute HTTP GET request 'https://localhost:8080/register/clientid'
	| Key           | Value              |
	| Authorization | Bearer accesstoken |
	
	Then HTTP status code equals to '404'