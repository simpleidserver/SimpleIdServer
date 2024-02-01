Feature: Credential
	Check the credential endpoint

Scenario: use the format and type parameters to get a credential
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                            | Value                                                    |
	| proof                          | { "proof_type": "jwt", "jwt": "$jwtProof$" }             |
	| format                         | jwt_vc_json                                              |
	| credential_definition          | { "type" : ["VerifiableCredential","UniversityDegree"] } |
		
	And extract JSON from body