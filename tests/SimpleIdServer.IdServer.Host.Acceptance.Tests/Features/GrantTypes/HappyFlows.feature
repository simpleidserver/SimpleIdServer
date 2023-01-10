Feature: HappyFlows
	Run happy flows for all the grant types

Scenario: Use 'client_credentials' grant type to get an access token
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value                                |
	| client_id     | firstClient						   |
	| client_secret | password                             |
	| scope         | firstScope	                       |
	| grant_type    | client_credentials                   |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And JSON '$.scope'='firstScope'
	And JSON '$.token_type'='Bearer'
	And JSON '$.expires_in'='1800'
	And access_token audience contains 'firstClient'
	And access_token audience contains 'secondClient'
	And access_token scope contains 'firstScope'
	And access_token alg equals to 'RS256'
	And access_token kid equals to 'keyid'

Scenario: Use 'password' grant type to get an access token
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        	|
	| client_id     | secondClient	|
	| client_secret | password     	|
	| scope         | firstScope	|
	| grant_type    | password		|
	| username		| user			|	
	| password      | password      |

	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.scope'='firstScope'
	And JSON '$.token_type'='Bearer'
	And JSON '$.expires_in'='1800'
	And access_token audience contains 'firstClient'
	And access_token audience contains 'secondClient'
	And access_token scope contains 'firstScope'
	And access_token alg equals to 'RS256'
	And access_token kid equals to 'keyid'

Scenario: Use 'authorization_code' grant type to get an access token
	Given authenticate a user
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | thirdClient           |
	| state         | state                 |
	| redirect_uri  | http://localhost:8080 |
	| response_mode | query                 |
	| scope         | secondScope			|

	And extract parameter 'code' from redirect url
	And extract parameter 'state' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|	

	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.scope'='secondScope'
	And JSON '$.token_type'='Bearer'
	And JSON '$.expires_in'='1800'
	And parameter 'state'='state'

Scenario: Use 'refresh_token' grant type to get an access token
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| grant_type    | client_credentials |
	| scope         | secondScope        |
	| client_id     | sixClient          |
	| client_secret | password           |

	And extract JSON from body
	And extract parameter '$.refresh_token' from JSON body into 'refreshToken'
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value          |
	| grant_type    | refresh_token  |
	| refresh_token | $refreshToken$ |
	| client_id     | sixClient      |
	| client_secret | password       |

	And extract JSON from body
	Then HTTP status code equals to '200'
	And JSON '$.scope'='secondScope'
	And JSON '$.token_type'='Bearer'
	And JSON '$.expires_in'='1800'

Scenario: No access token is issued then resulting claims are returned in the ID Token
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | fourteenClient        |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	
	And extract parameter 'id_token' from redirect url