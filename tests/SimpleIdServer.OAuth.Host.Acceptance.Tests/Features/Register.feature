Feature: Register
	Check registration endpoint
	
Scenario: Create minimalist client
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key				| Value													|
	| redirect_uris		| [http://localhost]									|

	And extract JSON from body
	
	Then HTTP status code equals to '201'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'token_endpoint_auth_method'
	Then JSON exists 'response_types'
	Then JSON exists 'client_name'

Scenario: Create client
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key				| Value													|
	| redirect_uris		| [http://localhost]									|
	| response_types	| [token]												|
	| grant_types		| [implicit]											|
	| client_name		| name													|
	| client_name#fr	| nom													|
	| client_name#en	| name													|

	And extract JSON from body
	
	Then HTTP status code equals to '201'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'token_endpoint_auth_method'
	Then JSON exists 'response_types'
	Then JSON exists 'client_name'
	Then JSON exists 'client_name#fr'
	Then JSON exists 'client_name#en'

Scenario: Use software_statement parameter to create a client
	When build software statement
	| Key								| Value								|
	| iss								| iss								|
	| redirect_uris						| [http://localhost]				|
	
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key								| Value														|
	| software_statement				| $softwareStatement$										|
	
	And extract JSON from body

	Then HTTP status code equals to '201'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'token_endpoint_auth_method'
	Then JSON exists 'response_types'
	Then JSON exists 'client_name'
	Then JSON exists 'software_statement'