Feature: UmaResourceErrors
	Check errors returned by the /rreguri endpoint
		
Scenario: Error is returned when trying to get an unknown UMA resource
	When execute HTTP GET request 'http://localhost/rreguri/1'
	
	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'

Scenario: Error is returned when resource_scopes parameter is not passed in the HTTP POST request
	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='parameter resource_scopes is missing'	

Scenario: Error is returned when subject parameter is not passed in the HTTP POST request
	When execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value        |
	| resource_scopes | [ "scope1" ] |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='parameter subject is missing'	

Scenario: Error is returned when resource_scopes parameter is not passed in the HTTP PUT request
	When execute HTTP PUT JSON request 'http://localhost/rreguri/id'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='parameter resource_scopes is missing'
		
Scenario: Error is returned when trying to update an unknown UMA resource
	When execute HTTP PUT JSON request 'http://localhost/rreguri/id'
	| Key             | Value        |
	| resource_scopes | [ "scope1" ] |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'

Scenario: Error is returned when trying to remove an unknown UMA resource
	When execute HTTP DELETE request 'http://localhost/rreguri/id'

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'

Scenario: Error is returned when permissions parameter is not passed in the HTTP PUT request
	When execute HTTP PUT JSON request 'http://localhost/rreguri/id/permissions'
	| Key | Value |

	And extract JSON from body
		
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='parameter permissions is missing'	

Scenario: Error is returned when trying to add permissions to unknown resource
	When execute HTTP PUT JSON request 'http://localhost/rreguri/id/permissions'
	| Key         | Value                                      |
	| permissions | [ { subject: "user1", scopes: [ "scope" ]] |

	And extract JSON from body
		
	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'

Scenario: Error is returned when trying to remove permissions from unknown resource
	When execute HTTP DELETE request 'http://localhost/rreguri/id/permissions'

	And extract JSON from body
		
	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'

Scenario: Error is returned when trying to get permissions from unknown resource
	When execute HTTP GET request 'http://localhost/rreguri/id/permissions'

	And extract JSON from body
		
	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'