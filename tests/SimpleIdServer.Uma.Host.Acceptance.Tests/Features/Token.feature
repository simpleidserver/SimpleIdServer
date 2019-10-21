Feature: Token
	Check /token endpoint

Scenario: Use uma-ticket grant type to get an access token
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key                        | Value                  |
	| redirect_uris              | ["https://web.com"]    |
	| grant_types                | ["client_credentials"] |
	| token_endpoint_auth_method | client_secret_post     |
	| scope                      | uma_protection         |

	And extract JSON from body
	And extract 'client_id' from JSON body
	And extract 'client_secret' from JSON body

	And execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | $client_id$        |
	| client_secret | $client_secret$    |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |

	And extract JSON from body
	And extract 'access_token' from JSON body

	And execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                           |
	| resource_scopes | [ "scope1", "scop2", "scope3" ] |
	| subject         | user1                           |
	| icon_uri        | icon                            |
	| name#fr         | nom                             |
	| name#en         | name                            |
	| description#fr  | descriptionFR                   |
	| description#en  | descriptionEN                   |
	| type            | type                            |
	| Authorization   | Bearer $access_token$           |
	
	And extract JSON from body
	And extract '_id' from JSON body
	
	And execute HTTP PUT JSON request 'http://localhost/rreguri/$_id$/permissions'
	| Key           | Value                                                                              |
	| permissions   | [ { claims: [ { name: "sub", value: "user" } ], scopes: [ "scope1", "scope3" ] } ] |
	| Authorization | Bearer $access_token$                                                              |

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| resource_id     | $_id$                 |
	| resource_scopes | [ "scope1", "scope3"] |
	| Authorization   | Bearer $access_token$ |

	And extract JSON from body
	And extract 'ticket' from JSON body

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key							| Value												|
	| redirect_uris					| ["https://web.com"]								|
	| grant_types					| ["urn:ietf:params:oauth:grant-type:uma-ticket"]	|
	| response_types				| ["token"]											|
	| token_endpoint_auth_method	| client_secret_post								|
	
	And extract JSON from body
	And extract 'client_id' from JSON body
	And extract 'client_secret' from JSON body

	And build claim_token
	| Key | Value |
	| sub | user  |

	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| client_id          | $client_id$                                                  |
	| client_secret      | $client_secret$                                              |
	| scope              | scope1 scope3                                                |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	| ticket             | $ticket$                                                     |
	| claim_token        | $claim_token$                                                |
	| claim_token_format | http://openid.net/specs/openid-connect-core-1_0.html#IDToken |
	
	And extract JSON from body
	And extract 'access_token' from JSON body
	And extract payload from JWS '$access_token$'
	
	Then HTTP status code equals to '200'	
	Then JSON exists 'refresh_token'
	Then JSON exists 'access_token'
	Then JSON 'token_type'='Bearer'
	Then JSON 'scope[0]'='scope1'	
	Then token contains 'aud'
	Then token contains 'iss'
	Then token contains 'scope'
	Then token contains 'iat'
	Then token contains 'exp'
	Then token claim 'permissions[0].resource_id'='$_id$'
	Then token claim 'permissions[0].resource_scopes[0]'='scope1'
	Then token claim 'permissions[0].resource_scopes[1]'='scope3'