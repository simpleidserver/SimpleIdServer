Feature: PreAuthorizedCodeErrors
	Check errors returned when using 'urn:ietf:params:oauth:grant-type:pre-authorized_code' grant-type

Scenario: client_id is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value                                                |
	| grant_type | urn:ietf:params:oauth:grant-type:pre-authorized_code	|

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='missing client_id'

Scenario: client must exists
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value                                                |
	| grant_type | urn:ietf:params:oauth:grant-type:pre-authorized_code	|
	| client_id  | invalid                                              |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='unknown client invalid'

Scenario: client must supports the grant-type
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value                                                |
	| grant_type | urn:ietf:params:oauth:grant-type:pre-authorized_code	|
	| client_id  | fiftySevenClient                                     |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='grant type urn:ietf:params:oauth:grant-type:pre-authorized_code is not supported'

Scenario: pre-authorized code is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value                                                |
	| grant_type | urn:ietf:params:oauth:grant-type:pre-authorized_code	|
	| client_id  | fiftyNineClient                                      |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter pre-authorized_code'

Scenario: when user_pin is true then pin is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                 | Value                                                |
	| grant_type          | urn:ietf:params:oauth:grant-type:pre-authorized_code |
	| client_id           | sixtyClient                                          |
	| pre-authorized_code | code                                                 |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter user_pin'

Scenario: pre-authorized must exists
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                                                |
	| grant_type           | urn:ietf:params:oauth:grant-type:pre-authorized_code |
	| client_id            | fiftyNineClient                                      |
	| pre-authorized_code  | code                                                 |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='either the pre-authorized code has expired or is invalid'

Scenario: format is required in the authorization_details
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
	| Key                     | Value                                                |
	| grant_type              | urn:ietf:params:oauth:grant-type:pre-authorized_code |
	| client_id               | fiftyNineClient                                      |
	| pre-authorized_code     | $preAuthorizedCode$                                  |
	| authorization_details   |  { "type" : "openid_credential" }                    |
	
	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the authorization_details must contain a format'

Scenario: format must be supported in the authorization_details
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
	| Key                     | Value                                                      |
	| grant_type              | urn:ietf:params:oauth:grant-type:pre-authorized_code       |
	| client_id               | fiftyNineClient                                            |
	| pre-authorized_code     | $preAuthorizedCode$                                        |
	| authorization_details   |  { "type" : "openid_credential", "format" : "invalid" }    |
	
	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='credential formats invalid are not supported'