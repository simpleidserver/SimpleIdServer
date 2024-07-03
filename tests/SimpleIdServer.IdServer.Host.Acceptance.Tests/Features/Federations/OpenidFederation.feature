Feature: OpenidFederation
	Check result returned by the .well-known/openid-federation endpoint

Scenario: Get the openid-federation
	When execute HTTP GET request 'https://localhost:8080/.well-known/openid-federation'
	| Key | Value |

	Then HTTP header has 'Content-Type'='application/entity-statement+jwt'