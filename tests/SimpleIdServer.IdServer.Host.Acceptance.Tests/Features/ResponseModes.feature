Feature: ResponseModes
	Check response modes

Scenario: JWT response is returned as a fragment parameter
	Given authenticate a user
	
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | token					|
	| client_id     | fiftyFourClient       |
	| state         | state                 |
	| response_mode | fragment.jwt          |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| scope         | openid profile        |	
	
	And extract parameter 'response' from redirect url fragment
	And extract payload from JWT '$response$'

	Then JWT has 'state'='state'
	Then JWT contains 'access_token'
	Then JWT contains 'exp'
	Then JWT contains 'iat'