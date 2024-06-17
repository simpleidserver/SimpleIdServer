Feature: AuthorizationErrors
	Check errors returned by the authorization endpoint	
	
Scenario: ClientId is required
	When execute HTTP GET request 'http://localhost/authorization'
	| Key        | Value        |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter client_id'
	
Scenario: When the client is not registered then only one parameter (client_metadata or client_metadata_uri) can be used
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                 | Value            |
	| client_id           | client           |
	| client_metadata_uri | http://localhost |
	| client_metadata     | { }              |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the parameters client_metadata and client_metadata_uri cannot be used the same time'

Scenario: When the client is not registered then the parameter client_metadata or client_metadata_uri is required
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                 | Value            |
	| client_id           | client           |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the parameter client_metadata or client_metadata_uri is required'
	
Scenario: The client_metadata_uri must be valid
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                 | Value                     |
	| client_id           | client                    |
	| client_metadata_uri | http://clientmetadata.com |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_client_metadata_uri'
	Then JSON 'error_description'='the client_metadata_uri in the Authorization Request returns an error or contains invalid data'

Scenario: Response type is required
	When execute HTTP GET request 'http://localhost/authorization'
	| Key        | Value          |
	| client_id  | client         |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter response_type'

Scenario: Response type must be supported
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value        |
	| response_type | invalid      |
	| client_id     | client       |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='unsupported_response_type'
	Then JSON 'error_description'='missing response types invalid'

Scenario: Scope is required
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value        |
	| response_type | code         |
	| client_id     | client       |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameters scope,resource,authorization_details'