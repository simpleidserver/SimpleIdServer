Feature: ExplicitRegistrationErrors
	Check the errors returned during explicit registration

Scenario: Error is returned when wrong content-type is passed
	When execute HTTP POST request 'https://localhost:8080/federation_registration', content-type 'application/json', content '{}'
	
	And extract JSON from body

	Then HTTP status code equals to '400'	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='only entity statement is supported'

Scenario: Error is returned when invalid entity-statement is passed
	When execute HTTP POST request 'https://localhost:8080/federation_registration', content-type 'application/entity-statement+jwt', content 'hello'
	
	And extract JSON from body

	Then HTTP status code equals to '400'	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the entity statement is not correctly formatted'

Scenario: Error is returned when there is no subject in the entity statement
	Given build random entity statement
	| Key           | Value     |
	| state         | state		|

	When execute HTTP POST request 'https://localhost:8080/federation_registration', content-type 'application/entity-statement+jwt', content '$entityStatement$'
	
	And extract JSON from body

	Then HTTP status code equals to '400'	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='subject is required in the entity statement'

Scenario: Error is returned when there is no trusted anchor
	Given build random entity statement
	| Key           | Value     |
	| sub           | sub		|

	When execute HTTP POST request 'https://localhost:8080/federation_registration', content-type 'application/entity-statement+jwt', content '$entityStatement$'
	
	And extract JSON from body

	Then HTTP status code equals to '400'	
	Then JSON 'error'='missing_trust_anchor'
	Then JSON 'error_description'='no trust anchor can be resolved'

Scenario: Error is returned when a random entity statement is generated with a trusted anchor
	Given build random entity statement
	| Key             | Value             |
	| sub             | sub		          |
	| authority_hints | ["http://ta.com"] |

	When execute HTTP POST request 'https://localhost:8080/federation_registration', content-type 'application/entity-statement+jwt', content '$entityStatement$'
	
	And extract JSON from body

	Then HTTP status code equals to '400'	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='impossible to resolve the trust chain'