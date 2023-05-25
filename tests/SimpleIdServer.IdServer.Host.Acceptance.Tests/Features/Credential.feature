Feature: Credential
	Check credential endpoint
	
Scenario: use pre authorized code to get a credential
	Given authenticate a user

	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value             |
	| wallet_client_id       | fiftyNineClient   |
	| credential_template_id | credTemplate      |

	And extract query parameters into JSON
	And extract query parameter 'credential_offer' into JSON
	And extract parameter '$.grants.urn:ietf:params:oauth:grant-type:pre-authorized_code.pre-authorized_code' from JSON body into 'preAuthorizedCode'

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                     | Value                                                                                                                        |
	| grant_type              | urn:ietf:params:oauth:grant-type:pre-authorized_code                                                                         |
	| client_id               | fiftyNineClient                                                                                                              |
	| pre-authorized_code     | $preAuthorizedCode$                                                                                                          |
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential", "UniversityDegree"] }           |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract parameter 'c_nonce' from JSON body
	And extract payload from JWT '$access_token$'	
	And build proof
	| Key     | Value                 |
	| typ     | openid4vci-proof+jwt  |
	| c_nonce | $c_nonce$             |

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                                         |
	| Authorization | Bearer $access_token$                         |
	| format        | jwt_vc_json                                   |
	| types         | ["VerifiableCredential","UniversityDegree"]   |
	| proof         | { "proof_type": "jwt", "jwt": "$proof$" }     |
	
	And extract JSON from body

	Then JSON exists 'credential'
	And JSON exists 'c_nonce'
	And JSON exists 'c_nonce_expires_in'
	And JSON 'format'='jwt_vc_json'

Scenario: use authorization code to get a credential