Feature: DeferredCredentialErrors
	Check the errors returned by the deferred_credential endpoint

Scenario: the parameter transaction_id is required
	When execute HTTP POST JSON request 'http://localhost/deferred_credential'
	| Key           | Value     |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_request'
	And JSON 'error_description'='the parameter transaction_id is missing'

Scenario: the transaction must exists
	When execute HTTP POST JSON request 'http://localhost/deferred_credential'
	| Key            | Value     |
	| transaction_id | invalid   |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_transaction_id'
	And JSON 'error_description'='The Deferred Credential Request contains an invalid transaction_id'

Scenario: the status of the transaction cannot be PENDING
	When execute HTTP POST JSON request 'http://localhost/deferred_credential'
	| Key            | Value                |
	| transaction_id | pendingTransaction   |
		
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='issuance_pending'
	And JSON 'error_description'='The Credential issuance is still pending'