Feature: UserInfoErrors
	Check errors returned by the userinfo endpoint

Scenario: access token is required (HTTP GET)
	When execute HTTP GET request 'http://localhost/userinfo'
	| Key | Value |
	
	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing token'

Scenario: access token is required (HTTP POST)
	When execute HTTP POST request 'http://localhost/userinfo'
	| Key | Value |
	
	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing token'

Scenario: content-Type cannot be equals to 'application/json'
	When execute HTTP POST JSON request 'http://localhost/userinfo'
	| Key | Value |
	
	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the content-type is not correct'

Scenario: access token must be valid
	When execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value          |
	| Authorization | Bearer rnd rnd |
	
	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing token'

Scenario: access token must be a JWT
	When execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value      |
	| Authorization | Bearer rnd |
	
	And extract JSON from body
	
	Then JSON 'error'='invalid_token'
	Then JSON 'error_description'='bad token'

Scenario: user must exists
	Given build access_token and sign with the key 'keyid'
	| Key | Value   |
	| sub | unknown |

	When execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	Then HTTP status code equals to '401'

Scenario: client identifier presents in the access token must be valid
	Given build access_token and sign with the key 'keyid'
	| Key       | Value   |
	| sub       | user    |
	| client_id | invalid |	

	When execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |
	
	And extract JSON from body
	
	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='unknown client invalid'

Scenario: consent must be confirmed by the end-user
	Given build access_token and sign with the key 'keyid'
	| Key       | Value       |
	| sub       | user        |
	| client_id | thirdClient |	
	| scope     | profile     |

	When execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |
	
	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='no consent has been accepted'

Scenario: rejected access token cannot be used
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                            |
	| response_type | code                             |
	| client_id     | thirtySevenClient                |
	| state         | state                            |
	| response_mode | query                            |
	| scope         | openid email role                |
	| redirect_uri  | http://localhost:8080            |
	| nonce         | nonce                            |
	| display       | popup                            |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirtySevenClient     |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|	

	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	
	And execute HTTP POST request 'https://localhost:8080/token/revoke'
	| Key           | Value             |
	| token         | $access_token$    |
	| client_id     | thirtySevenClient |
	| client_secret | password          |

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |
	
	And extract JSON from body
	
	Then JSON 'error'='invalid_token'
	Then JSON 'error_description'='access token has been rejected'