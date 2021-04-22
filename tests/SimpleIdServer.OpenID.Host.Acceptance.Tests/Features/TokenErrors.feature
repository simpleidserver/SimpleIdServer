Feature: TokenErrors
	Check errors returned by token endpoint

Scenario: Authorization code cannot be used two times
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| ENC  | 2   | RSA1_5  |

	When execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA1_5                        |
	| id_token_encrypted_response_enc | A256CBC-HS512                 |
	| jwks                            | $jwks_json$                   |
	| token_endpoint_auth_method      | client_secret_post            |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value               |
	| response_type | id_token token code |
	| client_id     | $client_id$         |
	| state         | state               |
	| response_mode | query               |
	| scope         | openid email role   |
	| redirect_uri  | https://web.com     |
	| ui_locales    | en fr               |
	| nonce         | nonce               |

	And extract 'id_token' from callback
	And extract 'code' from callback

	And execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | $client_id$        |
	| client_secret | $client_secret$    |
	| grant_type    | authorization_code |
	| code          | $code$             |
	| redirect_uri  | https://web.com    |

	And execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | $client_id$        |
	| client_secret | $client_secret$    |
	| grant_type    | authorization_code |
	| code          | $code$             |
	| redirect_uri  | https://web.com    |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_grant'
	Then JSON 'error_description'='authorization code has already been used, all tokens previously issued have been revoked'

Scenario: Only ping or push mode can be used to get tokens
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                                      | Value                                      |
	| token_endpoint_auth_method               | tls_client_auth                            |
	| response_types                           | [token]                                    |
	| grant_types                              | [urn:openid:params:grant-type:ciba]        |
	| scope                                    | openid profile                             |
	| redirect_uris                            | [http://localhost:8080]                    |
	| tls_client_auth_san_dns                  | firstMtlsClient                            |
	| backchannel_token_delivery_mode          | push                                       |
	| backchannel_client_notification_endpoint | https://localhost:8080/pushNotificationEdp |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                             |
	| client_id            | $client_id$                       |
	| scope                | openid profile                    |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| X-Testing-ClientCert | mtlsClient.crt                    |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='only ping or push mode can be used to get tokens'

Scenario: auth_req_id parameter is mandatory 
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                                      | Value                                      |
	| token_endpoint_auth_method               | tls_client_auth                            |
	| response_types                           | [token]                                    |
	| grant_types                              | [urn:openid:params:grant-type:ciba]        |
	| scope                                    | openid profile                             |
	| redirect_uris                            | [http://localhost:8080]                    |
	| tls_client_auth_san_dns                  | firstMtlsClient                            |
	| backchannel_token_delivery_mode          | ping                                       |
	| backchannel_client_notification_endpoint | https://localhost:8080/pushNotificationEdp |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                             |
	| client_id            | $client_id$                       |
	| scope                | openid profile                    |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| X-Testing-ClientCert | mtlsClient.crt                    |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter auth_req_id'

Scenario: Error is returned when auth_req_id is invalid
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                                      | Value                                      |
	| token_endpoint_auth_method               | tls_client_auth                            |
	| response_types                           | [token]                                    |
	| grant_types                              | [urn:openid:params:grant-type:ciba]        |
	| scope                                    | openid profile                             |
	| redirect_uris                            | [http://localhost:8080]                    |
	| tls_client_auth_san_dns                  | firstMtlsClient                            |
	| backchannel_token_delivery_mode          | ping                                       |
	| backchannel_client_notification_endpoint | https://localhost:8080/pushNotificationEdp |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                             |
	| client_id            | $client_id$                       |
	| scope                | openid profile                    |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| X-Testing-ClientCert | mtlsClient.crt                    |
	| auth_req_id          | authreqid                         |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_grant'
	Then JSON 'error_description'='auth_req_id doesn't exist'

Scenario: auth_req_id must be confirmed
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                                            | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
	| token_endpoint_auth_method                     | tls_client_auth                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
	| response_types                                 | [token]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
	| grant_types                                    | [urn:openid:params:grant-type:ciba]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
	| scope                                          | openid profile                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
	| redirect_uris                                  | [http://localhost:8080]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
	| tls_client_auth_san_dns                        | firstMtlsClient                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
	| backchannel_token_delivery_mode                | ping                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
	| backchannel_client_notification_endpoint       | https://localhost:8080/pushNotificationEdp                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
	| backchannel_authentication_request_signing_alg | PS256                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
	| jwks                                           | {"keys":[{ "kty": "RSA",  "use": "sig", "alg": "PS256",  "kid": "1", "key_ops": [ "sign", "verify"  ], "n": "ykDUVZX8lDX-O97R6CCqogzTuaLPR71v_0ul2FurEFQaMJYzGZg-tjSp9smrQUNuaNvxvb4cC0iJwHno-2cg2bFO5Ks-8NyP7fM0QQSPJUyq1-qa-wsOJ3NcGrphSHGN3fJEeJRIV1R1Yyl2XZBYCdbR1pFqBQ4yw0kiaOjAGr_bQloNjeo3Z-ak8oYfaVJvlCyQZpKoBlsBud8aFmr2KIuf_q9vJjiDObnoCaWf4ZYtCbeSP-qwshtTuC6YuzQL8IsiqTeUAm0VjV0yZkRStl-mQnHXI2o6du-3PYFqWXOwMn9USilJ1lYjqqL3jg-mXtQzckOpiXZfy2D8HTwFgQ", "e": "AQAB", "d": "mBnlFXA0gSmRxmitp0pZyICpAWRFSghXH8E-OdXGcgMtpvht-YObNd-pKmVDm1Mgx08RH8bGxF2K6utDoT1PYSeM0z9NmEnnG-XYmETbeguMN9DBOKZ5wIVq8NbVrmtna1B02dF6DeMAXNCjqX2SF-Qr7pdxCdhBqMdpT0gqoHHtjARGQT2JjjfQSWuQiU6EOZOqJFPYbER1b_mwbacHKwb3D8bX4JPcTvCMP5SNqUDIkjerWCq9j1DfnvRpnlcNrTnhlqG-S8mXr1Mt0bn40Kc9Y0-7d7Q3hvj3ObNW4jdEyf7Efb0d0ZjyFgZuAfPa4tkwIym9cBaz71Asz1-9mQ", "p": "1imC2zKZXeXONfKNoBJmq-JWyGziMlLvKWGnFdqIqqH9WNkxM8bAYPe2G8hd5SYOGg2CEWm4BN7cj4tKlegzOHmaVybo-a5odHB5UCo3ZsWtM0tnU6bkdYboLR4nu4j_Fhsu2cHe46IaG2BGMnZw4qgKyjoPd81QbOCBfGZR_ac", "q": "8cO9fplrHh01BQrxCe-yZwUOg2w14j-DkC9VtvgFDNeQOv-mGqxldhvCsBzAJ7Jog-lk-vPbMT4OT2lHJ4hhWBSt9mZbAHAL4hZR-95JTmh5crEpyfQYEoWhI_29uDfRl9nb9d7ef28B_o0yxQcnxaSAvUpk_t_ffrlHTHCiWJc", "dp": "XM_sp_ZLxQe80GBnxEF1QXR7y6x0cv7CKxrpAG2O4PQHCaRG0HuID_4KbAvvtUlCv0OvZMB-QY4b5BGnp0PUPttkafSw7tQI7L15taY2EFIG973r77LaO5zVrgftjDaY5gmtyi1c3SX5TnfwtBnkXjYZRv7WiULvYeAB_dmSGu0", "dq": "RKubjHUZVvFm2OrVskPSQa7PA4dd-sidnvvC9fiWvqIQBqIM1TC4lQankwxnjB8Bzs4hb6KKVP9SYz39Bv6W38Tn8L_8AzDQP0SlvsvRuKK2NFycCQ_7Mm-gaK-vDr9UGjS4ZKsMOdgXEe2bSRmSM3JZWurhqv995OnNlam8gzs", "qi": "lR7p9_WQgDdVW5Sq0r4CzqENOScLddmCDb2o9RXUGh0TU-mJ92wuEE92llrm4JlsaXNFryW7yhKunmPy0onMNjSnYIvwVcmYVV3MrnJbSZmMTRoAmTnTtAoSSf0eGgCepnphI3lZnfIZAONUXBmdT0zcT3WO6nveLDsCP_MTWLg" }]} |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body
	And add '2' seconds and store the result into 'exp'
	And add '-2' seconds and store the result into 'nbf'

	And client '$client_id$' build JWS token using the algorithm 'PS256'
	| Key                       | Value                                |
	| aud                       | [https://localhost:8080]             |
	| iss                       | $client_id$                          |
	| exp                       | $exp$                                |
	| iat                       | 1587492240                           |
	| nbf                       | $nbf$                                |
	| jti                       | jti                                  |
	| login_hint                | administrator                        |
	| scope                     | openid profile                       |
	| client_notification_token | 7dc3061e-bad9-4817-bd33-8db789bfb516 |

	And execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| request              | $requestParameter$ |

	And extract JSON from body
	And extract parameter 'auth_req_id' from JSON body

	And poll HTTP POST request 'https://localhost:8080/mtls/token', until 'error'='authorization_pending'
	| Key                  | Value                             |
	| client_id            | $client_id$                       |
	| scope                | openid profile                    |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| X-Testing-ClientCert | mtlsClient.crt                    |
	| auth_req_id          | $auth_req_id$                     |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='authorization_pending'
	Then JSON 'error_description'='the authentication request '$auth_req_id$' has not been confirmed'