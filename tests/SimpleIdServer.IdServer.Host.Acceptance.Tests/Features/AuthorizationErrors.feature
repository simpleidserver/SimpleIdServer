Feature: AuthorizationErrors
	Check errors returned by the authorization endpoint

Scenario: Check redirect_uri is valid
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | fortyClient           |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8081 |
	| nonce         | nonce                 |
	| display       | popup                 |	
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='redirect uri http://localhost:8081 is not correct'
	Then JSON 'state'='state'

Scenario: Scope is required
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | fortyClient           |
	| state         | state                 |
	| response_mode | query                 |
	| redirect_uri  | http://localhost:8081 |
	| nonce         | nonce                 |
	| display       | popup                 |	
	
	Then redirection url contains the parameter value 'error'='invalid_request'
	Then redirection url contains the parameter value 'error_description'='missing parameter scope'

Scenario: Scope must be supported
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | fortyClient           |
	| state         | state                 |
	| response_mode | query                 |
	| redirect_uri  | http://localhost:8080 |
	| scope         | scope1                |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	Then redirection url contains the parameter value 'error'='invalid_request'
	Then redirection url contains the parameter value 'error_description'='scopes scope1 are not supported'

Scenario: Nonce is required when id_token is present in the response_type
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | code id_token         |
	| client_id     | thirtyOneClient       |
	| state         | state                 |
	| response_mode | query                 |
	| redirect_uri  | http://localhost:8080 |
	| scope         | openid email          |
	| display       | popup                 |
	
	Then redirection url contains the parameter value 'error'='invalid_request'
	Then redirection url contains the parameter value 'error_description'='missing parameter nonce'