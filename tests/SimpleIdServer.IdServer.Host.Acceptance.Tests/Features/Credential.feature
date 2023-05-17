Feature: Credential
	Check credential endpoint
	
Scenario: get credential
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key            | Value              |
	| grant_type     | client_credentials |
	| client_id      | fiftyNineClient    |
	| client_secret  | password           |
	| scope          | credential_offer   |
	
	And extract JSON from body	
	And extract parameter 'access_token' from JSON body	

	And execute HTTP GET request 'http://localhost/credential_offer/credentialOfferId'
	| Key           | Value                  |
	| Authorization | Bearer $access_token$  |	

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

	Then HTTP status code equals to '200'