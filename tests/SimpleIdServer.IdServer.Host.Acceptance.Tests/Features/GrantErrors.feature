Feature: GrantErrors
	Check errors returned by grants API
	
Scenario: access token is required
	When execute HTTP GET request 'http://localhost/grants/id'
	| Key           | Value                 |

	And extract JSON from body
	
	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'
	
Scenario: grant must exists
	When execute HTTP GET request 'http://localhost/grants/id'
	| Key           | Value                 |
	| Authorization | Bearer AT             |

	And extract JSON from body
	
	Then HTTP status code equals to '404'
	And JSON 'error'='invalid_target'
	And JSON 'error_description'='the grant id doesn't exist'
	
Scenario: access token must be valid
	Given authenticate a user

	When execute HTTP GET request 'http://localhost/grants/consentId'
	| Key           | Value           |
	| Authorization | Bearer INVALID  |

	And extract JSON from body	
	
	Then HTTP status code equals to '401'
	And JSON 'error'='invalid_token'
	And JSON 'error_description'='either the access token has been revoked or is invalid'
	
Scenario: only the same client can query the grant
	Given authenticate a user
	
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                                                 |
	| response_type           | code                                                                                  |
	| client_id               | fortyEightClient                                                                      |
	| state                   | state                                                                                 |
	| response_mode           | query                                                                                 |
	| redirect_uri            | http://localhost:8080                                                                 |
	| nonce                   | nonce                                                                                 |
	| claims                  | { "id_token": { "acr": { "essential" : true, "value": "urn:openbanking:psd2:ca" } } } | 
	| resource                | https://cal.example.com                                                               |
	| scope                   | grant_management_query                                                                |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | fortyEightClient      |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'

	And execute HTTP GET request 'http://localhost/grants/consentId'
	| Key           | Value                |
	| Authorization | Bearer $accessToken$ |

	And extract JSON from body	
	
	Then HTTP status code equals to '401'
	And JSON 'error'='invalid_token'
	And JSON 'error_description'='the client fortyEightClient is not authorized to access to perform operations on the grant'