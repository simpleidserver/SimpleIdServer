Feature: Credential
	Check the credential endpoint

Scenario: use the jwt_vc_json format and type parameters to get a credential
	Given build jwt proof
	| Key   | Value                |
	| typ   | openid4vci-proof+jwt |
	| nonce | nonce                |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                            | Value                                                    |
	| proof                          | { "proof_type": "jwt", "jwt": "$jwtProof$" }             |
	| format                         | jwt_vc_json                                              |
	| credential_definition          | { "type" : ["VerifiableCredential","UniversityDegree"] } |
		
	And extract JSON from body	

	Then HTTP status code equals to '200'
	And JSON 'c_nonce'='nonce'
	And JSON exists 'credential'

Scenario: use the ldp_vc format and type parameters to get a credential
	Given build jwt proof
	| Key   | Value                |
	| typ   | openid4vci-proof+jwt |
	| nonce | nonce                |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                            | Value                                                    |
	| proof                          | { "proof_type": "jwt", "jwt": "$jwtProof$" }             |
	| format                         | ldp_vc                                                   |
	| credential_definition          | { "type" : ["VerifiableCredential","UniversityDegree"] } |
		
	And extract JSON from body	

	Then HTTP status code equals to '200'
	And JSON 'c_nonce'='nonce'
	And JSON exists 'credential'
	And JSON '$.credential.@context[0]'='https://www.w3.org/2018/credentials/v1'
	And JSON '$.credential.@context[1]'='https://www.w3.org/2018/credentials/examples/v1'
	And JSON exists '$.credential.credentialSubject.id'
	And JSON exists '$.credential.proof.verificationMethod'
	And JSON exists '$.credential.proof.jws'
	And JSON '$.credential.proof.type'='JsonWebSignature2020'
	And JSON '$.credential.proof.proofPurpose'='assertionMethod'

Scenario: use the credential_identifier to get a credential
	Given build jwt proof
	| Key   | Value                |
	| typ   | openid4vci-proof+jwt |
	| nonce | nonce                |
	
	And access token contains one credential identifier 'MasterComputerScience'

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                            | Value                                        |
	| proof                          | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| credential_identifier          | MasterComputerScience                        |
		
	And extract JSON from body	

	Then HTTP status code equals to '200'
	And JSON 'c_nonce'='nonce'
	And JSON exists 'credential'