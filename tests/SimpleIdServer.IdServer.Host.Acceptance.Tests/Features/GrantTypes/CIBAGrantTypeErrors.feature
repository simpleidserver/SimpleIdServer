Feature: CIBAGrantTypeErrors
	Check errors returned when using 'urn:openid:params:grant-type:ciba' grant-type

Scenario: Only PING or POLL modes are supported to get a token
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyThreeClient                  |
	| X-Testing-ClientCert | mtlsClient.crt                    |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='only ping or poll mode can be used to get tokens'

Scenario: parameter auth_req_id is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyTwoClient                    |
	| X-Testing-ClientCert | mtlsClient.crt                    |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter auth_req_id'

Scenario: authorization request must exists
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                             |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| client_id            | fortyTwoClient                    |
	| X-Testing-ClientCert | mtlsClient.crt                    |
	| auth_req_id          | id                                |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='authorization request doesn't exist'