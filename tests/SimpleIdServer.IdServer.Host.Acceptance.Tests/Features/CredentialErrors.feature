Feature: CredentialErrors
	Check errors returned by the credential endpoint
	
Scenario: access token is required
	When execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                 |

	And extract JSON from body
	
	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'
	
Scenario: access token must be valid
	When execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                 |
	| Authorization | Bearer INVALID        |

	And extract JSON from body

	Then HTTP status code equals to '401'
	And JSON 'error'='invalid_token'
	And JSON 'error_description'='either the access token has been revoked or is invalid'

Scenario: format is required
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

	And extract query parameters into JSON
	And extract query parameter 'credential_offer' into JSON
	And extract parameter '$.grants.urn:ietf:params:oauth:grant-type:pre-authorized_code.pre-authorized_code' from JSON body into 'preAuthorizedCode'

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                     | Value                                                                                                                        |
	| grant_type              | urn:ietf:params:oauth:grant-type:pre-authorized_code                                                                         |
	| client_id               | fiftyNineClient                                                                                                              |
	| pre-authorized_code     | $preAuthorizedCode$                                                                                                          |
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential", "UniversityDegreeCredential"] } |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |
	
	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the parameter format is missing'

Scenario: format must be supported
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

	And extract query parameters into JSON
	And extract query parameter 'credential_offer' into JSON
	And extract parameter '$.grants.urn:ietf:params:oauth:grant-type:pre-authorized_code.pre-authorized_code' from JSON body into 'preAuthorizedCode'

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                     | Value                                                                                                                        |
	| grant_type              | urn:ietf:params:oauth:grant-type:pre-authorized_code                                                                         |
	| client_id               | fiftyNineClient                                                                                                              |
	| pre-authorized_code     | $preAuthorizedCode$                                                                                                          |
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential", "UniversityDegreeCredential"] } |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |
	| format        | notSupported          |
	
	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the format notSupported is not supported'

Scenario: types is required (jwt_vc_json)
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

	And extract query parameters into JSON
	And extract query parameter 'credential_offer' into JSON
	And extract parameter '$.grants.urn:ietf:params:oauth:grant-type:pre-authorized_code.pre-authorized_code' from JSON body into 'preAuthorizedCode'

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                     | Value                                                                                                                        |
	| grant_type              | urn:ietf:params:oauth:grant-type:pre-authorized_code                                                                         |
	| client_id               | fiftyNineClient                                                                                                              |
	| pre-authorized_code     | $preAuthorizedCode$                                                                                                          |
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential", "UniversityDegreeCredential"] } |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |
	| format        | jwt_vc_json           |
	
	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the parameter types is missing'

Scenario: types contained in the request must be valid
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

	And extract query parameters into JSON
	And extract query parameter 'credential_offer' into JSON
	And extract parameter '$.grants.urn:ietf:params:oauth:grant-type:pre-authorized_code.pre-authorized_code' from JSON body into 'preAuthorizedCode'

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                     | Value                                                                                                                        |
	| grant_type              | urn:ietf:params:oauth:grant-type:pre-authorized_code                                                                         |
	| client_id               | fiftyNineClient                                                                                                              |
	| pre-authorized_code     | $preAuthorizedCode$                                                                                                          |
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential", "UniversityDegreeCredential"] } |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                        |
	| Authorization | Bearer $access_token$        |
	| format        | jwt_vc_json                  |
	| types         | ["firstcred", "secondcred"]  |
	
	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='no credential found'

Scenario: types contained in the request must be similar to the one present in the authorization_details
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

	And extract query parameters into JSON
	And extract query parameter 'credential_offer' into JSON
	And extract parameter '$.grants.urn:ietf:params:oauth:grant-type:pre-authorized_code.pre-authorized_code' from JSON body into 'preAuthorizedCode'

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                     | Value                                                                                                                        |
	| grant_type              | urn:ietf:params:oauth:grant-type:pre-authorized_code                                                                         |
	| client_id               | fiftyNineClient                                                                                                              |
	| pre-authorized_code     | $preAuthorizedCode$                                                                                                          |
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential", "InvalidCredential"] }          |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                                                   |
	| Authorization | Bearer $access_token$                                   |
	| format        | jwt_vc_json                                             |
	| types         | ["VerifiableCredential","UniversityDegree"]             |
	
	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='you are not authorized to access to UniversityDegree'

Scenario: proof_type is required
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

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
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                                                   |
	| Authorization | Bearer $access_token$                                   |
	| format        | jwt_vc_json                                             |
	| types         | ["VerifiableCredential", "UniversityDegree"]            |
	| proof         | { }                                                     |
	
	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the parameter proof_type is missing'

Scenario: proof_type must be supported
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

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
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                                                   |
	| Authorization | Bearer $access_token$                                   |
	| format        | jwt_vc_json                                             |
	| types         | ["VerifiableCredential", "UniversityDegree"]            |
	| proof         | { "proof_type": "invalid" }                             |
	
	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the proof type invalid is unknown'

Scenario: JWT parameter is required
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

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
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                                                   |
	| Authorization | Bearer $access_token$                                   |
	| format        | jwt_vc_json                                             |
	| types         | ["VerifiableCredential", "UniversityDegree"]            |
	| proof         | { "proof_type": "jwt" }                                 |
	
	And extract JSON from body

	Then JSON 'error'='invalid_proof'
	And JSON 'error_description'='the parameter jwt is missing'

Scenario: JWT parameter must be valid
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |

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
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                                                   |
	| Authorization | Bearer $access_token$                                   |
	| format        | jwt_vc_json                                             |
	| types         | ["VerifiableCredential", "UniversityDegree"]            |
	| proof         | { "proof_type": "jwt", "jwt": "invalid" }               |
	
	And extract JSON from body

	Then JSON 'error'='invalid_proof'
	And JSON 'error_description'='the proof is not a well formed JWT token'

Scenario: JWT Type must be equals to openid4vci-proof+jwt
	Given authenticate a user

	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | fiftyNineClient    |
	| credential_template_id | credTemplate       |
	
	And build proof
	| Key     | Value |
	| c_nonce | test  |

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
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                                         |
	| Authorization | Bearer $access_token$                         |
	| format        | jwt_vc_json                                   |
	| types         | [VerifiableCredential,UniversityDegree]       |
	| proof         | { "proof_type": "jwt", "jwt": "$proof$" }     |
	
	And extract JSON from body

	Then JSON 'error'='invalid_proof'
	And JSON 'error_description'='the proof typ must be equals to openid4vci-proof+jwt'

Scenario: JWT must contains a valid NONCE
	Given authenticate a user

	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value                                                    |
	| wallet_client_id       | fiftyNineClient                                          |
	| credential_template_id | credTemplate                                             |
	
	And build proof
	| Key     | Value                                                    |
	| typ     | openid4vci-proof+jwt                                     |
	| c_nonce | invalid                                                  |

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
	And extract payload from JWT '$access_token$'

	And execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                                         |
	| Authorization | Bearer $access_token$                         |
	| format        | jwt_vc_json                                   |
	| types         | [VerifiableCredential,UniversityDegree]       |
	| proof         | { "proof_type": "jwt", "jwt": "$proof$" }     |
	
	And extract JSON from body

	Then JSON 'error'='invalid_proof'
	And JSON 'error_description'='the credential nonce (c_nonce) is not valid'