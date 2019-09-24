Feature: Jwks
	Get the JWKS and check its content

Scenario: Get the jwks
	When execute HTTP GET request 'http://localhost/jwks'
	| Key			| Value													|

	Then HTTP status code equals to '200'