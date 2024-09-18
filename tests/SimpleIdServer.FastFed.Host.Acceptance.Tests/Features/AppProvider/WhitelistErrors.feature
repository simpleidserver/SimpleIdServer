Feature: WhitelistErrors
	Check errors returned during whitelisting

Scenario: Identity Provider url cannot be empty
	When execute HTTP POST JSON request 'http://localhost/fastfed/whitelist'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '400'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Parameter 'url' is missing'


Scenario: Identity Provider Url must be valid
	When execute HTTP POST JSON request 'http://localhost/fastfed/whitelist'
	| Key					| Value |
	| identity_provider_url | URL	|

	And extract JSON from body

	Then HTTP status code equals to '400'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Provider metadata cannot be retrieved'

Scenario: Identity Provider Metadata must be valid
	When execute HTTP POST JSON request 'http://localhost/fastfed/whitelist'
	| Key					| Value					|
	| identity_provider_url | http://localhost/bad	|
	
	And extract JSON from body

	Then HTTP status code equals to '400'	
	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Parameter 'entity_id' is missing from the metadata'
	Then JSON '$.error_descriptions[1]'='Parameter 'provider_domain' is missing from the metadata'
	Then JSON '$.error_descriptions[2]'='Parameter 'organization' is missing from the metadata'
	Then JSON '$.error_descriptions[3]'='Parameter 'phone' is missing from the metadata'
	Then JSON '$.error_descriptions[4]'='Parameter 'email' is missing from the metadata'
	Then JSON '$.error_descriptions[5]'='Parameter 'display_name' is missing from the metadata'
	Then JSON '$.error_descriptions[6]'='Parameter 'license' is missing from the metadata'
	Then JSON '$.error_descriptions[7]'='Parameter 'signing_algorithms' is missing from the metadata'
	Then JSON '$.error_descriptions[8]'='Parameter 'schema_grammars' is missing from the metadata'

Scenario: Provider domain suffix must be valid
	When execute HTTP POST JSON request 'http://localhost/fastfed/whitelist'
	| Key					| Value							|
	| identity_provider_url | http://localhost/badsuffix	|
	
	And extract JSON from body

	Then HTTP status code equals to '400'	
	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='The provider name suffix invalid is not satisfied'

Scenario: Identity provider cannot be duplicated
	When execute HTTP POST JSON request 'http://localhost/fastfed/whitelist'
	| Key					| Value							|
	| identity_provider_url | http://localhost/duplicate	|
	
	And extract JSON from body

	Then HTTP status code equals to '400'	
	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Federation with the identity provider is already configured'

Scenario: Identity provider capabilities must be compatible
	When execute HTTP POST JSON request 'http://localhost/fastfed/whitelist'
	| Key					| Value							|
	| identity_provider_url | http://localhost/incompatible	|
	
	And extract JSON from body

	Then HTTP status code equals to '400'	
	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Provisioning profile invalid:provisioning is not compatible'
	Then JSON '$.error_descriptions[1]'='Schema grammar invalid:schemagrammar is not compatible'
	Then JSON '$.error_descriptions[2]'='Signing algorithm invalid-sigalg is not compatible'