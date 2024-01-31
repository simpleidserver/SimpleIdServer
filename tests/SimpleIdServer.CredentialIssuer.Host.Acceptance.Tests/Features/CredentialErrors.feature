Feature: CredentialErrors
	Check the errors returned by the credential endpoint

Scenario: format parameter is required when credential_identifiers are not returned from the Token Response
	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key           | Value     |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_request'
	And JSON 'error_description'='the parameter format is missing'

Scenario: proof_type is required when the proof parameter is passed
	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key           | Value     |
	| proof         | { }       |
	| format        | format    |	
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_request'
	And JSON 'error_description'='the parameter proof_type is missing'

Scenario: proof_type must be supported
	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key           | Value                             |
	| proof         | { "proof_type": "invalid" }       |
	| format        | format                            |	
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_request'
	And JSON 'error_description'='the proof format invalid is not supported'

Scenario: proof typ must be equals to openid4vci-proof+jwt
	Given build jwt proof
	| Key | Value   |
	| typ | invalid |
	
	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key           | Value                                        |
	| proof         | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format        | format                                       |	
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_proof'
	And JSON 'error_description'='the proof typ must be equals to openid4vci-proof+jwt'

Scenario: proof kid must be present
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |
	
	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key           | Value                                        |
	| proof         | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format        | format                                       |	
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_proof'
	And JSON 'error_description'='the jwt proof doesn't contain a kid'

Scenario: proof kid must be a did
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |
	| kid | did                  |
	
	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key           | Value                                        |
	| proof         | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format        | format                                       |	
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_proof'
	And JSON 'error_description'='did doesn't have the correct format'

Scenario: credential_identifier must be present when access token contains credential_identifiers
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |
	
	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key           | Value                                        |
	| proof         | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format        | format                                       |	
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_request'
	And JSON 'error_description'='the parameter credential_identifier is missing'