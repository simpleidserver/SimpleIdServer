Feature: Grants
	Check happy flow implementation : https://bitbucket.org/openid/fapi/src/master/fapi-grant-management.md
	
Scenario: grant is returned when valid access token is passed
	Given authenticate a user
	
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                                                                |
	| response_type           | code                                                                                                 |
	| client_id               | fortySevenClient                                                                                     |
	| state                   | state                                                                                                |
	| response_mode           | query                                                                                                |
	| redirect_uri            | http://localhost:8080                                                                                |
	| nonce                   | nonce                                                                                                |
	| claims                  | { "id_token": { "acr": { "essential" : true, "value": "urn:openbanking:psd2:ca" } } }                | 
	| resource                | https://cal.example.com                                                                              |
	| grant_management_action | create                                                                                               |
	| scope                   | grant_management_query                                                                               |
	| authorization_details   |  { "type" : "secondDetails", "locations": [ "https://cal.example.com" ], "actions": [ "read" ] }     |

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
	| Authorization | Bearer $accessToken$  |

	And extract JSON from body

	Then JSON '$.claims[0]'='acr'
	And JSON '$.scopes[0].scope'='admin'
	And JSON '$.scopes[1].scope'='calendar'
	And JSON '$.scopes[2].scope'='grant_management_query'
	And JSON '$.authorization_details[0].type'='secondDetails'
	And JSON '$.authorization_details[0].locations[0]'='https://cal.example.com'
	And JSON '$.authorization_details[0].actions[0]'='read'

Scenario: grant is returned when a valid refreshed access token is passed
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
	| scope                   | grant_management_query offline_access												  |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | fortySevenClient      |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|
	
	And extract JSON from body
	And extract parameter '$.refresh_token' from JSON body into 'refreshToken'	
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value            |
	| grant_type    | refresh_token    |
	| refresh_token | $refreshToken$   |
	| client_id     | fortySevenClient |
	| client_secret | password         |
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'
	And extract parameter '$.grant_id' from JSON body into 'grantId'

	And execute HTTP GET request 'http://localhost/grants/$grantId$'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |

	And extract JSON from body

	Then JSON '$.claims[0]'='acr'
	And JSON '$.scopes[0].scope'='admin'
	And JSON '$.scopes[1].scope'='calendar'

Scenario: revoke a grant and check access token is revoked
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
	| scope                   | grant_management_query grant_management_revoke                                        |

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

	And execute HTTP DELETE request 'http://localhost/grants/$grantId$'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |	

	And execute HTTP POST request 'https://localhost:8080/token_info'
	| Key           | Value            |
	| client_id     | fortySevenClient |
	| client_secret | password         |
	| token         | $accessToken$    |	
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.active'='false'

Scenario: merge the authorization_details types and check the grant is updated
	Given authenticate a user
	
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                                                                                                |
	| response_type           | code                                                                                                                                 |
	| client_id               | fiftyEightClient                                                                                                                     |
	| state                   | state                                                                                                                                |
	| response_mode           | query                                                                                                                                |
	| redirect_uri            | http://localhost:8080                                                                                                                |
	| nonce                   | nonce                                                                                                                                |
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential"], "locations" : [ "http://localhost" ] } |
	| grant_management_action | create                                                                                                                               |
	| scope                   | grant_management_query                                                                                                               |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | fiftyEightClient      |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|
	
	And extract JSON from body
	And extract parameter '$.grant_id' from JSON body into 'grantId'	
	
	And execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                                                                                                                              |
	| response_type           | code                                                                                                                                                               |
	| client_id               | fiftyEightClient                                                                                                                                                   |
	| state                   | state                                                                                                                                                              |
	| response_mode           | query                                                                                                                                                              |
	| redirect_uri            | http://localhost:8080                                                                                                                                              |
	| nonce                   | nonce                                                                                                                                                              |
	| grant_management_action | merge                                                                                                                                                              |
	| scope                   | grant_management_revoke grant_management_query                                                                                                                     |
	| grant_id                | $grantId$                                                                                                                                                          |	
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential", "UniversityDegreeCredential"], "locations" : [ "http://localhost" ] } |
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | fiftyEightClient      |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|

	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'

	And execute HTTP GET request 'http://localhost/grants/$grantId$'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |

	And extract JSON from body

	Then JSON '$.authorization_details[0].type'='openid_credential'
	And JSON '$.authorization_details[0].format'='jwt_vc_json'
	And JSON '$.authorization_details[0].locations[0]'='http://localhost'
	And JSON '$.authorization_details[0].types[0]'='VerifiableCredential'
	And JSON '$.authorization_details[0].types[1]'='UniversityDegreeCredential'

Scenario: merge the permissions consented by the user in the actual request with those already exist within the grant
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
	And extract parameter '$.grant_id' from JSON body into 'grantId'	
	
	And execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                 |
	| response_type           | code                                                  |
	| client_id               | fortySevenClient                                      |
	| state                   | state                                                 |
	| response_mode           | query                                                 |
	| redirect_uri            | http://localhost:8080                                 |
	| nonce                   | nonce                                                 |
	| claims                  | { "id_token": { "iss": { "essential" : false } } }    | 
	| resource                | https://contacts.example.com                          |
	| grant_management_action | merge                                                 |
	| scope                   | grant_management_revoke grant_management_query        |
	| grant_id                | $grantId$                                             |
	
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

	And execute HTTP GET request 'http://localhost/grants/$grantId$'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |

	And extract JSON from body

	Then JSON '$.claims[0]'='acr'
	And JSON '$.claims[1]'='iss'
	And JSON '$.scopes[0].scope'='admin'
	And JSON '$.scopes[0].resources[0]'='https://cal.example.com'
	And JSON '$.scopes[0].resources[1]'='https://contacts.example.com'
	And JSON '$.scopes[1].scope'='calendar'
	And JSON '$.scopes[1].resources[0]'='https://cal.example.com'
	And JSON '$.scopes[1].resources[1]'='https://contacts.example.com'
	And JSON '$.scopes[2].scope'='grant_management_query'
	And JSON '$.scopes[3].scope'='grant_management_revoke'	

Scenario: change the grant to be ONLY the permissions requested by the client and consented by the user in the actual request
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
	| scope                   | grant_management_revoke                                                               |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | fortySevenClient      |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|
	
	And extract JSON from body
	And extract parameter '$.grant_id' from JSON body into 'grantId'	
	
	And execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                 |
	| response_type           | code                                                  |
	| client_id               | fortySevenClient                                      |
	| state                   | state                                                 |
	| response_mode           | query                                                 |
	| redirect_uri            | http://localhost:8080                                 |
	| nonce                   | nonce                                                 |
	| claims                  | { "id_token": { "iss": { "essential" : false } } }    | 
	| grant_management_action | replace                                               |
	| scope                   | grant_management_query                                |
	| grant_id                | $grantId$                                             |
	
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

	And execute HTTP GET request 'http://localhost/grants/$grantId$'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |

	And extract JSON from body

	Then JSON '$.claims[0]'='iss'
	Then JSON '$.scopes[0].scope'='grant_management_query'