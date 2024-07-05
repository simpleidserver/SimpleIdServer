Feature: FederationList
	Check result returned by the federation_list endpoint

Scenario: Get the openid-federation
	When execute HTTP GET request 'https://localhost:8080/federation_list'
	| Key | Value |

	And extract JSON from body

	Then JSON '$.[0]'='http://rp.com'