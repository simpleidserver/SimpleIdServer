Feature: BCAuthorize
	Check result returned by the /mtls/bc-authorize endpoint

Scenario: Authentication response is returned with interval because PING is used
	Given authenticate a user
	And build expiration time and add '5000' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key                       | Value                                |
	| aud                       | https://localhost:8080               |
	| iss                       | fortyTwoClient                       |
	| exp                       | $exp$                                |
	| jti                       | jti                                  |
	| login_hint                | user                                 |
	| scope                     | secondScope                          |
	| client_notification_token | 04bcf708-dfba-4719-a3d3-b213322e2c38 |
	| user_code                 | password                             |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value          |
	| X-Testing-ClientCert      | sidClient.crt |
	| client_id                 | fortyTwoClient |
	| request                   | $request$      |	

	And extract JSON from body

	Then JSON 'expires_in'='120'
	And JSON 'interval'='5'
	And JSON exists 'auth_req_id'

Scenario: Authentication response is returned without interval because PUSH is used
	Given authenticate a user
	And build expiration time and add '5000' seconds
	And build JWS request object for client 'fortyThreeClient' and sign with the key 'keyId'
	| Key                       | Value                                |
	| aud                       | https://localhost:8080               |
	| iss                       | fortyThreeClient                     |
	| exp                       | $exp$                                |
	| jti                       | jti                                  |
	| login_hint                | user                                 |
	| scope                     | secondScope                          |
	| client_notification_token | 04bcf708-dfba-4719-a3d3-b213322e2c38 |
	| user_code                 | password                             |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value            |
	| X-Testing-ClientCert      | sidClient.crt   |
	| client_id                 | fortyThreeClient |
	| request                   | $request$        |	

	And extract JSON from body

	Then JSON 'expires_in'='120'
	And JSON exists 'auth_req_id'
	And JSON doesn't exist '$.interval'