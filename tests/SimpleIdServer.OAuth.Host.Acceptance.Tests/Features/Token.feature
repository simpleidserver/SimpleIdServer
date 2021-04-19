Feature: Token
	Get access token

Scenario: Use client_credentials grant type to get an access token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value                                |
	| client_id     | f3d35cce-de69-45bf-958c-4a8796f8ed37 |
	| client_secret | BankCvSecret                         |
	| scope         | scope1                               |
	| grant_type    | client_credentials                   |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'

Scenario: Use client_credentials grant type & use tls_client_auth authentication type to get an access token
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |
	| X-Testing-ClientCert | mtlsClient.crt     |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'
	
Scenario: Use client_credentials grant type & use self_signed_tls_client_auth authentication type to get an access token
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
	| token_endpoint_auth_method | self_signed_tls_client_auth                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
	| response_types             | [token]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
	| grant_types                | [client_credentials]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
	| scope                      | scope1                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |
	| redirect_uris              | [http://localhost:8080]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
	| jwks                       | {"keys":[{"kty":"RSA","e":"AQAB","use":"sig","kid":"Bn7g-S0XFmgOXwBtSQZrjoVJthn-w9iPY03DrO18IiA","alg":"PS256","n":"oszxSz7pqibQ4rn5-Izv00ILebp_JIVgYz8bD45-rysG63dNx36P_R-DbdPdQrkza1h4JAJgoe690fh1iKXFbvKEPyTFwqRrUvtqDlgmo-L1889BUJfrjdkavvl71gVCfnGA_Bn8nyMEqgqcsVfX2F1Cl4Y7Adm9S9GwR72ziOOLOni5iAzqYOg5MCcqVukZpaBu07M5HbkLvl-vO_ihYBpwC5wY28l-ROopwKbUDoh7t_zmU0WCNzjOUEptj1gFhHJYWLZi9ZvVC62ZLz3UY7AcPgwd0vyL5SQ3xc7zRZeH1FmOnXy_3zljeEszM6TXwnlNxBgHVRG1Stbv0-_RoQ","x5c":["MIICvzCCAaegAwIBAgIGAXjl7NWqMA0GCSqGSIb3DQEBCwUAMBYxFDASBgNVBAMTC2ZpcnN0Q2xpZW50MB4XDTIxMDQxODE3MDAwNloXDTIyMDQxODE3MDAwNlowFjEUMBIGA1UEAxMLZmlyc3RDbGllbnQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCizPFLPumqJtDiufn4jO/TQgt5un8khWBjPxsPjn6vKwbrd03Hfo/9H4Nt091CuTNrWHgkAmCh7r3R+HWIpcVu8oQ/JMXCpGtS+2oOWCaj4vXzz0FQl+uN2Rq++XvWBUJ+cYD8GfyfIwSqCpyxV9fYXUKXhjsB2b1L0bBHvbOI44s6eLmIDOpg6DkwJypW6RmloG7TszkduQu+X687+KFgGnALnBjbyX5E6inAptQOiHu3/OZTRYI3OM5QSm2PWAWEclhYtmL1m9ULrZkvPdRjsBw+DB3S/IvlJDfFzvNFl4fUWY6dfL/fOWN4SzMzpNfCeU3EGAdVEbVK1u/T79GhAgMBAAGjEzARMA8GA1UdEwEB/wQFMAMBAf8wDQYJKoZIhvcNAQELBQADggEBADrJKAl3ydazH0thYKb6KQ1fSjPuHvp3Ejnc2DWUjjpNydqXsFgkN0UK78v9/r1k7O80aK5HSUkMQvM+qyXIEelub1+KLcjuWYWRN+33eXfjqiFoJpQ9Tcf7THloNTW+er0FxYOYQBt8d0pUph3f/A6WRbzMV9AH2XoKjWsHZN8FKpkNdj2TPxuiB03WPYYJP9BGmei+yj19RN9IGyJfeUQfW3F4l5l4HgMdNlfNAAg0iE5XxPt73u7m8jCqK1Atr1CNYv62XvzPb5DxnyRK4CxEjJDy6gDbKhlfoC7sOw9WWZv6kJxWEsv2RY3ZGwLoCr5Ydb3BBeVkLBnmyDM1Tdk="]}]} |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                     |
	| client_id            | $client_id$               |
	| scope                | scope1                    |
	| grant_type           | client_credentials        |
	| X-Testing-ClientCert | selfSignedCertificate.cer |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'

Scenario: Use password grant type to get an access token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| username		| administrator											|
	| password		| password												|
	| grant_type	| password												|

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'

Scenario: Use authorization_code grant type to get an access token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And add user consent : user='administrator', scope='scope1', clientId='f3d35cce-de69-45bf-958c-4a8796f8ed37'

	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key			| Value													|
	| response_type | code													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| state			| state													|
	| redirect_uri  | http://localhost:8080									|
	| response_mode | query													|
	
	And extract parameter 'code' from redirect url

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| grant_type	| authorization_code									|
	| code			| $code$												|
	| redirect_uri  | http://localhost:8080									|

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'

Scenario: Use refresh_token grant type to get an access token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| scope			| scope1												|
	| grant_type	| client_credentials									|
	
	And extract JSON from body
	And extract parameter 'refresh_token' from JSON body
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| refresh_token	| $refresh_token$										|
	| grant_type	| refresh_token											|

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'

Scenario: Revoke refresh_token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| scope			| scope1												|
	| grant_type	| client_credentials									|
	
	And extract JSON from body
	And extract parameter 'refresh_token' from JSON body	
	
	And execute HTTP POST request 'https://localhost:8080/token/revoke'
	| Key			| Value													|
	| token			| $refresh_token$										|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	
	Then HTTP status code equals to '200'

Scenario: Use authorization_code grant type to get an access token (PKCE)
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
	| Key					| Value											|
	| response_type			| code											|
	| client_id				| $client_id$									|
	| state					| state											|
	| scope					| scope1										|
	| code_challenge		| VpTQii5T_8rgwxA-Wtb2B2q9lg6x-KVldwQLwQKPcCs	|
	| code_challenge_method	| S256											|	
	
	And extract parameter 'code' from redirect url	

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value						|
	| client_id		| $client_id$				|
	| client_secret | BankCvSecret				|
	| grant_type	| authorization_code		|
	| code			| $code$					|
	| code_verifier | code					|
	| redirect_uri  | http://localhost:8080		|
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'