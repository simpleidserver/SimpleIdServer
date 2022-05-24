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
	| scope         | scope1 scope2                        |
	| grant_type    | client_credentials                   |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'
	Then Extract JWS payload from 'access_token' and check claim 'scope' is array

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
	| jwks                       | {"keys":[{"kty":"RSA","e":"AQAB","use":"sig","kid":"Bn7g-S0XFmgOXwBtSQZrjoVJthn-w9iPY03DrO18IiA","alg":"PS256","n":"oszxSz7pqibQ4rn5-Izv00ILebp_JIVgYz8bD45-rysG63dNx36P_R-DbdPdQrkza1h4JAJgoe690fh1iKXFbvKEPyTFwqRrUvtqDlgmo-L1889BUJfrjdkavvl71gVCfnGA_Bn8nyMEqgqcsVfX2F1Cl4Y7Adm9S9GwR72ziOOLOni5iAzqYOg5MCcqVukZpaBu07M5HbkLvl-vO_ihYBpwC5wY28l-ROopwKbUDoh7t_zmU0WCNzjOUEptj1gFhHJYWLZi9ZvVC62ZLz3UY7AcPgwd0vyL5SQ3xc7zRZeH1FmOnXy_3zljeEszM6TXwnlNxBgHVRG1Stbv0-_RoQ","x5c":["MIIDHjCCAgagAwIBAgIQaynQaJJbu65PvzytHISk1zANBgkqhkiG9w0BAQsFADAWMRQwEgYDVQQDDAtmaXJzdENsaWVudDAeFw0yMjA0MjIxMzU3MDVaFw0yMzA0MjIxNDE3MDVaMBYxFDASBgNVBAMMC2ZpcnN0Q2xpZW50MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3Q1612L7cHhwNC9VcyRq+0Vah4auPtOS81U9c46XA30U1J3acaJnkxGEyVU4I75vkgA2pvz8pkJvFu/JjPGq76BcF0gwwi5JiTrxfUmIDrhexRXcoEcr+2T/q4WeuIZl8rkTrGhJX7njxdk9jyfx3LSUikm/VeGdpf516MOiVRiKaFCx6XWNcHRSNNQkkyiWbVEzLzdF+zmS5U7hZRl41hAN4/DI+Sbywweu82eUIuO6Rk5rjJT+gQwhcpguwgNu/TJgt5n1Ugb4195FtcvVEMQ/jCnLMEn/7ha9NIwcH7PQBpJRFrQ+iVU2oaR4Cc5qh967fIaYeiFagp8Q57T5OQIDAQABo2gwZjAOBgNVHQ8BAf8EBAMCBaAwHQYDVR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMBMBYGA1UdEQQPMA2CC2ZpcnN0Q2xpZW50MB0GA1UdDgQWBBTJKpXT9bppJh67W0DrwjJVYhsiPTANBgkqhkiG9w0BAQsFAAOCAQEAyiM7vXjvbjWUPUouZJYAN5pRXUTfQOtrGfN0UkJqUjreVeK7CCaBOafeugtgM4iLkbiykJtYC7pLvd9WTyb+e/hvXOzzdh15+BgSYIHPYUn6G7/OvoIjfvSvbUtKIOSTEcFAG8atYDn7xzHZk7k8uxwc3jdpuoFcwHSD0vdxE6UgpczlR71X62hfe3h0UwzJDLbWHalJrKnTzI3x7FTXhXpDvQGH+nvCM31D8wbXcATjRZPV9lu/t0YjWpg6HXMPLHqKyx8uCfJuZL6CqhFo7YnFH2ktQf5G5bfCjTHX9kkrHh1EslgExF2arcUx0+wjm0B2QUepAgrtOCV/WEINWA=="]}]} |

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
	| scope							| scope1 scope2				|
	| redirect_uris					| [http://localhost:8080]	|	

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='scope1 scope2', clientId='$client_id$'
	
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key					| Value											|
	| response_type			| code											|
	| client_id				| $client_id$									|
	| state					| state											|
	| scope					| scope1 scope2									|
	| code_challenge		| VpTQii5T_8rgwxA-Wtb2B2q9lg6x-KVldwQLwQKPcCs	|
	| code_challenge_method	| S256											|	
	
	And extract parameter 'code' from redirect url	

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key			| Value						|
	| client_id		| $client_id$				|
	| client_secret | BankCvSecret				|
	| grant_type	| authorization_code		|
	| code			| $code$					|
	| code_verifier | code						|
	| redirect_uri  | http://localhost:8080		|
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'
	Then JSON 'scope'='scope1 scope2'