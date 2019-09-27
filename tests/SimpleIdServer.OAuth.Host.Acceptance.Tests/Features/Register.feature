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
	| Key							| Value						|
	| redirect_uris					| [http://localhost]		|
	| response_types				| [token]					|
	| grant_types					| [implicit]				|
	| client_name					| name						|
	| client_name#fr				| nom						|
	| client_name#en				| name						|
	| client_uri					| http://localhost			|
	| client_uri#fr					| http://localhost/fr		|
	| logo_uri						| http://localhost/1.png	|
	| logo_uri#fr					| http://localhost/fr/1.png |
	| software_id					| software					|
	| software_version				| 1.0						|
	| token_endpoint_auth_method	| client_secret_basic		|
	| scope							| scope1					|
	| contacts						| [addr1,addr2]				|
	| tos_uri						| http://localhost/tos 		|
	| policy_uri					| http://localhost/policy	|
	| jwks_uri						| http://localhost/jwks 	|

	And extract JSON from body
	
	Then HTTP status code equals to '201'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'response_types'
	Then JSON exists 'contacts'
	Then JSON 'token_endpoint_auth_method'='client_secret_basic'
	Then JSON 'client_uri'='http://localhost'
	Then JSON 'client_uri#fr'='http://localhost/fr'
	Then JSON 'logo_uri'='http://localhost/1.png'
	Then JSON 'logo_uri#fr'='http://localhost/fr/1.png'
	Then JSON 'scope'='scope1'
	Then JSON 'tos_uri'='http://localhost/tos'
	Then JSON 'policy_uri'='http://localhost/policy'
	Then JSON 'jwks_uri'='http://localhost/jwks'
	Then JSON 'client_name'='name'
	Then JSON 'client_name#fr'='nom'
	Then JSON 'client_name#en'='name'
	Then JSON 'software_id'='software'
	Then JSON 'software_version'='1.0'

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