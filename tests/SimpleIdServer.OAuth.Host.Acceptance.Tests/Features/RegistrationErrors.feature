Feature: RegistrationErrors
	Check errors returned by client registration endpoint
	
Scenario: Error is returned when grant_type is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key			| Value													|
	| grant_types	| [a,b]													|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='grant types a,b are not supported'

Scenario: Error is returned when token_endpoint_auth_method is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key							| Value													|
	| token_endpoint_auth_method	| invalid												|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='unknown authentication method : invalid'

Scenario: Error is returned when response_types is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key				| Value													|
	| response_types	| [a,b]													|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='response types a,b are not supported'

Scenario: Error is returned when response_type is missing
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key				| Value														|
	| response_types	| [token]													|
	| grant_types		| [implicit,authorization_code]								|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='valid response type must be passed for the grant type authorization_code'

Scenario: Error is returned when redirect_uris is missing
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key				| Value														|
	| response_types	| [token]													|
	| grant_types		| [implicit]												|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='missing parameter redirect_uris'

Scenario: Error is returned when redirect_uris is invalid
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key				| Value														|
	| response_types	| [token]													|
	| grant_types		| [implicit]												|
	| redirect_uris		| [invalid]													|

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='redirect uri invalid is not correct'
	
Scenario: Error is returned when scope is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key				| Value														|
	| scope				| a															|
	| redirect_uris		| [http://localhost]										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='scopes a are not supported'

Scenario: Error is returned when software_statement is not a JWS token
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key								| Value														|
	| software_statement				| a															|

	And extract JSON from body

	Then JSON 'error'='invalid_software_statement'
	Then JSON 'error_description'='software statement is not a JWS token'

Scenario: Error is returned when software_statement is a bad JWS token
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key								| Value														|
	| software_statement				| a.b.c														|

	And extract JSON from body

	Then JSON 'error'='invalid_software_statement'
	Then JSON 'error_description'='software statement is not a JWS token'

Scenario: Error is returned when iss is not correct
	When build software statement
	| Key								| Value								|
	| iss								| unknown							|
	
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key								| Value														|
	| software_statement				| $softwareStatement$										|
	
	And extract JSON from body

	Then JSON 'error'='invalid_software_statement'
	Then JSON 'error_description'='software statement issuer is not trusted'