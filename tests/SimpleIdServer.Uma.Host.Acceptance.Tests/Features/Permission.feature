Feature: Permission
	Check /perm endpoint
	
Scenario: Get permission ticket
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

	Then HTTP status code equals to '201'
	Then JSON exists 'ticket'