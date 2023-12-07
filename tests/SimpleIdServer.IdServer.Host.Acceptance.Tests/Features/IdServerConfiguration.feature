Feature: IdServerConfiguration
	Get the IdServerConfiguration and check its content

Scenario: Get the configuration
	When execute HTTP GET request 'https://localhost:8080/.well-known/idserver-configuration'
	| Key | Value |
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.amrs[0]'='console'