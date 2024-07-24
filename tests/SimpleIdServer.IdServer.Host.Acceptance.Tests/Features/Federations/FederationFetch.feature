Feature: FederationFetch
	Check result returned by the federation_fetch endpoint

Scenario: Get a self-signed entity statement
	When execute HTTP GET request 'https://localhost:8080/federation_fetch?iss=https://localhost:8080'
	| Key | Value |

	And extract payload from HTTP body

	Then JWT has 'iss'='https://localhost:8080'
	And JWT has 'sub'='https://localhost:8080'