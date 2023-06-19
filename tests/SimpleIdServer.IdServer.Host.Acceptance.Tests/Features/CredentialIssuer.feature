Feature: CredentialIssuer
	Get the CredentialIssuer and check its content

Scenario: Get the configuration
	When execute HTTP GET request 'https://localhost:8080/.well-known/openid-credential-issuer'
	| Key | Value |
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON 'credential_endpoint'='https://localhost:8080/credential'