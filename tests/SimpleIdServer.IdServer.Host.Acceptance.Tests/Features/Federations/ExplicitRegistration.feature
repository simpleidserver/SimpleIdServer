Feature: ExplicitRegistration
	Check explicit registration

Scenario: Check access token can be returned when using explicit client registration
	Given build entity statement for RP

	When execute HTTP POST request 'https://localhost:8080/federation_registration', content-type 'application/entity-statement+jwt', content '$entityStatement$'
	
	And extract payload from HTTP body

	Then HTTP header has 'Content-Type'='application/entity-statement+jwt'
	And HTTP status code equals to '200'
	And JWT has 'iss'='https://localhost:8080'
	And JWT has 'sub'='http://rp.com'
	And JWT has 'trust_anchor_id'='http://ta.com'