Feature: BCCallbackErrors
	Check errors returned by the /bc-callback endpoint	

Scenario: authorization request must exists
	When execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                  |
	| auth_req_id   | invalid                |

	And extract JSON from body

	Then HTTP status code equals to '404'
	And JSON 'error'='invalid_request'
	And JSON 'error_description'='the back channel authorization invalid doesn't exist'