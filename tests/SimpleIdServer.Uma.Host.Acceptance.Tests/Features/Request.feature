Feature: Request
	Check /reqs endpoint
	
Scenario: Get received pending requests
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
	| Key             | Value                 |
	| resource_scopes | [ "scope1" ]          |
	| subject         | owner                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |
	
	And extract JSON from body
	And extract '_id' from JSON body

	And execute HTTP PUT JSON request 'http://localhost/rreguri/$_id$/permissions'
	| Key           | Value                                                                                                                  |
	| permissions   | [ { claims: [ { name: "sub", value: "user" }, { name: "email", value: "user@hotmail.com" } ], scopes: [ "scope1" ] } ] |
	| Authorization | Bearer $access_token$                                                                                                  |

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| resource_id     | $_id$                 |
	| resource_scopes | [ "scope1"]           |
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
	| Key   | Value           |
	| sub   | requester       |
	| email | user@hotmail.fr |
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| Key                | Value                                                        |
	| client_id          | $client_id$                                                  |
	| client_secret      | $client_secret$                                              |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	| ticket             | $ticket$                                                     |
	| claim_token        | $claim_token$                                                |
	| claim_token_format | http://openid.net/specs/openid-connect-core-1_0.html#IDToken |
	| scope              | scope1                                                       |
		
	And build claim_token
	| Key   | Value           |
	| sub   | owner           |
	| email | user@hotmail.fr |

	And execute HTTP GET against 'http://localhost/reqs/.search/received/me?count=50&startIndex=0' and pass authorization header 'Bearer $claim_token$'
		
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'totalResults'='1'
	Then JSON 'count'='50'
	Then JSON 'startIndex'='0'
	Then JSON 'data[0].requester'='requester'
	Then JSON 'data[0].owner'='owner'
	Then JSON 'data[0].scopes[0]'='scope1'
	Then JSON 'data[0].resource.resource_scopes[0]'='scope1'
	Then JSON 'data[0].resource.icon_uri'='icon'
	Then JSON 'data[0].resource.description#fr'='descriptionFR'
	Then JSON 'data[0].resource.description#en'='descriptionEN'
	Then JSON 'data[0].resource.name#fr'='nom'
	Then JSON 'data[0].resource.name#en'='name'