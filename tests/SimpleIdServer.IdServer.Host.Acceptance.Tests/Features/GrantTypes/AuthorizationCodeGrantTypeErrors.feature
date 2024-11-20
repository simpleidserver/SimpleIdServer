﻿Feature: AuthorizationCodeGrantTypeErrors
	Check errors returned when using 'authorization_code' grant-type

Scenario: Send 'grant_type=authorization_code' with no code parameter
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value              |
	| grant_type | authorization_code |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter code'

Scenario: Send 'grant_type=authorization_code,code=code' with no redirect_uri
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

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value                 |
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter redirect_uri'
	
Scenario: Send 'grant_type=authorization_code,code=code,redirect_uri=http://localhost,client_id=firstClient,client_secret=password' with unauthorized grant_type
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

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| grant_type    | authorization_code |
	| code          | $code$  	         |
	| redirect_uri  | http://localhost   |
	| client_id     | firstClient        |
	| client_secret | password           |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='grant type authorization_code is not supported by the client'

Scenario:  Send 'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080,client_id=thirdClient,client_secret=password' with previous issued token
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
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|
	| redirect_uri  | http://localhost:8080	|
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='authorization code has already been used, all tokens previously issued have been revoked'

Scenario: Send 'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080,client_id=thirdClient,client_secret=password' with bad code
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| invalidCode			|	
	| redirect_uri  | http://localhost:8080	|

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='bad authorization code'
	
Scenario: Send 'grant_type=authorization_code,code=code,redirect_uri=http://localhost:8080,client_id=thirdClient,client_secret=password' with code not issued by the client
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
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|
	| redirect_uri  | http://localhost:9080	|

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='not the same redirect_uri'

Scenario: authorization code cannot be used twice
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
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|
	| redirect_uri  | http://localhost:8080	|
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|
	| redirect_uri  | http://localhost:8080	|	

	And extract JSON from body
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_grant'
	Then JSON 'error_description'='authorization code has already been used, all tokens previously issued have been revoked'	

Scenario: cannot have a mismatch between dpop_jkt and the DPoP proof
	Given authenticate a user

	When build DPoP proof
	| Key   | Value                          |
	| htm   | POST                           |
	| htu   | https://localhost:8080/token   |

	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | sixtyFiveClient       |
	| state         | state                 |
	| redirect_uri  | http://localhost:8080 |
	| response_mode | query                 |
	| scope         | secondScope			|	
	| dpop_jkt      | invalid               |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | sixtyFiveClient		|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|
	| redirect_uri  | http://localhost:8080	|
	| DPoP          | $DPOP$                |

	And extract header 'DPoP-Nonce' to 'nonce'

	And build DPoP proof
	| Key   | Value                          |
	| htm   | POST                           |
	| htu   | https://localhost:8080/token   |
	| nonce | $nonce$                        |
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | sixtyFiveClient		|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|
	| redirect_uri  | http://localhost:8080	|
	| DPoP          | $DPOP$                |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_dpop_proof'
	Then JSON 'error_description'='there is a mismatch between the dpop_jkt and the DPoP proof'