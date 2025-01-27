Feature: UMAGrantTypeErrors
	Check errors returned when using 'urn:ietf:params:oauth:grant-type:uma-ticket' grant-type	

Scenario: ticket parameter is required
	When execute HTTP POST request 'http://localhost/token'
	| Key        | Value                                       |
	| grant_type | urn:ietf:params:oauth:grant-type:uma-ticket |

	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter ticket'

Scenario: claim_token_format parameter is required when claim_token is passed
	When execute HTTP POST request 'http://localhost/token'
	| Key         | Value                                       |
	| grant_type  | urn:ietf:params:oauth:grant-type:uma-ticket |
	| ticket      | ticket                                      |
	| claim_token | token                                       |
	
	And extract JSON from body	

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter claim_token_format'

Scenario: claim_token parameter is required when claim_token_format is passed
	When execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                       |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket |
	| ticket             | ticket                                      |
	| claim_token_format | format                                      |
	
	And extract JSON from body	

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter claim_token'

Scenario: ticket must be valid
	When execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                       |
	| client_id          | fiftyThreeClient                            |
	| client_secret      | password                                    |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket |
	| ticket             | ticket                                      |
	| claim_token        | token                                       |
	| claim_token_format | format                                      |	

	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='permission ticket is not correct'

Scenario: client_token_format must be valid
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                 |
	| resource_scopes | ["scope1"]            |
	| subject         | user1                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |	
	
	And extract JSON from body
	And extract parameter '_id' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| resource_id     | $_id$                 |
	| resource_scopes | ["scope1"]            |
	| Authorization   | Bearer $access_token$ |
	
	And extract JSON from body
	And extract parameter 'ticket' from JSON body	
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                       |
	| client_id          | fiftyThreeClient                            |
	| client_secret      | password                                    |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket |
	| ticket             | $ticket$                                    |
	| claim_token        | token                                       |
	| claim_token_format | format                                      |

	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='token format format is invalid'

Scenario: claim_token must be a valid JWT
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                 |
	| resource_scopes | ["scope1"]            |
	| subject         | user1                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |	
	
	And extract JSON from body
	And extract parameter '_id' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| resource_id     | $_id$                 |
	| resource_scopes | ["scope1"]            |
	| Authorization   | Bearer $access_token$ |
	
	And extract JSON from body
	And extract parameter 'ticket' from JSON body	
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| client_id          | fiftyThreeClient                                             |
	| client_secret      | password                                                     |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	| ticket             | $ticket$                                                     |
	| claim_token        | token                                                        |
	| claim_token_format | http://openid.net/specs/openid-connect-core-1_0.html#IDToken |

	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='JSON Web Token cannot be read'

Scenario: scope must be supported
	Given build JWS id_token_hint and sign with the key 'keyid'
	| Key       | Value             |
	| sub       | random            |
	| iss       | http://localhost  |

	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                 |
	| resource_scopes | ["scope1"]            |
	| subject         | user1                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |	
	
	And extract JSON from body
	And extract parameter '_id' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| resource_id     | $_id$                 |
	| resource_scopes | ["scope1"]             |
	| Authorization   | Bearer $access_token$ |
	
	And extract JSON from body
	And extract parameter 'ticket' from JSON body	
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| client_id          | fiftyThreeClient                                             |
	| client_secret      | password                                                     |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	| ticket             | $ticket$                                                     |
	| claim_token        | $id_token_hint$                                              |
	| claim_token_format | http://openid.net/specs/openid-connect-core-1_0.html#IDToken |
	| scope              | invalid                                                      |

	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_scope'
	And JSON '$.error_description'='At least one of the scopes included in the request does not match an available scope for any of the resources associated with requested permissions for the permission ticket provided by the client.'

Scenario: claim_token must contains the claims
	Given build JWS id_token_hint and sign with the key 'keyid'
	| Key       | Value             |
	| sub       | user              |
	| iss       | http://localhost  |

	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                 |
	| resource_scopes | ["scope1"]            |
	| subject         | user1                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |	
	
	And extract JSON from body
	And extract parameter '_id' from JSON body	

	And execute HTTP PUT JSON request 'http://localhost/rreguri/$_id$/permissions'
	| Key           | Value                                                                                                                                  |
	| permissions   | [ { "claims": [ { "name": "sub", "value": "user" }, { "name": "email", "value": "user@hotmail.com" } ], "scopes": [ "scope1" ] } ]     |
	| Authorization | Bearer $access_token$                                                                                                                  |

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| resource_id     | $_id$                 |
	| resource_scopes | ["scope1"]            |
	| Authorization   | Bearer $access_token$ |
	
	And extract JSON from body
	And extract parameter 'ticket' from JSON body	
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| client_id          | fiftyThreeClient                                             |
	| client_secret      | password                                                     |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	| ticket             | $ticket$                                                     |
	| claim_token        | $id_token_hint$                                              |
	| claim_token_format | http://openid.net/specs/openid-connect-core-1_0.html#IDToken |
	| scope              | scope1                                                       |

	And extract JSON from body

	Then HTTP status code equals to '401'
	And JSON '$.need_info.required_claims[0].claim_token_format'='http://openid.net/specs/openid-connect-core-1_0.html#IDToken'
	And JSON '$.need_info.required_claims[0].name'='email'

Scenario: User must be authorized
	Given build JWS id_token_hint and sign with the key 'keyid'
	| Key   | Value            |
	| sub   | user1            |
	| email | user@hotmail.fr  |
	| iss   | http://localhost |

	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                 |
	| resource_scopes | ["scope1"]            |
	| subject         | user1                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |	
	
	And extract JSON from body
	And extract parameter '_id' from JSON body	

	And execute HTTP PUT JSON request 'http://localhost/rreguri/$_id$/permissions'
	| Key           | Value                                                                                                                                  |
	| permissions   | [ { "claims": [ { "name": "sub", "value": "user" }, { "name": "email", "value": "user@hotmail.com" } ], "scopes": [ "scope1" ] } ]     |
	| Authorization | Bearer $access_token$                                                                                                                  |

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| resource_id     | $_id$                 |
	| resource_scopes | ["scope1"]            |
	| Authorization   | Bearer $access_token$ |
	
	And extract JSON from body
	And extract parameter 'ticket' from JSON body	
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| client_id          | fiftyThreeClient                                             |
	| client_secret      | password                                                     |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	| ticket             | $ticket$                                                     |
	| claim_token        | $id_token_hint$                                              |
	| claim_token_format | http://openid.net/specs/openid-connect-core-1_0.html#IDToken |
	| scope              | scope1                                                       |

	And extract JSON from body

	Then HTTP status code equals to '401'
	And JSON '$.request_submitted.ticket'='$ticket$'
	And JSON '$.request_submitted.interval'='5'