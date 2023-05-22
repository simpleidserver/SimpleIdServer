Feature: CredentialOffer
	Check result returned by credential_offer API
	
Scenario: user shares his credentials
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

	And extract query parameters into JSON
	And extract query parameter 'credential_offer' into JSON

	Then JSON exists 'credential_issuer'
	And JSON exists 'grants'
	And JSON exists 'credentials'