Feature: TokenErrors
	Check errors returned by /token endpoint

Scenario: Error is returned when ticket parameter is not passed
	When execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='parameter ticket is missing'	

Scenario: Error is returned when claim_token parameter is passed but claim_token_format parameter is missing
	When execute HTTP POST request 'http://localhost/token'
	| Key         | Value                                       |
	| grant_type  | urn:ietf:params:oauth:grant-type:uma-ticket |
	| ticket      | ticket                                      |
	| claim_token | token                                       |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='parameter claim_token_format is missing'	

Scenario: Error is returned when claim_token_format parameter is passed but claim_token parameter is missing
	When execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                       |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket |
	| ticket             | ticket                                      |
	| claim_token_format | format                                      |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='parameter claim_token is missing'	

Scenario: Error is returned when ticket is invalid
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key							| Value												|
	| redirect_uris					| ["https://web.com"]								|
	| grant_types					| ["urn:ietf:params:oauth:grant-type:uma-ticket"]	|
	| response_types				| ["token"]											|
	| token_endpoint_auth_method	| client_secret_post								|
	
	And extract JSON from body
	And extract 'client_id' from JSON body
	And extract 'client_secret' from JSON body
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                       |
	| Key                | Value                                       |
	| client_id          | $client_id$                                 |
	| client_secret      | $client_secret$                             |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket |
	| ticket             | ticket                                      |
	| claim_token        | token                                       |
	| claim_token_format | format                                      |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_grant'
	Then JSON 'error_description'='permission ticket is not correct'	

Scenario: Error is returned when client_token_format is not supported
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
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                       |
	| Key                | Value                                       |
	| client_id          | $client_id$                                 |
	| client_secret      | $client_secret$                             |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket |
	| ticket             | $ticket$                                    |
	| claim_token        | token                                       |
	| claim_token_format | format                                      |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='token format format is invalid'	

Scenario: Error is returned when claim_token is not a JWS token
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

	And execute HTTP POST JSON request 'http://localhost/register'
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
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| Key                | Value                                                        |
	| client_id          | $client_id$                                                  |
	| client_secret      | $client_secret$                                              |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	| ticket             | $ticket$                                                     |
	| claim_token        | token                                                        |
	| claim_token_format | http://openid.net/specs/openid-connect-core-1_0.html#IDToken |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='claim_token parameter is not a JWS token'	
	
Scenario: Error is returned when unsupported scopes are passed
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
	| Key | Value |
	| sub | user  |
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| Key                | Value                                                        |
	| client_id          | $client_id$                                                  |
	| client_secret      | $client_secret$                                              |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	| ticket             | $ticket$                                                     |
	| claim_token        | $claim_token$                                                |
	| claim_token_format | http://openid.net/specs/openid-connect-core-1_0.html#IDToken |
	| scope              | invalid                                                      |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_scope'
	Then JSON 'error_description'='At least one of the scopes included in the request does not match an available scope for any of the resources associated with requested permissions for the permission ticket provided by the client.'	

Scenario: Error is returned when some claims are missing
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
	| Key | Value |
	| sub | user  |
	
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

	And extract JSON from body
	
	Then HTTP status code equals to '401'
	Then JSON 'need_info.ticket'='$ticket$'
	Then JSON 'need_info.redirect_uri'='https://openid.net/'
	Then JSON 'need_info.required_claims[0].claim_token_format'='http://openid.net/specs/openid-connect-core-1_0.html#IDToken'
	Then JSON 'need_info.required_claims[0].name'='email'

Scenario: Error is returned when user is not authorized
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
	| sub   | user1           |
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

	And extract JSON from body
	
	Then HTTP status code equals to '401'
	Then JSON 'request_submitted.ticket'='$ticket$'
	Then JSON 'request_submitted.interval'='5'