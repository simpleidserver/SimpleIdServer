Feature: UMAResource
	Check the endpoint /rreguri	

Scenario: add UMA resource
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
	| resource_scopes | [scope1,scope2]       |
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
	And JSON exists '_id'
	And JSON exists 'user_access_policy_uri'

Scenario: get UMA resource
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
	| resource_scopes | [scope1,scope2]       |
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

	And execute HTTP GET request 'http://localhost/rreguri/$_id$'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON exists 'resource_scopes'
	And JSON 'icon_uri'='icon'
	And JSON 'name#fr'='nom'
	And JSON 'name#en'='name'
	And JSON 'description#fr'='descriptionFR'
	And JSON 'description#en'='descriptionEN'
	And JSON 'type'='type'

Scenario: delete UMA resource
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
	| resource_scopes | [scope1,scope2]       |
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

	And execute HTTP DELETE request 'http://localhost/rreguri/$_id$'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |
	
	Then HTTP status code equals to '204'

Scenario: add UMA permissions
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
	| resource_scopes | [scope1,scope2]       |
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
	| Key             | Value                                                                           |
	| Authorization   | Bearer $access_token$                                                           |
	| permissions     | [ { "claims": [ { "name": "sub", "value": "user" } ], "scopes": [ "scope" ] } ] |

	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON exists '_id'

Scenario: delete UMA permissions
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
	| resource_scopes | [scope1,scope2]       |
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
	
	And execute HTTP POST JSON request 'http://localhost/rreguri/$_id$/permissions'
	| Key             | Value                                                                           |
	| Authorization   | Bearer $access_token$                                                           |
	| permissions     | [ { "claims": [ { "name": "sub", "value": "user" } ], "scopes": [ "scope" ] } ] |

	And execute HTTP DELETE request 'http://localhost/rreguri/$_id$'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |

	Then HTTP status code equals to '204'
