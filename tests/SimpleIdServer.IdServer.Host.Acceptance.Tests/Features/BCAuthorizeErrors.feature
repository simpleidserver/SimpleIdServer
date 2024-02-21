Feature: BCAuthorizeErrors
	Check errors returned by the /mtls/bc-authorize endpoint

Scenario: at least one token hint must be passed
	Given authenticate a user

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='only one hint can be passed in the request'

Scenario: request parameter must contains audience (request)
	Given authenticate a user
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key | Value          |
	| iss | fortyTwoClient |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the request doesn't contain audience'

Scenario: request parameter must contains valid audience (request)
	Given authenticate a user
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key | Value   |
	| aud | invalid |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the request doesn't contain correct audience'

Scenario: request parameter must contains issuer (request)
	Given authenticate a user
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key | Value                  |
	| aud | https://localhost:8080 |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the request doesn't contain issuer'

Scenario: request parameter must contains valid issuer (request)
	Given authenticate a user
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key | Value                  |
	| aud | https://localhost:8080 |
	| iss | invalid                |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the request doesn't contain correct issuer'

Scenario: request parameter must not be expired (request)
	Given authenticate a user
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key | Value                  |
	| aud | https://localhost:8080 |
	| iss | fortyTwoClient         |
	| exp | 1587492240             |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'

Scenario: lifetime of the request must not exceed 300 seconds (request)
	Given authenticate a user
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key | Value                  |
	| aud | https://localhost:8080 |
	| iss | fortyTwoClient         |
	| nbf | 1587492240             |
	| exp | 7267687440             |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the maximum lifetime of the request is '300' seconds'

Scenario: request parameter must contains jti (request)
	Given authenticate a user
	And build expiration time and add '30' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key | Value                  |
	| aud | https://localhost:8080 |
	| iss | fortyTwoClient         |
	| exp | $exp$                  |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the request doesn't contain jti'

Scenario: at least one token hint must be passed (request)
	Given authenticate a user
	And build expiration time and add '10' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key | Value                  |
	| aud | https://localhost:8080 |
	| iss | fortyTwoClient         |
	| exp | $exp$                  |
	| jti | jti                    |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='only one hint can be passed in the request'

Scenario: user_code is required when backchannel_user_code_parameter is true (request)
	Given authenticate a user
	And build expiration time and add '10' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key           | Value                  |
	| aud           | https://localhost:8080 |
	| iss           | fortyTwoClient         |
	| exp           | $exp$                  |
	| jti           | jti                    |
	| id_token_hint | idtokenhint            |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the parameter user_code is missing'

Scenario: scope or authorization_details parameter is required (request)
	Given authenticate a user
	And build expiration time and add '10' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key           | Value                  |
	| aud           | https://localhost:8080 |
	| iss           | fortyTwoClient         |
	| exp           | $exp$                  |
	| jti           | jti                    |
	| id_token_hint | idtokenhint            |
	| user_code     | code                   |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='missing parameters scope,authorization_details'

Scenario: authorization_details type is required (request)
	Given authenticate a user
	And build expiration time and add '10' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key                   | Value                      |
	| aud                   | https://localhost:8080     |
	| iss                   | fortyTwoClient             |
	| exp                   | $exp$                      |
	| jti                   | jti                        |
	| id_token_hint         | idtokenhint                |
	| user_code             | code                       |
	| authorization_details | { "locations": [] }        |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt  |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_authorization_details'
	And JSON 'error_description'='the authorization_details type is required'

Scenario: authorization_details type must be valid (request)
	Given authenticate a user
	And build expiration time and add '10' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key                   | Value                      |
	| aud                   | https://localhost:8080     |
	| iss                   | fortyTwoClient             |
	| exp                   | $exp$                      |
	| jti                   | jti                        |
	| id_token_hint         | idtokenhint                |
	| user_code             | code                       |
	| authorization_details | { "type": "invalid" }      |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt  |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_authorization_details'
	And JSON 'error_description'='authorization details types invalid are not supported'

Scenario: scope must be valid (request)
	Given authenticate a user
	And build expiration time and add '10' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key           | Value                  |
	| aud           | https://localhost:8080 |
	| iss           | fortyTwoClient         |
	| exp           | $exp$                  |
	| jti           | jti                    |
	| id_token_hint | idtokenhint            |
	| scope         | invalid                |
	| user_code     | code                   |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |

	And extract JSON from body

	Then JSON 'error'='invalid_scope'
	And JSON 'error_description'='unauthorized to scopes : invalid'

Scenario: client_notification_token parameter is required (request)
	Given authenticate a user
	And build expiration time and add '10' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key           | Value                  |
	| aud           | https://localhost:8080 |
	| iss           | fortyTwoClient         |
	| exp           | $exp$                  |
	| jti           | jti                    |
	| id_token_hint | idtokenhint            |
	| scope         | secondScope            |
	| user_code     | code                   |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value          |
	| X-Testing-ClientCert | sidClient.crt |
	| client_id            | fortyTwoClient |
	| request              | $request$      |	

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='missing parameter client_notification_token'

Scenario: client_notification_token size must be greater than 128 bits (request)
	Given authenticate a user
	And build expiration time and add '10' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key                       | Value                  |
	| aud                       | https://localhost:8080 |
	| iss                       | fortyTwoClient         |
	| exp                       | $exp$                  |
	| jti                       | jti                    |
	| id_token_hint             | idtokenhint            |
	| scope                     | secondScope            |
	| client_notification_token | 1                      |
	| user_code                 | code                   |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value          |
	| X-Testing-ClientCert      | sidClient.crt |
	| client_id                 | fortyTwoClient |
	| request                   | $request$      |	

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='client_notification_token must contains at least 128 bytes'

Scenario: id_token_hint must be valid (request)
	Given authenticate a user
	And build expiration time and add '10' seconds
	And build JWS request object for client 'fortyTwoClient' and sign with the key 'keyId'
	| Key                       | Value                                |
	| aud                       | https://localhost:8080               |
	| iss                       | fortyTwoClient                       |
	| exp                       | $exp$                                |
	| jti                       | jti                                  |
	| id_token_hint             | idtokenhint                          |
	| scope                     | secondScope                          |
	| client_notification_token | 04bcf708-dfba-4719-a3d3-b213322e2c38 |
	| user_code                 | code                                 |

	When execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value          |
	| X-Testing-ClientCert      | sidClient.crt |
	| client_id                 | fortyTwoClient |
	| request                   | $request$      |	

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='JSON Web Token cannot be read'