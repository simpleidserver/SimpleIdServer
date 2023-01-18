Feature: Grants
	Check happy flow
	
Scenario: grant can be returned
	Given authenticate a user
	
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                                                 |
	| response_type           | code                                                                                  |
	| client_id               | fortySevenClient                                                                      |
	| state                   | state                                                                                 |
	| response_mode           | query                                                                                 |
	| redirect_uri            | http://localhost:8080                                                                 |
	| nonce                   | nonce                                                                                 |
	| claims                  | { "id_token": { "acr": { "essential" : true, "value": "urn:openbanking:psd2:ca" } } } | 
	| resource                | https://cal.example.com                                                               |
	| grant_management_action | create                                                                                |
	| scope                   | grant_management_query                                                                |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | fortySevenClient      |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'
	And extract parameter '$.grant_id' from JSON body into 'grantId'

	And execute HTTP GET request 'http://localhost/grants/$grantId$'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$ |

	And extract JSON from body