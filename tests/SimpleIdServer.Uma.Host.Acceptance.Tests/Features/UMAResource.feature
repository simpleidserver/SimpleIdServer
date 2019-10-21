Feature: UmaResource
	Check /rreguri endpoint
		
Scenario: Add a UMA resource
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
	| subject         | user1                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '201'
	Then JSON exists '_id'
	Then JSON exists 'user_access_policy_uri'

Scenario: Get a UMA resource
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
	| subject         | user1                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |
	
	And extract JSON from body
	And extract '_id' from JSON body
	And execute HTTP GET against 'http://localhost/rreguri/$_id$' and pass authorization header 'Bearer $access_token$'
	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'resource_scopes'
	Then JSON 'icon_uri'='icon'
	Then JSON 'name#fr'='nom'
	Then JSON 'name#en'='name'
	Then JSON 'description#fr'='descriptionFR'
	Then JSON 'description#en'='descriptionEN'
	Then JSON 'type'='type'

Scenario: Delete a UMA resource
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
	| subject         | user1                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |
	
	And extract JSON from body
	And extract '_id' from JSON body	
	And execute HTTP DELETE against 'http://localhost/rreguri/$_id$' and pass authorization header 'Bearer $access_token$'
	
	Then HTTP status code equals to '204'

Scenario: Add UMA permissions	
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

	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                 |
	| resource_scopes | [ "scope1" ]          |
	| subject         | user1                 |
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
	| Key           | Value                                                                   |
	| permissions   | [ { claims: [ { name: "sub", value: "user" } ], scopes: [ "scope" ] } ] |
	| Authorization | Bearer $access_token$                                                   |
		
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists '_id'

Scenario: Delete UMA permissions	
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

	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                 |
	| resource_scopes | [ "scope1" ]          |
	| subject         | user1                 |
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
	| Key           | Value                                                                   |
	| permissions   | [ { claims: [ { name: "sub", value: "user" } ], scopes: [ "scope" ] } ] |
	| Authorization | Bearer $access_token$                                                   |
	
	And execute HTTP DELETE against 'http://localhost/rreguri/$_id$' and pass authorization header 'Bearer $access_token$'

	Then HTTP status code equals to '204'

Scenario: Get UMA permissions
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
	| subject         | user1                 |
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
	| Key           | Value                                                                   |
	| permissions   | [ { claims: [ { name: "sub", value: "user" } ], scopes: [ "scope" ] } ] |
	| Authorization | Bearer $access_token$                                                   |

	And execute HTTP GET against 'http://localhost/rreguri/$_id$/permissions' and pass authorization header 'Bearer $access_token$'

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'permissions[0].claims[0].name'='sub'
	Then JSON 'permissions[0].claims[0].value'='user'
	Then JSON 'permissions[0].scopes[0]'='scope'