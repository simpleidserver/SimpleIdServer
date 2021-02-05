Feature: TokenErrors
	Check errors returned by token endpoint

Scenario: Authorization code cannot be used two times
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| ENC  | 2   | RSA1_5  |

	When execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA1_5                        |
	| id_token_encrypted_response_enc | A256CBC-HS512                 |
	| jwks                            | $jwks_json$                   |
	| token_endpoint_auth_method      | client_secret_post            |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value               |
	| response_type | id_token token code |
	| client_id     | $client_id$         |
	| state         | state               |
	| response_mode | query               |
	| scope         | openid email role   |
	| redirect_uri  | https://web.com     |
	| ui_locales    | en fr               |

	And extract 'id_token' from callback
	And extract 'code' from callback

	And execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | $client_id$        |
	| client_secret | $client_secret$    |
	| grant_type    | authorization_code |
	| code          | $code$             |
	| redirect_uri  | https://web.com    |

	And execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | $client_id$        |
	| client_secret | $client_secret$    |
	| grant_type    | authorization_code |
	| code          | $code$             |
	| redirect_uri  | https://web.com    |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_grant'
	Then JSON 'error_description'='authorization code has already been used, all tokens previously issued have been revoked'