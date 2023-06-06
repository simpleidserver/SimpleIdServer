Feature: DeviceAuthorization
	Check result returned by the authorization device endpoint
	
Scenario: Check result
	When execute HTTP POST request 'https://localhost:8080/device_authorization'
	| Key           | Value				|
	| client_id     | fiftyFiveClient   |
	| scope         | admin             |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And JSON exists 'device_code'
	And JSON exists 'user_code'
	And JSON exists 'verification_uri'
	And JSON exists 'verification_uri_complete'
	And JSON exists 'expires_in'
	And JSON exists 'interval'