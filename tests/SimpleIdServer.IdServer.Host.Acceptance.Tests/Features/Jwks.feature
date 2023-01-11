Feature: Jwks
	Get JWKS and check its content

Scenario: Get Json Web Keys (JWKS)
	When execute HTTP GET request 'https://localhost:8080/jwks'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '200'