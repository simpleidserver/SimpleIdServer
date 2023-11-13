Feature: Users
	Check result returned by the /users endpoint

Scenario: create a user
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	And execute HTTP POST JSON request 'https://localhost:8080/users'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |	
	| name            | newUser               |

	And extract JSON from body
	
	Then HTTP status code equals to '201'
	And JSON '$.name'='newUser'

Scenario: get a user
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
	| name            | newuser2              |
	
	And extract JSON from body
	And extract parameter 'id' from JSON body	
	
	When execute HTTP GET request 'https://localhost:8080/users/$id$'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |	

	Then HTTP status code equals to '200'
	And JSON '$.name'='newuser2'

Scenario: remove a user
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
	| name            | newuser3              |
	
	And extract JSON from body
	And extract parameter 'id' from JSON body	
	
	When execute HTTP DELETE request 'https://localhost:8080/users/$id$'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |	

	Then HTTP status code equals to '204'