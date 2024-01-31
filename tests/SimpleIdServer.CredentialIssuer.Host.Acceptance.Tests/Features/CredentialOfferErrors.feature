Feature: CredentialOfferErrors
	Check the errors returned by the credential_offer endpoint

Scenario: grants parameter is required
	When execute HTTP POST JSON request 'http://localhost/credential_offer'
	| Key           | Value     |
	| Authorization | Bearer AT |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_offer_request'
	And JSON 'error_description'='the parameter grants is missing'

Scenario: credential_configuration_ids parameter is required
	When execute HTTP POST JSON request 'http://localhost/credential_offer'
	| Key           | Value     |
	| Authorization | Bearer AT |
	| grants        | ["grant"] |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_offer_request'
	And JSON 'error_description'='the parameter credential_configuration_ids is missing'

Scenario: sub parameter is required
	When execute HTTP POST JSON request 'http://localhost/credential_offer'
	| Key                          | Value     |
	| Authorization                | Bearer AT |
	| grants                       | ["grant"] |
	| credential_configuration_ids | ["id"]    |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_offer_request'
	And JSON 'error_description'='the parameter sub is missing'

Scenario: grant must be valid
	When execute HTTP POST JSON request 'http://localhost/credential_offer'
	| Key                          | Value     |
	| Authorization                | Bearer AT |
	| grants                       | ["grant"] |
	| credential_configuration_ids | ["id"]    |
	| sub                          | user      |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_offer_request'
	And JSON 'error_description'='the grant types grant are not supported'

Scenario: grant must be supported
	When execute HTTP POST JSON request 'http://localhost/credential_offer'
	| Key                          | Value     |
	| Authorization                | Bearer AT |
	| grants                       | ["grant"] |
	| credential_configuration_ids | ["id"]    |
	| sub                          | user      |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_offer_request'
	And JSON 'error_description'='the grant types grant are not supported'

Scenario: credential configuration must exists
	When execute HTTP POST JSON request 'http://localhost/credential_offer'
	| Key                          | Value                     |
	| Authorization                | Bearer AT                 |
	| grants                       | ["authorization_code"]    |
	| credential_configuration_ids | ["id"]                    |
	| sub                          | user                      |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_offer_request'
	And JSON 'error_description'='the credentials id are not supported'