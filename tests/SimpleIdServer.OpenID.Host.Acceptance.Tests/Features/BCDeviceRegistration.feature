Feature: BCDeviceRegistration
	Check /bc-device-registration endpoint

Scenario: Update 'device_registration_token'
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| grant_types                  | [implicit]        |
	| response_types               | [id_token]        |
	| scope                        | openid email role |
	| id_token_signed_response_alg | none              |
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| response_type | id_token          |
	| client_id     | $client_id$       |
	| state         | state             |
	| response_mode | query             |
	| scope         | openid email role |
	| redirect_uri  | https://web.com   |
	| ui_locales    | en fr             |
	| nonce         | nonce             |
	
	And extract 'id_token' from callback

	When execute HTTP POST JSON request 'http://localhost/bc-device-registration'
	| Key                       | Value      |
	| id_token_hint             | $id_token$ |
	| device_registration_token | device     |
	
	And extract JSON from body

	Then HTTP status code equals to '204'