Feature: UMAPermissions
	Check /perm endpoitn

Scenario: get a permisson ticket
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

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| resource_id     | $_id$                 |
	| resource_scopes | [scope1,scope2]       |
	| Authorization   | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '201'
	And JSON exists 'ticket'