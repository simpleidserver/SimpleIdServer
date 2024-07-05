Feature: FederationFetch
	Check result returned by the federation_fetch endpoint

Scenario: Get the entity-statement of the RP
	When execute HTTP GET request 'https://localhost:8080/federation_fetch?sub=http://rp.com&iss=https://localhost:8080'
	| Key | Value |

	And extract JSON from body

	Then JSON '$.[0]'='http://rp.com'