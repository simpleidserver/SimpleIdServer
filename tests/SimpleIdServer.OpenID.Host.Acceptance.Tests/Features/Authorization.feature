Feature: Authorization
	Check the authorization endpoint

Scenario: When no access token is issued the resulting claims are returned in the ID token
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | none              |
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract 'display' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'none'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Check display is passed into the callback url
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | ES256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email             |
	| id_token_signed_response_alg | ES256             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value           |
	| response_type | id_token        |
	| client_id     | $client_id$     |
	| state         | state           |
	| response_mode | query           |
	| scope         | openid email    |
	| redirect_uri  | https://web.com |
	| ui_locales    | en fr           |
	| display		| popup			  |
	
	And extract 'display' from callback

	Then '$display$'='popup'

Scenario: Identity token is returned in JWS format (none)
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | none              |
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract 'display' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'none'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWS format (ES256)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | ES256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | ES256             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract 'display' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'ES256'
	Then JWS Kid equals to '1'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWS format (ES384)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | ES384   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | ES384             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'ES384'
	Then JWS Kid equals to '1'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWS format (ES512)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | ES512   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | ES512             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'ES512'
	Then JWS Kid equals to '1'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWS format (HS256)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | HS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | HS256             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'HS256'
	Then JWS Kid equals to '1'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWS format (HS384)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | HS384   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | HS384             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'HS384'
	Then JWS Kid equals to '1'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWS format (HS512)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | HS512   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | HS512             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'HS512'
	Then JWS Kid equals to '1'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWS format (RS256)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | RS256             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'RS256'
	Then JWS Kid equals to '1'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWS format (RS384)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS384   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | RS384             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'RS384'
	Then JWS Kid equals to '1'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWS format (RS512)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | ES256   |

	And add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS512   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | email role        |
	| id_token_signed_response_alg | RS512             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then JWS Alg equals to 'RS512'
	Then JWS Kid equals to '1'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'role' contains 'role1'
	Then token claim 'role' contains 'role2'

Scenario: Identity token is returned in JWE format (RSA1_5 & A128CBC-HS256)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| ENC  | 2   | RSA1_5  |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA1_5                        |
	| id_token_encrypted_response_enc | A128CBC-HS256                 |
	| jwks                            | $jwks_json$                   |

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
	And use 'jwks' JWKS to decrypt '$id_token$' JWE into 'jws'
	And extract payload from JWS '$jws$'
	
	Then JWE Alg equals to 'RSA1_5'
	Then JWE Enc equals to 'A128CBC-HS256'
	Then JWS Alg equals to 'RS256'
	Then token contains 'at_hash'
	Then token contains 'c_hash'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim doesn't contain 'email'
	Then token claim doesn't contain 'role'

Scenario: Identity token is returned in JWE format (RSA1_5 & A192CBC-HS384)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| ENC  | 2   | RSA1_5  |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA1_5                        |
	| id_token_encrypted_response_enc | A192CBC-HS384                 |
	| jwks                            | $jwks_json$                   |

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
	And use 'jwks' JWKS to decrypt '$id_token$' JWE into 'jws'
	And extract payload from JWS '$jws$'
	
	Then JWE Alg equals to 'RSA1_5'
	Then JWE Enc equals to 'A192CBC-HS384'
	Then JWS Alg equals to 'RS256'
	Then token contains 'at_hash'
	Then token contains 'c_hash'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim doesn't contain 'email'
	Then token claim doesn't contain 'role'

Scenario: Identity token is returned in JWE format (RSA1_5 & A256CBC-HS512)
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
	And use 'jwks' JWKS to decrypt '$id_token$' JWE into 'jws'
	And extract payload from JWS '$jws$'
	
	Then JWE Alg equals to 'RSA1_5'
	Then JWE Enc equals to 'A256CBC-HS512'
	Then JWS Alg equals to 'RS256'
	Then token contains 'at_hash'
	Then token contains 'c_hash'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim doesn't contain 'email'
	Then token claim doesn't contain 'role'

Scenario: Identity token is returned in JWE format (RSA-OAEP-256 & A128CBC-HS256)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName      |
	| ENC  | 2   | RSA-OAEP-256 |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA-OAEP-256                  |
	| id_token_encrypted_response_enc | A128CBC-HS256                 |
	| jwks                            | $jwks_json$                   |

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
	And use 'jwks' JWKS to decrypt '$id_token$' JWE into 'jws'
	And extract payload from JWS '$jws$'
	
	Then JWE Alg equals to 'RSA-OAEP-256'
	Then JWE Enc equals to 'A128CBC-HS256'
	Then JWS Alg equals to 'RS256'
	Then token contains 'at_hash'
	Then token contains 'c_hash'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim doesn't contain 'email'
	Then token claim doesn't contain 'role'

Scenario: Identity token is returned in JWE format (RSA-OAEP-256 & A192CBC-HS384)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName      |
	| ENC  | 2   | RSA-OAEP-256 |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA-OAEP-256                  |
	| id_token_encrypted_response_enc | A192CBC-HS384                 |
	| jwks                            | $jwks_json$                   |

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
	And use 'jwks' JWKS to decrypt '$id_token$' JWE into 'jws'
	And extract payload from JWS '$jws$'
	
	Then JWE Alg equals to 'RSA-OAEP-256'
	Then JWE Enc equals to 'A192CBC-HS384'
	Then JWS Alg equals to 'RS256'
	Then token contains 'at_hash'
	Then token contains 'c_hash'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim doesn't contain 'email'
	Then token claim doesn't contain 'role'

Scenario: Identity token is returned in JWE format (RSA-OAEP-256 & A256CBC-HS512)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName      |
	| ENC  | 2   | RSA-OAEP-256 |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA-OAEP-256                  |
	| id_token_encrypted_response_enc | A256CBC-HS512                 |
	| jwks                            | $jwks_json$                   |

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
	And use 'jwks' JWKS to decrypt '$id_token$' JWE into 'jws'
	And extract payload from JWS '$jws$'
	
	Then JWE Alg equals to 'RSA-OAEP-256'
	Then JWE Enc equals to 'A256CBC-HS512'
	Then JWS Alg equals to 'RS256'
	Then token contains 'at_hash'
	Then token contains 'c_hash'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim doesn't contain 'email'
	Then token claim doesn't contain 'role'

Scenario: Identity token is returned in JWE format (RSA-OAEP & A128CBC-HS256)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName  |
	| ENC  | 2   | RSA-OAEP |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA-OAEP                      |
	| id_token_encrypted_response_enc | A128CBC-HS256                 |
	| jwks                            | $jwks_json$                   |

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
	And use 'jwks' JWKS to decrypt '$id_token$' JWE into 'jws'
	And extract payload from JWS '$jws$'
	
	Then JWE Alg equals to 'RSA-OAEP'
	Then JWE Enc equals to 'A128CBC-HS256'
	Then JWS Alg equals to 'RS256'
	Then token contains 'at_hash'
	Then token contains 'c_hash'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim doesn't contain 'email'
	Then token claim doesn't contain 'role'

Scenario: Identity token is returned in JWE format (RSA-OAEP & A192CBC-HS384)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName  |
	| ENC  | 2   | RSA-OAEP |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA-OAEP                      |
	| id_token_encrypted_response_enc | A192CBC-HS384                 |
	| jwks                            | $jwks_json$                   |

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
	And use 'jwks' JWKS to decrypt '$id_token$' JWE into 'jws'
	And extract payload from JWS '$jws$'
	
	Then JWE Alg equals to 'RSA-OAEP'
	Then JWE Enc equals to 'A192CBC-HS384'
	Then JWS Alg equals to 'RS256'
	Then token contains 'at_hash'
	Then token contains 'c_hash'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim doesn't contain 'email'
	Then token claim doesn't contain 'role'

Scenario: Identity token is returned in JWE format (RSA-OAEP & A256CBC-HS512)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName  |
	| ENC  | 2   | RSA-OAEP |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA-OAEP                      |
	| id_token_encrypted_response_enc | A256CBC-HS512                 |
	| jwks                            | $jwks_json$                   |

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
	And use 'jwks' JWKS to decrypt '$id_token$' JWE into 'jws'
	And extract payload from JWS '$jws$'
	
	Then JWE Alg equals to 'RSA-OAEP'
	Then JWE Enc equals to 'A256CBC-HS512'
	Then JWS Alg equals to 'RS256'
	Then token contains 'at_hash'
	Then token contains 'c_hash'
	Then token contains 'iss'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'azp'
	Then token contains 'aud'
	Then token claim 'sub'='administrator'
	Then token claim doesn't contain 'email'
	Then token claim doesn't contain 'role'
	   	
Scenario: Use request object (JWS) parameter to get an access token and authorization code
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                        | Value                         |
	| redirect_uris              | [https://web.com]             |
	| grant_types                | [implicit,authorization_code] |
	| response_types             | [code,id_token]               |
	| scope                      | email                         |
	| request_object_signing_alg | RS256                         |
	| jwks                       | $jwks_json$                   |
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And use '1' JWK from 'jwks' to build JWS and store into 'request'
	| Key           | Value           |
	| iss           | $client_id$     |
	| aud           | aud             |
	| response_type | code id_token   |
	| client_id     | $client_id$     |
	| response_mode | query           |
	| scope         | openid email    |
	| redirect_uri  | https://web.com |

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value         |
	| request       | $request$     |
	| response_type | code id_token |
	| client_id     | $client_id$   |
	| state         | state         |
	| scope         | openid email  |

	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'
	
	Then redirect url contains 'id_token'
	Then redirect url contains 'code'
	Then token contains 'iss'
	Then token contains 'aud'
	Then token contains 'exp'
	Then token contains 'iat'
	Then token contains 'azp'
	Then token contains 'c_hash'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'

Scenario: Use request object (JWE) parameter to get an access token and authorization code
	When add JSON web key to Authorization Server and store into 'jwks_enc'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |
	| ENC  | 2   | RSA1_5  |

	And build JSON Web Keys, store JWKS into 'jwks_sig' and store the public keys into 'jwks_sig_json'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                           | Value                         |
	| redirect_uris                 | [https://web.com]             |
	| grant_types                   | [implicit,authorization_code] |
	| response_types                | [code,id_token]               |
	| scope                         | email                         |
	| request_object_signing_alg    | RS256                         |
	| request_object_encryption_alg | RSA1_5                        |
	| request_object_encryption_enc | A128CBC-HS256                 |
	| jwks                          | $jwks_sig_json$               |
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And extract parameter 'client_secret' from JSON body	
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And use '1' JWK from 'jwks_sig' to build JWS and store into 'jws_request'
	| Key           | Value           |
	| iss           | $client_id$     |
	| aud           | aud             |
	| response_type | code id_token   |
	| client_id     | $client_id$     |
	| response_mode | query           |
	| scope         | openid email    |
	| redirect_uri  | https://web.com |

	And use '2' JWKS from 'jwks_enc' to encrypt '$jws_request$' and enc 'A128CBC-HS256' and store the result into 'request'
		
	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value         |
	| request       | $request$     |
	| response_type | code id_token |
	| client_id     | $client_id$   |
	| state         | state         |
	| scope         | openid email  |

	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'
	
	Then redirect url contains 'id_token'
	Then redirect url contains 'code'
	Then token contains 'iss'
	Then token contains 'aud'
	Then token contains 'exp'
	Then token contains 'iat'
	Then token contains 'azp'
	Then token contains 'c_hash'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'

Scenario: Public subject is returned in id_token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value                         |
	| redirect_uris  | [https://web.com]             |
	| grant_types    | [implicit,authorization_code] |
	| response_types | [token,id_token,code]         |
	| scope          | email                         |
	| subject_type   | public	                     |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value               |
	| response_type | id_token token code |
	| client_id     | $client_id$         |
	| state         | state               |
	| response_mode | query               |
	| scope         | openid email        |
	| redirect_uri  | https://web.com     |
	| ui_locales    | en fr               |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then redirect url contains 'id_token'
	Then redirect url contains 'token'
	Then redirect url contains 'code'
	Then redirect url contains 'ui_locales'
	Then token claim 'sub'='administrator'

Scenario: Pairwise subject is returned in id_token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value                         |
	| redirect_uris  | [https://web.com]             |
	| grant_types    | [implicit,authorization_code] |
	| response_types | [token,id_token,code]         |
	| scope          | email	                     |
	| subject_type   | pairwise                      |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value               |
	| response_type | id_token token code |
	| client_id     | $client_id$         |
	| state         | state               |
	| response_mode | query               |
	| scope         | openid email        |
	| redirect_uri  | https://web.com     |
	| ui_locales    | en fr               |
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then redirect url contains 'id_token'
	Then redirect url contains 'token'
	Then redirect url contains 'code'
	Then redirect url contains 'ui_locales'
	Then token claim 'sub'!='administrator'

Scenario: User-agent is redirected to the login page when elapsed time > authentication time + default client max age
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key             | Value             |
	| redirect_uris   | [https://web.com] |
	| default_max_age | 2                 |
	| grant_types     | [implicit]        |
	| response_types  | [token,id_token]  |
	| scope           | email             |
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email', clientId='$client_id$'
	And add '-10' seconds to authentication instant to user 'administrator'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value           |
	| response_type | id_token        |
	| client_id     | $client_id$     |
	| state         | state           |
	| response_mode | query           |
	| scope         | openid email    |
	| redirect_uri  | https://web.com |
	
	Then redirect url contains 'http://localhost/Authenticate'

Scenario: User-agent is redirected to the login page when elapsed time > to authentication time + max_age parameter
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value             |
	| redirect_uris  | [https://web.com] |
	| grant_types    | [implicit]        |
	| response_types | [token,id_token]  |
	| scope          | email             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email', clientId='$client_id$'
	And add '-10' seconds to authentication instant to user 'administrator'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value           |
	| response_type | id_token        |
	| client_id     | $client_id$     |
	| state         | state           |
	| response_mode | query           |
	| scope         | openid email    |
	| max_age       | 2               |
	| redirect_uri  | https://web.com |

	Then redirect url contains 'http://localhost/Authenticate'
	
Scenario: Identity token must contains an auth_time claim when mentionned as essential
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value             |
	| redirect_uris  | [https://web.com] |
	| grant_types    | [implicit]        |
	| response_types | [token,id_token]  |
	| scope          | email             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                                             |
	| response_type | id_token                                          |
	| client_id     | $client_id$                                       |
	| state         | state                                             |
	| response_mode | query                                             |
	| scope         | openid email                                      |
	| redirect_uri  | https://web.com                                   |
	| nonce         | nonce                                             |
	| claims        | { id_token: { auth_time: { essential : true } } } |

	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'

	Then redirect url contains 'id_token'
	Then token contains 'aud'
	Then token contains 'exp'
	Then token contains 'iat'
	Then token contains 'auth_time'
	Then token claim 'iss'='http://localhost'
	Then token claim 'azp'='$client_id$'
	Then token claim 'nonce'='nonce'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'

Scenario: Use implicit grant-type to get an access token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value             |
	| redirect_uris  | [https://web.com] |
	| grant_types    | [implicit]        |
	| response_types | [token]           |
	| scope          | email             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                                             |
	| response_type | token                                             |
	| client_id     | $client_id$                                       |
	| state         | state                                             |
	| response_mode | query                                             |
	| scope         | openid email                                      |
	| redirect_uri  | https://web.com                                   |

	And extract 'access_token' from callback
	And extract payload from JWS '$access_token$'

	Then redirect url contains 'access_token'
	Then token contains 'aud'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token contains 'auth_time'
	Then token contains 'scope'
	Then token claim 'sub'='administrator'
	Then token claim 'iss'='http://localhost'

Scenario: Use implicit grant-type to get an access and identity token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value             |
	| redirect_uris  | [https://web.com] |
	| grant_types    | [implicit]        |
	| response_types | [token,id_token]  |
	| scope          | email             |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value           |
	| response_type | token id_token  |
	| client_id     | $client_id$     |
	| state         | state           |
	| response_mode | query           |
	| scope         | openid email    |
	| redirect_uri  | https://web.com |
	| nonce         | nonce           |

	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'
	
	Then redirect url contains 'id_token'
	Then redirect url contains 'access_token'
	Then token contains 'iss'
	Then token contains 'aud'
	Then token contains 'exp'
	Then token contains 'iat'
	Then token contains 'azp'
	Then token contains 'at_hash'
	Then token claim 'sub'='administrator'
	Then token claim 'nonce'='nonce'

Scenario: Use hybrid grant-type to get an identity token and authorization code
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value                         |
	| redirect_uris  | [https://web.com]             |
	| grant_types    | [implicit,authorization_code] |
	| response_types | [code,id_token]               |
	| scope          | email                         |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value           |
	| response_type | id_token code   |
	| client_id     | $client_id$     |
	| state         | state           |
	| response_mode | query           |
	| scope         | openid email    |
	| redirect_uri  | https://web.com |
	| nonce         | nonce           |

	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'
	
	Then redirect url contains 'id_token'
	Then redirect url contains 'code'
	Then token contains 'iss'
	Then token contains 'aud'
	Then token contains 'exp'
	Then token contains 'iat'
	Then token contains 'azp'
	Then token contains 'c_hash'
	Then token claim 'sub'='administrator'
	Then token claim 'nonce'='nonce'

Scenario: Check a refresh token is returned when the scope offline_access is used
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value                         |
	| redirect_uris  | [https://web.com]             |
	| grant_types    | [authorization_code]			 |
	| response_types | [code]					     |
	| scope          | email offline_access 		 |
	| subject_type   | public	                     |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email offline_access', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value								 |
	| response_type | code								 |
	| client_id     | $client_id$						 |
	| state         | state								 |
	| response_mode | query								 |
	| scope         | openid offline_access email        |
	| redirect_uri  | https://web.com					 |
	| ui_locales    | en fr								 |

	Then redirect url contains 'code'
	Then redirect url contains 'refresh_token'

Scenario: Check amr and acr claims are present in the identity token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And add authentication class references
	| Name | DisplayName | Amrs |
	| 1    | Level1      | pwd  |

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value                         |
	| redirect_uris  | [https://web.com]             |
	| grant_types    | [implicit,authorization_code] |
	| response_types | [code,id_token]               |
	| scope          | email 						 |
	| subject_type   | public	                     |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value				|
	| response_type | code id_token		|
	| client_id     | $client_id$		|
	| state         | state				|
	| response_mode | query				|
	| scope         | openid email		|
	| redirect_uri  | https://web.com	|
	| acr_values    | 1					|	
	
	And extract 'id_token' from callback
	And extract payload from JWS '$id_token$'
	
	Then token contains 'amr'
	Then token claim 'sub'='administrator'
	Then token claim 'email'='habarthierry@hotmail.fr'
	Then token claim 'acr'='1'
