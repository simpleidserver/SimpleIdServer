Feature: Management
	Check management endpoint

Scenario: Search OAUTH2.0 clients
	When execute HTTP POST JSON request 'http://localhost/management/clients/.search'
	| Key         | Value |
	| start_index | 0     |
	| count       | 10    |	
	And extract JSON from body
	
	Then HTTP status code equals to '200'