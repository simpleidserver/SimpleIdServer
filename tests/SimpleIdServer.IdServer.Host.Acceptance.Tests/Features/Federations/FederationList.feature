Feature: FederationList
	Check result returned by the federation_list endpoint

Scenario: Get the list of federation
	When execute HTTP GET request 'https://localhost:8080/federation_list'
	| Key | Value |

	And extract JSON from body
	
	Then HTTP status code equals to '200'