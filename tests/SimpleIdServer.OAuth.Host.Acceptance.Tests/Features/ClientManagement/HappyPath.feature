Feature: HappyPath
	Manage a client

Scenario: Add a client
	When execute HTTP POST request 'https://localhost:8080/management/clients'
	| Key           | Value     |
	| client_id     | newClient |
	
	And extract JSON from body

	Then HTTP status code equals to '200'