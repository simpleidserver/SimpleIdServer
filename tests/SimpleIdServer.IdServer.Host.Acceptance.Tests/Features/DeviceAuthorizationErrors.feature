Feature: DeviceAuthorizations
	Check errors returned by the device authorization endpoint	
	
Scenario: Request URI must be valid
	When execute HTTP POST request 'https://localhost:8080/device_authorization'
	| Key           | Value        			|

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter client_id'

Scenario: Client must exists
	When execute HTTP POST request 'https://localhost:8080/device_authorization'
	| Key          | Value   |
	| client_id    | invalid |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='unknown client invalid'

Scenario: Scopes must be supported
	When execute HTTP POST request 'https://localhost:8080/device_authorization'
	| Key          | Value        |
	| client_id    | twelveClient |
	| scope        | invalid      |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='scopes invalid are not supported'

