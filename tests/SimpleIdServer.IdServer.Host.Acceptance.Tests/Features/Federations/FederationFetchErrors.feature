Feature: FederationFetchErrors
	Check errors returned by the federation_fetch endpoint

Scenario: Parameter iss is required
	When execute HTTP GET request 'https://localhost:8080/federation_fetch'
	| Key | Value |
	
	And extract JSON from body

	Then HTTP status code equals to '400'

	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the parameter iss is missing'

Scenario: Parameter iss must be valid
	When execute HTTP GET request 'https://localhost:8080/federation_fetch?iss=invalid'
	| Key | Value |
	
	And extract JSON from body

	Then HTTP status code equals to '400'

	And JSON '$.error'='invalid_issuer'
	And JSON '$.error_description'='the issuer is invalid'

Scenario: The entity statement must exists
	When execute HTTP GET request 'https://localhost:8080/federation_fetch?iss=https://localhost:8080&sub=unknown'
	| Key | Value |
	
	And extract JSON from body

	Then HTTP status code equals to '400'

	And JSON '$.error'='not_found'
	And JSON '$.error_description'='the entity statement unknown doesn't exist'