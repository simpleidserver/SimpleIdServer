Feature: Configuration
	Get the OAUTHConfiguration and check its content

Scenario: Get the configuration
	When execute HTTP GET request 'http://localhost/.well-known/oauth-authorization-server'
	| Key			| Value													|

	Then HTTP status code equals to '200'