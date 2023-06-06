Feature: CredentialOfferErrors
	Check errors returned by credential_offer API
	
Scenario: wallet_client_id parameter is required (share)
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key | Value |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the parameter wallet_client_id is missing'

Scenario: credential_template_id parameter is required (share)
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key              | Value  |
	| wallet_client_id | wallet |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the parameter credential_template_id is missing'

Scenario: credential template must exists (share)
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value    |
	| wallet_client_id       | unknown  |
	| credential_template_id | unknown  |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the credential template unknown doesn't exist'

Scenario: the wallet must exists (share)
	Given authenticate a user
	
	When execute HTTP POST JSON request 'http://localhost/credential_offer/share'
	| Key                    | Value              |
	| wallet_client_id       | unknown            |
	| credential_template_id | credentialOfferId  |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the credential template credentialOfferId doesn't exist'