Feature: UmaResource
	Check /rreguri endpoint
		
Scenario: Add a UMA resource
	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value         |
	| resource_scopes | [ "scope1" ]  |
	| subject         | user1         |
	| icon_uri        | icon          |
	| name#fr         | nom           |
	| name#en         | name          |
	| description#fr  | descriptionFR |
	| description#en  | descriptionEN |
	| type            | type          |

	And extract JSON from body

	Then HTTP status code equals to '201'
	Then JSON exists '_id'
	Then JSON exists 'user_access_policy_uri'

Scenario: Get a UMA resource
	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value         |
	| resource_scopes | [ "scope1" ]  |
	| subject         | user1         |
	| icon_uri        | icon          |
	| name#fr         | nom           |
	| name#en         | name          |
	| description#fr  | descriptionFR |
	| description#en  | descriptionEN |
	| type            | type          |
	
	And extract JSON from body
	And extract '_id' from JSON body
	And execute HTTP GET request 'http://localhost/rreguri/$_id$'
	
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
	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value         |
	| resource_scopes | [ "scope1" ]  |
	| subject         | user1         |
	| icon_uri        | icon          |
	| name#fr         | nom           |
	| name#en         | name          |
	| description#fr  | descriptionFR |
	| description#en  | descriptionEN |
	| type            | type          |
	
	And extract JSON from body
	And extract '_id' from JSON body
	And execute HTTP DELETE request 'http://localhost/rreguri/$_id$'
	
	Then HTTP status code equals to '204'

Scenario: Add UMA permissions	
	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value         |
	| resource_scopes | [ "scope1" ]  |
	| subject         | user1         |
	| icon_uri        | icon          |
	| name#fr         | nom           |
	| name#en         | name          |
	| description#fr  | descriptionFR |
	| description#en  | descriptionEN |
	| type            | type          |
	
	And extract JSON from body
	And extract '_id' from JSON body
	
	And execute HTTP PUT JSON request 'http://localhost/rreguri/$_id$/permissions'
	| Key         | Value                                      |
	| permissions | [ { subject: "user1", scopes: [ "scope" ]] |
		
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists '_id'

Scenario: Delete UMA permissions
	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value         |
	| resource_scopes | [ "scope1" ]  |
	| subject         | user1         |
	| icon_uri        | icon          |
	| name#fr         | nom           |
	| name#en         | name          |
	| description#fr  | descriptionFR |
	| description#en  | descriptionEN |
	| type            | type          |
	
	And extract JSON from body
	And extract '_id' from JSON body
	
	And execute HTTP PUT JSON request 'http://localhost/rreguri/$_id$/permissions'
	| Key         | Value                                      |
	| permissions | [ { subject: "user1", scopes: [ "scope" ]] |

	And execute HTTP DELETE request 'http://localhost/rreguri/$_id$/permissions'

	Then HTTP status code equals to '204'

Scenario: Get UMA permissions
	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value         |
	| resource_scopes | [ "scope1" ]  |
	| subject         | user1         |
	| icon_uri        | icon          |
	| name#fr         | nom           |
	| name#en         | name          |
	| description#fr  | descriptionFR |
	| description#en  | descriptionEN |
	| type            | type          |
	
	And extract JSON from body
	And extract '_id' from JSON body
	
	And execute HTTP PUT JSON request 'http://localhost/rreguri/$_id$/permissions'
	| Key         | Value                                         |
	| permissions | [ { subject: "user1", scopes: [ "scope" ] } ] |

	And execute HTTP GET request 'http://localhost/rreguri/$_id$/permissions'

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'permissions[0].subject'='user1'
	Then JSON 'permissions[0].scopes[0]'='scope'