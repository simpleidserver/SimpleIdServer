Feature: AuthorizationCodeGrantType
	Check all the alternatives scenarios in code grant-type	
	
Scenario: resource parameter can be overridden and scopes 'admin', 'calendar' are returned
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
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fortySixClient               |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| redirect_uri  | http://localhost:8080	       |	
	| resource      | https://cal.example.com      |
	| resource      | https://contacts.example.com |

	And extract JSON from body

	Then HTTP status code equals to '200'
	
	And JSON 'scope'='admin calendar'
	And access_token audience contains 'https://cal.example.com'
	And access_token audience contains 'https://contacts.example.com'
	And access_token contains the claim 'scope'='admin'
	And access_token contains the claim 'scope'='calendar'

Scenario: scope of a resource can be filtered
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
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fortySixClient               |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| redirect_uri  | http://localhost:8080	       |	
	| resource      | https://cal.example.com      |
	| resource      | https://contacts.example.com |
	| scope         | admin                        |

	And extract JSON from body

	Then HTTP status code equals to '200'
	And access_token audience contains 'https://cal.example.com'
	And access_token audience contains 'https://contacts.example.com'
	And access_token contains the claim 'scope'='admin'
	And access_token doesn't contain the claim 'scope'='calendar'

Scenario: scopes 'admin', 'calendar' are returned thanks to the original request
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
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fortySixClient               |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| redirect_uri  | http://localhost:8080	       |

	And extract JSON from body
	
	Then JSON 'scope'='admin calendar'
	And access_token audience contains 'https://cal.example.com'
	And access_token contains the claim 'scope'='admin'
	And access_token contains the claim 'scope'='calendar'