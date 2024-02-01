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
	
	And access token contains one credential identifier 'ItMaster'

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key           | Value                                        |
	| proof         | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format        | format                                       |	
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_request'
	And JSON 'error_description'='the parameter credential_identifier is missing'

Scenario: format parameter must not be present when the credential_identifier parameter is passed
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |
	
	And access token contains one credential identifier 'ItMaster'

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                   | Value                                        |
	| proof                 | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format                | format                                       |	
	| credential_identifier | ItMaster                                     |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_request'
	And JSON 'error_description'='the credential_identifier parameter cannot be used with the format parameter'

Scenario: credential_identifier parameter must be present in the access token
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |
	
	And access token contains one credential identifier 'ItMaster'

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                   | Value                                        |
	| proof                 | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| credential_identifier | Invalid                                      |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_request'
	And JSON 'error_description'='the credential_identifier parameter is not valid'

Scenario: format must be supported
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                   | Value                                        |
	| proof                 | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format                | format                                       |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='unsupported_credential_format'
	And JSON 'error_description'='the credential format format is not supported'

Scenario: the credential_definition parameter is required when format is equals to jwt_vc_json
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                   | Value                                        |
	| proof                 | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format                | jwt_vc_json                                  |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_request'
	And JSON 'error_description'='the credential type cannot be extracted'

Scenario: the type parameter is required when format is equals to jwt_vc_json
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                   | Value                                        |
	| proof                 | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format                | jwt_vc_json                                  |
	| credential_definition | { }                                          |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_credential_request'
	And JSON 'error_description'='the credential type cannot be extracted'

Scenario: the credential type must be supported
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                   | Value                                        |
	| proof                 | { "proof_type": "jwt", "jwt": "$jwtProof$" } |
	| format                | jwt_vc_json                                  |
	| credential_definition | { "type" : ["VerifiableCredential","Type"] } |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='unsupported_credential_type'
	And JSON 'error_description'='the credential type Type is not supported'

Scenario: the alg parameter is required when credential_response_encryption is passed
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                            | Value                                                    |
	| proof                          | { "proof_type": "jwt", "jwt": "$jwtProof$" }             |
	| format                         | jwt_vc_json                                              |
	| credential_definition          | { "type" : ["VerifiableCredential","UniversityDegree"] } |
	| credential_response_encryption | { }                                                      |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_encryption_parameters'
	And JSON 'error_description'='the parameter alg is missing'

Scenario: the enc parameter is required when credential_response_encryption is passed
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                            | Value                                                    |
	| proof                          | { "proof_type": "jwt", "jwt": "$jwtProof$" }             |
	| format                         | jwt_vc_json                                              |
	| credential_definition          | { "type" : ["VerifiableCredential","UniversityDegree"] } |
	| credential_response_encryption | { "alg": "alg" }                                         |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_encryption_parameters'
	And JSON 'error_description'='the parameter enc is missing'

Scenario: the jwk parameter is required when credential_response_encryption is passed
	Given build jwt proof
	| Key | Value                |
	| typ | openid4vci-proof+jwt |

	When execute HTTP POST JSON request 'http://localhost/credential'
	| Key                            | Value                                                    |
	| proof                          | { "proof_type": "jwt", "jwt": "$jwtProof$" }             |
	| format                         | jwt_vc_json                                              |
	| credential_definition          | { "type" : ["VerifiableCredential","UniversityDegree"] } |
	| credential_response_encryption | { "alg": "alg", "enc": "enc" }                           |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_encryption_parameters'
	And JSON 'error_description'='the parameter jwk is missing'