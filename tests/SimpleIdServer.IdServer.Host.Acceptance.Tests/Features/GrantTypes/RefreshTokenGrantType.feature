Feature: RefreshTokenGrantType
	Check all the alternatives scenarios in refresh_token grant-type

Scenario: parameter resource and scope are always coming from the original request
	Given authenticate a user
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                        |
	| response_type | code                         |
	| client_id     | fortySixClient               |
	| state         | state                        |
	| response_mode | query                        |
	| redirect_uri  | http://localhost:8080        |
	| nonce         | nonce                        |
	| resource      | https://cal.example.com      |	
	| scope         | offline_access               |
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fortySixClient               |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| redirect_uri  | http://localhost:8080	       |

	And extract JSON from body
	And extract parameter '$.refresh_token' from JSON body into 'refreshToken'	
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fortySixClient               |
	| client_secret | password     			       |
	| grant_type    | refresh_token      	       |
	| refresh_token | $refreshToken$               |

	And extract JSON from body
	
	Then JSON 'scope'='admin calendar offline_access'
	And access_token audience contains 'https://cal.example.com'
	And access_token contains the claim 'scope'='admin'
	And access_token contains the claim 'scope'='calendar'

Scenario: the same refresh token can be used twice
	Given authenticate a user
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                        |
	| response_type | code                         |
	| client_id     | seventyFiveClient            |
	| state         | state                        |
	| response_mode | query                        |
	| redirect_uri  | http://localhost:8080        |
	| nonce         | nonce                        |
	| resource      | https://cal.example.com      |	
	| scope         | offline_access               |
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | seventyFiveClient            |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| redirect_uri  | http://localhost:8080	       |

	And extract JSON from body
	And extract parameter '$.refresh_token' from JSON body into 'refreshToken'	
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | seventyFiveClient            |
	| client_secret | password     			       |
	| grant_type    | refresh_token      	       |
	| refresh_token | $refreshToken$               |
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | seventyFiveClient            |
	| client_secret | password     			       |
	| grant_type    | refresh_token      	       |
	| refresh_token | $refreshToken$               |

	And extract JSON from body
	
	Then JSON 'scope'='admin calendar offline_access'
	And access_token audience contains 'https://cal.example.com'
	And access_token contains the claim 'scope'='admin'
	And access_token contains the claim 'scope'='calendar'