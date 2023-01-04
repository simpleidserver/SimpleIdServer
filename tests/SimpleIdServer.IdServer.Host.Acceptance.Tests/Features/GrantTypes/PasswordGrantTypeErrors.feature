Feature: PasswordGrantTypeErrors
	Check errors returned when using 'password' grant-type

Scenario: Send 'grant_type=password' with no username parameter
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value    |
	| grant_type | password	|

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter username'

Scenario: Send 'grant_type=password,username=user' with no password parameter
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value    |
	| grant_type | password	|
	| username   | user  	|

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter password'
	
Scenario: Send 'grant_type=password,username=user,password=pwd,client_id=firstClient,client_secret=password' with unauthorized grant_type
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        |
	| grant_type    | password	   |
	| username      | user  	   |
	| password      | pwd  	       |
	| client_id     | firstClient  |
	| client_secret | password     |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='grant type password is not supported by the client'
		
Scenario: Send 'grant_type=password,username=user,password=pwd,client_id=secondClient,client_secret=password' with duplicate scopes
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        |
	| grant_type    | password	   |
	| username      | user  	   |
	| password      | pwd  	       |
	| client_id     | secondClient |
	| client_secret | password     |
	| scope         | scope scope  |
	
	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_scope'
	And JSON '$.error_description'='duplicate scopes : scope'
		
Scenario: Send 'grant_type=password,username=user,password=pwd,client_id=secondClient,client_secret=password' with invalid scope
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        |
	| grant_type    | password	   |
	| username      | user  	   |
	| password      | pwd  	       |
	| client_id     | secondClient |
	| client_secret | password     |
	| scope         | scope        |
	
	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_scope'
	And JSON '$.error_description'='unauthorized to scopes : scope'
		
Scenario: Send 'grant_type=password,username=user,password=pwd,client_id=secondClient,client_secret=password,scope=firstScope' with bad user login
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        |
	| grant_type    | password	   |
	| username      | badUser  	   |
	| password      | badPwd       |
	| client_id     | secondClient |
	| client_secret | password     |
	| scope         | firstScope   |
	
	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='bad user credential'
		
Scenario: Send 'grant_type=password,username=user,password=pwd,client_id=secondClient,client_secret=password,scope=firstScope' with bad user password
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        |
	| grant_type    | password	   |
	| username      | user  	   |
	| password      | badPwd       |
	| client_id     | secondClient |
	| client_secret | password     |
	| scope         | firstScope   |
	
	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='bad user credential'