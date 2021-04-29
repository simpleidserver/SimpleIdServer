Feature: TokenErrors
	Check errors returned by token endpoint

Scenario: Error is returned when code_verifier parameter is missing
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key							| Value						|
	| token_endpoint_auth_method	| pkce						|
	| response_types				| [code]					|
	| grant_types					| [authorization_code]		|
	| scope							| scope1					|
	| redirect_uris					| [http://localhost:8080]	|

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value								|
	| client_id		| $client_id$						|
	| grant_type	| authorization_code				|
	| code			| code								|
	| redirect_uri  | http://localhost:8080				|

	And extract JSON from body
	
	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='missing parameter code_verifier'

Scenario: Error is returned when code_verifier is invalid
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key							| Value						|
	| token_endpoint_auth_method	| pkce						|
	| response_types				| [code]					|
	| grant_types					| [authorization_code]		|
	| scope							| scope1					|
	| redirect_uris					| [http://localhost:8080]	|

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='scope1', clientId='$client_id$'
	
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key					| Value				|
	| response_type			| code				|
	| client_id				| $client_id$		|
	| state					| state				|
	| scope					| scope1			|
	| code_challenge		| code_challenge	|
	| code_challenge_method	| S256				|
	
	And extract parameter 'code' from redirect url	

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value						|
	| client_id		| $client_id$				|
	| client_secret | BankCvSecret				|
	| grant_type	| authorization_code		|
	| code			| $code$					|
	| code_verifier | invalid					|
	| redirect_uri  | http://localhost:8080		|
	
	And extract JSON from body

	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='code_verifier is invalid'

Scenario: Error is returned when certificate is missing
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [code]                  |
	| grant_types                | [authorization_code]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value								|
	| client_id		| $client_id$						|
	| grant_type	| authorization_code				|
	| code			| code								|
	| redirect_uri  | http://localhost:8080				|

	And extract JSON from body
	
	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='no client certificate'

Scenario: Error is returned when certificate in incorrect
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [code]                  |
	| grant_types                | [authorization_code]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key          | Value                 |
	| X-SSL-CERT   | cert                  |
	| client_id    | $client_id$           |
	| grant_type   | authorization_code    |
	| code         | code                  |
	| redirect_uri | http://localhost:8080 |

	And extract JSON from body
	
	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='no client certificate'

Scenario: Error is returned when certificate doesn't contain the correct SAN DNS
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [code]                  |
	| grant_types                | [authorization_code]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | dns                     |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                 |
	| X-Testing-ClientCert | mtlsClient.crt        |
	| client_id            | $client_id$           |
	| grant_type           | authorization_code    |
	| code                 | code                  |
	| redirect_uri         | http://localhost:8080 |

	And extract JSON from body
	
	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='certificate san DNS is invalid'

Scenario: Error is returned when certificate doesn't contain the correct SAN EMAIL
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [code]                  |
	| grant_types                | [authorization_code]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_email  | invalidemail@com        |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                 |
	| X-Testing-ClientCert | mtlsClient.crt        |
	| client_id            | $client_id$           |
	| grant_type           | authorization_code    |
	| code                 | code                  |
	| redirect_uri         | http://localhost:8080 |

	And extract JSON from body
	
	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='certificate san EMAIL is invalid'

Scenario: Error is returned when certificate doesn't contain the correct SAN IP
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [code]                  |
	| grant_types                | [authorization_code]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_ip     | 127.0.0.2               |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                 |
	| X-Testing-ClientCert | mtlsClient.crt        |
	| client_id            | $client_id$           |
	| grant_type           | authorization_code    |
	| code                 | code                  |
	| redirect_uri         | http://localhost:8080 |

	And extract JSON from body
	
	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='certificate san IP is invalid'