Feature: CredentialIssuer
	Check the .well-known/openid-credential-issuer endpoint

Scenario: Configuration is returned
	When execute HTTP GET request 'http://localhost/.well-known/openid-credential-issuer'
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And HTTP header has 'Content-Type'='application/json'
	And JSON 'credential_issuer'='http://localhost'
	And JSON 'credential_endpoint'='http://localhost/credential'
	And JSON '$.credentials_supported.UniversityDegree_ldp_vc.@context[0]'='https://www.w3.org/2018/credentials/v1'
	And JSON '$.credentials_supported.UniversityDegree_ldp_vc.@context[1]'='https://www.w3.org/2018/credentials/examples/v1'
	And JSON '$.credentials_supported.UniversityDegree_ldp_vc.type[0]'='VerifiableCredential'
	And JSON '$.credentials_supported.UniversityDegree_ldp_vc.type[1]'='UniversityDegree'
	And JSON '$.credentials_supported.UniversityDegree_ldp_vc.format'='ldp_vc'
	And JSON '$.credentials_supported.UniversityDegree_ldp_vc.credentialSubject.given_name.display[0].name'='Given Name'
	And JSON '$.credentials_supported.UniversityDegree_ldp_vc.credentialSubject.given_name.display[0].locale'='en-US'