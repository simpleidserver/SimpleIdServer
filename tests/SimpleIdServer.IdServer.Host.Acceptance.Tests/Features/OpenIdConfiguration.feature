Feature: OpenIdConfiguration
	Get the OpenIdConfiguration and check its content

Scenario: Get the configuration
	When execute HTTP GET request 'https://localhost:8080/.well-known/openid-configuration'
	| Key | Value |
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON 'userinfo_endpoint'='https://localhost:8080/userinfo'