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

Scenario: Use 'authorization_code' grant type to get an access token and use PAR
	Given authenticate a user
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | thirdClient           |
	| state         | state                 |
	| redirect_uri  | http://localhost:8080 |
	| response_mode | query                 |
	| scope         | secondScope			|

	And extract JSON from body
	And extract parameter 'request_uri' from JSON body

	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value          |
	| client_id     | thirdClient    |
	| request_uri   | $request_uri$  |

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

Scenario: Use 'authorization_code' grant type to get an access token and use PAR and RAR
	Given authenticate a user
	When execute HTTP POST request 'https://localhost:8080/par'
	| Key                     | Value                                                                                                |
	| response_type           | code token                                                                                           |
	| client_id               | fiftyFiveClient                                                                                      |
	| state                   | state                                                                                                |
	| response_mode           | query                                                                                                |
	| redirect_uri            | http://localhost:8080                                                                                |
	| nonce                   | nonce                                                                                                |
	| authorization_details   |  { "type" : "secondDetails", "locations": [ "https://cal.example.com" ], "actions": [ "read" ] }     |

	And extract JSON from body
	And extract parameter 'request_uri' from JSON body

	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value              |
	| client_id     | fiftyFiveClient    |
	| request_uri   | $request_uri$      |

	And extract parameter 'code' from redirect url
	And extract parameter 'state' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | fiftyFiveClient       |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|	

	And extract JSON from body	
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	Then JWT has authorization_details type 'secondDetails'
	And JWT has authorization_details action 'read'

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

Scenario: Use 'urn:openid:params:grant-type:ciba' grant type to get an identity token (POLL mode)
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

	And execute HTTP POST request 'https://localhost:8080/bc-authorize'
	| Key                       | Value            |
	| client_id                 | fortyNineClient  |
	| client_secret             | password         |
	| scope                     | admin calendar   |
	| login_hint                | user             |
	| user_code                 | password         |

	And extract JSON from body
	And extract parameter 'auth_req_id' from JSON body

	And execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                  |
	| Authorization | Bearer $id_token$      |
	| auth_req_id   | $auth_req_id$          |
	| action        | 0                      |

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyNineClient                   |
	| client_secret        | password                          |
	| auth_req_id          | $auth_req_id$                     |
	
	And extract JSON from body
	Then HTTP status code equals to '200'
	And JSON exists 'access_token'	

Scenario: Use 'urn:openid:params:grant-type:ciba' grant type to get an identity token (PUSH mode)
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

	And execute HTTP POST request 'https://localhost:8080/bc-authorize'
	| Key                       | Value                                |
	| client_id                 | fiftyOneClient                       |
	| client_secret             | password                             |
	| scope                     | admin calendar                       |
	| login_hint                | user                                 |
	| user_code                 | password                             |
	| client_notification_token | 04bcf708-dfba-4719-a3d3-b213322e2c38 |

	And extract JSON from body
	And extract parameter 'auth_req_id' from JSON body

	And execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                  |
	| Authorization | Bearer $id_token$      |
	| auth_req_id   | $auth_req_id$          |
	| action        | 0                      |

	And polls until notification is received
	
	Then JSON exists 'access_token'

Scenario: Use 'urn:openid:params:grant-type:ciba' grant type to get an identity token (PING mode)
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

	And execute HTTP POST request 'https://localhost:8080/bc-authorize'
	| Key                       | Value                                |
	| client_id                 | fiftyTwoClient                       |
	| client_secret             | password                             |
	| scope                     | admin calendar                       |
	| login_hint                | user                                 |
	| user_code                 | password                             |
	| client_notification_token | 04bcf708-dfba-4719-a3d3-b213322e2c38 |

	And extract JSON from body
	And extract parameter 'auth_req_id' from JSON body

	And execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                  |
	| Authorization | Bearer $id_token$      |
	| auth_req_id   | $auth_req_id$          |
	| action        | 0                      |

	And polls until notification is received
	And extract parameter 'auth_req_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fiftyTwoClient                    |
	| client_secret        | password                          |
	| auth_req_id          | $auth_req_id$                     |
	
	And extract JSON from body
	Then HTTP status code equals to '200'
	And JSON exists 'access_token'	
	
Scenario: Use 'urn:ietf:params:oauth:grant-type:uma-ticket' grant type to get an access token
	Given build JWS id_token_hint and sign with the key 'keyid'
	| Key   | Value           |
	| sub   | user            |
	| email | user@hotmail.fr |

	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                 |
	| resource_scopes | [scope1,scope2]       |
	| subject         | user1                 |
	| icon_uri        | icon                  |
	| name#fr         | nom                   |
	| name#en         | name                  |
	| description#fr  | descriptionFR         |
	| description#en  | descriptionEN         |
	| type            | type                  |
	| Authorization   | Bearer $access_token$ |	
	
	And extract JSON from body
	And extract parameter '_id' from JSON body	

	And execute HTTP PUT JSON request 'http://localhost/rreguri/$_id$/permissions'
	| Key           | Value                                                                                                                                            |
	| permissions   | [ { "claims": [ { "name": "sub", "value": "user" }, { "name": "email", "value": "user@hotmail.com" } ], "scopes": [ "scope1", "scope2" ] } ]     |
	| Authorization | Bearer $access_token$                                                                                                                            |

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| resource_id     | $_id$                 |
	| resource_scopes | [scope1,scope2]       |
	| Authorization   | Bearer $access_token$ |
	
	And extract JSON from body
	And extract parameter 'ticket' from JSON body	
	
	And execute HTTP POST request 'http://localhost/token'
	| Key                | Value                                                        |
	| client_id          | fiftyThreeClient                                             |
	| client_secret      | password                                                     |
	| grant_type         | urn:ietf:params:oauth:grant-type:uma-ticket                  |
	| ticket             | $ticket$                                                     |
	| claim_token        | $id_token_hint$                                              |
	| claim_token_format | http://openid.net/specs/openid-connect-core-1_0.html#IDToken |
	| scope              | scope1 scope2                                                |

	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	Then HTTP status code equals to '200'
	And JSON exists 'access_token'
	And JSON exists 'refresh_token'
	And access_token has permission to access to the resource id '$_id$'
	And access_token has permission to access to the scope 'scope1'
	And access_token has permission to access to the scope 'scope2'

Scenario: Use 'urn:openid:params:grant-type:ciba' grant type to get an identity token (POLL mode) with authorization_details
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

	And execute HTTP POST request 'https://localhost:8080/bc-authorize'
	| Key                       | Value                                                                                                |
	| client_id                 | fortyNineClient                                                                                      |
	| client_secret             | password                                                                                             |
	| login_hint                | user                                                                                                 |
	| user_code                 | password                                                                                             |
	| authorization_details     |  { "type" : "secondDetails", "locations": [ "https://cal.example.com" ], "actions": [ "read" ] }     |

	And extract JSON from body
	And extract parameter 'auth_req_id' from JSON body

	And execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                  |
	| Authorization | Bearer $id_token$      |
	| auth_req_id   | $auth_req_id$          |
	| action        | 0                      |

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyNineClient                   |
	| client_secret        | password                          |
	| auth_req_id          | $auth_req_id$                     |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	Then HTTP status code equals to '200'
	And JWT has authorization_details type 'secondDetails'
	And JWT has authorization_details action 'read'

Scenario: Use 'urn:ietf:params:oauth:grant-type:pre-authorized_code' grant type to get an access token with authorization details
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key            | Value              |
	| grant_type     | client_credentials |
	| client_id      | fiftyNineClient    |
	| client_secret  | password           |
	| scope          | credential_offer   |
	
	And extract JSON from body	
	And extract parameter 'access_token' from JSON body	

	And execute HTTP GET request 'http://localhost/credential_offer/credentialOfferId'
	| Key           | Value                  |
	| Authorization | Bearer $access_token$  |	

	And extract query parameter 'credential_offer' into JSON
	And extract parameter '$.grants.urn:ietf:params:oauth:grant-type:pre-authorized_code.pre-authorized_code' from JSON body into 'preAuthorizedCode'

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                     | Value                                                                                                                        |
	| grant_type              | urn:ietf:params:oauth:grant-type:pre-authorized_code                                                                         |
	| client_id               | fiftyNineClient                                                                                                              |
	| pre-authorized_code     | $preAuthorizedCode$                                                                                                          |
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential", "UniversityDegreeCredential"] } |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	Then HTTP status code equals to '200'
	And JWT has authorization_details type 'openid_credential'
	And JSON exists 'c_nonce'	
	And JSON exists 'c_nonce_expires_in'	