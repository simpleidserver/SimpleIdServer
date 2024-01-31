Feature: CredentialOffer
	Get credential offer

Scenario: Create a credential offer
	When execute HTTP POST JSON request 'http://localhost/credential_offer'
	| Key                          | Value                        |
	| Authorization                | Bearer AT                    |
	| grants                       | ["authorization_code"]       |
	| credential_configuration_ids | ["UniversityDegree_ldp_vc"]  |
	| sub                          | user                         |
		
	And extract JSON from body

	Then HTTP status code equals to '201'
	And JSON exists '$.id'
	And JSON exists '$.sub'
	And JSON exists '$.create_datetime'
	And JSON exists '$.offer.grants.authorization_code.issuer_state'
	And JSON exists '$.offer_uri'