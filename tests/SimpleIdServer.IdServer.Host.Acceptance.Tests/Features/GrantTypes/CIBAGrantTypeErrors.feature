Feature: CIBAGrantTypeErrors
	Check errors returned when using 'urn:openid:params:grant-type:ciba' grant-type

Scenario: Only PING or POLL modes are supported to get a token
	When execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyThreeClient                  |
	| X-Testing-ClientCert | sidClient.crt                    |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='only ping or poll mode can be used to get tokens'

Scenario: parameter auth_req_id is required
	When execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyTwoClient                    |
	| X-Testing-ClientCert | sidClient.crt                    |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter auth_req_id'

Scenario: authorization request must exists
	When execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyTwoClient                    |
	| X-Testing-ClientCert | sidClient.crt                    |
	| auth_req_id          | id                                |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='authorization request doesn't exist'

Scenario: client must be the same
	Given authenticate a user

	When execute HTTP POST request 'https://localhost:8080/bc-authorize'
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
	| auth_req_id   | $auth_req_id$          |

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fiftyClient                       |
	| client_secret        | password                          |
	| auth_req_id          | $auth_req_id$                     |

	And extract JSON from body
	
	Then JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='the client is not authorized to use the auth_req_id'

Scenario: authorization request must be confirmed
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

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyNineClient                   |
	| client_secret        | password                          |
	| auth_req_id          | $auth_req_id$                     |

	And extract JSON from body
	
	Then JSON '$.error'='authorization_pending'
	And JSON '$.error_description'='the authorization request has not been confirmed'

Scenario: authorization request cannot be rejected
	Given authenticate a user
	
	When execute HTTP POST request 'https://localhost:8080/bc-authorize'
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
	| auth_req_id   | $auth_req_id$          |
	| action        | 1                      |

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyNineClient                   |
	| client_secret        | password                          |
	| auth_req_id          | $auth_req_id$                     |

	And extract JSON from body
	
	Then JSON '$.error'='access_denied'
	And JSON '$.error_description'='the authorization request has been rejected'