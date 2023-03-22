Feature: PushedAuthorizationRequestErrors
	Check errors returned by the pushed authorization request endpoint

Scenario: Reject request with request_uri parameter
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key           | Value      |
	| request_uri   | request    |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the request cannot contains request_uri'

Scenario: Response Type is required
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key       | Value       |
	| client_id | fortyClient |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter response_type'

Scenario: Response Type must be supported
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key           | Value       |
	| client_id     | fortyClient |
	| response_type | invalid     |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='unsupported_response_type'
	Then JSON 'error_description'='missing response types invalid'

Scenario: Scope or resource parameter are required
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | fortyClient           |
	| state         | state                 |
	| response_mode | query                 |
	| redirect_uri  | http://localhost:8081 |
	| nonce         | nonce                 |
	| display       | popup                 |	

	And extract JSON from body
	
	Then HTTP status code equals to '400'	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameters scope,resource'

Scenario: Scope must be supported
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | fortyClient           |
	| state         | state                 |
	| response_mode | query                 |
	| redirect_uri  | http://localhost:8080 |
	| scope         | scope1                |
	| nonce         | nonce                 |
	| display       | popup                 |
	And extract JSON from body
	
	Then HTTP status code equals to '400'	
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='scopes scope1 are not supported'

Scenario: Redirect Uri must be valid
	When execute HTTP POST request 'https://localhost:8080/par'
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
	Then JSON 'error_description'='redirect_uri http://localhost:8081 is not correct'

Scenario: Response Mode must be valid
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | fortyClient           |
	| state         | state                 |
	| response_mode | invalid               |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |	
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='response mode invalid is not supported'	

Scenario: Response type must be supported by the client
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | fortyClient           |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |	
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='unsupported_response_type'
	Then JSON 'error_description'='response types id_token are not supported by the client'

Scenario: Nonce is required when id_token is present in the response_type
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key           | Value                 |
	| response_type | code id_token         |
	| client_id     | thirtyOneClient       |
	| state         | state                 |
	| response_mode | query                 |
	| redirect_uri  | http://localhost:8080 |
	| scope         | openid email          |
	| display       | popup                 |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter nonce'

Scenario: grant_management_action parameter must be valid
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key                     | Value                                                                                 |
	| response_type           | code token                                                                            |
	| client_id               | fortySixClient                                                                        |
	| state                   | state                                                                                 |
	| response_mode           | query                                                                                 |
	| redirect_uri            | http://localhost:8080                                                                 |
	| nonce                   | nonce                                                                                 |
	| claims                  | { "id_token": { "acr": { "essential" : true, "value": "urn:openbanking:psd2:ca" } } } | 
	| resource                | https://cal.example.com                                                               |
	| grant_management_action | invalid                                                                               |
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the grant_management_action invalid is not valid'


Scenario: grant_id cannot be specified when grant_management_action is equals to create
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key                     | Value                                                                                 |
	| response_type           | code token                                                                            |
	| client_id               | fortySixClient                                                                        |
	| state                   | state                                                                                 |
	| response_mode           | query                                                                                 |
	| redirect_uri            | http://localhost:8080                                                                 |
	| nonce                   | nonce                                                                                 |
	| claims                  | { "id_token": { "acr": { "essential" : true, "value": "urn:openbanking:psd2:ca" } } } | 
	| resource                | https://cal.example.com                                                               |
	| grant_management_action | create                                                                                |
	| grant_id                | id                                                                                    |
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='grant_id cannot be specified because the grant_management_action is equals to create'	

Scenario: grant_management_action must be specified when grant_id is present
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key                     | Value                                                                                 |
	| response_type           | code token                                                                            |
	| client_id               | fortySixClient                                                                        |
	| state                   | state                                                                                 |
	| response_mode           | query                                                                                 |
	| redirect_uri            | http://localhost:8080                                                                 |
	| nonce                   | nonce                                                                                 |
	| claims                  | { "id_token": { "acr": { "essential" : true, "value": "urn:openbanking:psd2:ca" } } } | 
	| resource                | https://cal.example.com                                                               |
	| grant_id                | id                                                                                    |
	And extract JSON from body
	
	Then HTTP status code equals to '400'	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter grant_management_action'