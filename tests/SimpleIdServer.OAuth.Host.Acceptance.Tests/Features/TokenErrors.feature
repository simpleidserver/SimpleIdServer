Feature: TokenErrors
	Check errors returned by token endpoint

Scenario: Error is returned when code_verifier parameter is missing
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key							| Value						|
	| token_endpoint_auth_method	| pkce						|
	| response_types				| [code]					|
	| grant_types					| [authorization_code]		|
	| scope							| scope1					|
	| redirect_uris					| [http://localhost:8080]	|

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value								|
	| client_id		| $client_id$						|
	| grant_type	| authorization_code				|
	| code			| code								|
	| redirect_uri  | http://localhost:8080				|

	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter code_verifier'

Scenario: Error is returned when code_verifier is invalid
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key							| Value						|
	| token_endpoint_auth_method	| pkce						|
	| response_types				| [code]					|
	| grant_types					| [authorization_code]		|
	| scope							| scope1					|
	| redirect_uris					| [http://localhost:8080]	|

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='scope1', clientId='$client_id$'
	
	And execute HTTP GET request 'http://localhost/authorization'
	| Key					| Value				|
	| response_type			| code				|
	| client_id				| $client_id$		|
	| state					| state				|
	| scope					| scope1			|
	| code_challenge		| code_challenge	|
	| code_challenge_method	| S256				|
	
	And extract parameter 'code' from redirect url	

	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value						|
	| client_id		| $client_id$				|
	| client_secret | BankCvSecret				|
	| grant_type	| authorization_code		|
	| code			| $code$					|
	| code_verifier | invalid					|
	| redirect_uri  | http://localhost:8080		|
	
	And extract JSON from body

	Then JSON 'error'='invalid_grant'
	Then JSON 'error_description'='code_verifier is invalid'