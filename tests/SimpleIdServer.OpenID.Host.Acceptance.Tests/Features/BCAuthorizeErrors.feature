Feature: BCAuthorizeErrors
	Check errors returned by /mtls/bc-authorize endpoint

Scenario: Error is returned when id_token_hint is invalid (get auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                                            | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
	| token_endpoint_auth_method                     | tls_client_auth                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
	| response_types                                 | [token]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
	| grant_types                                    | [client_credentials]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
	| scope                                          | scope1                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
	| redirect_uris                                  | [http://localhost:8080]                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
	| tls_client_auth_san_dns                        | firstMtlsClient                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
	| backchannel_authentication_request_signing_alg | PS256                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
	| jwks                                           | {"keys":[{ "kty": "RSA",  "use": "sig", "alg": "PS256",  "kid": "1", "key_ops": [ "sign", "verify"  ], "n": "ykDUVZX8lDX-O97R6CCqogzTuaLPR71v_0ul2FurEFQaMJYzGZg-tjSp9smrQUNuaNvxvb4cC0iJwHno-2cg2bFO5Ks-8NyP7fM0QQSPJUyq1-qa-wsOJ3NcGrphSHGN3fJEeJRIV1R1Yyl2XZBYCdbR1pFqBQ4yw0kiaOjAGr_bQloNjeo3Z-ak8oYfaVJvlCyQZpKoBlsBud8aFmr2KIuf_q9vJjiDObnoCaWf4ZYtCbeSP-qwshtTuC6YuzQL8IsiqTeUAm0VjV0yZkRStl-mQnHXI2o6du-3PYFqWXOwMn9USilJ1lYjqqL3jg-mXtQzckOpiXZfy2D8HTwFgQ", "e": "AQAB", "d": "mBnlFXA0gSmRxmitp0pZyICpAWRFSghXH8E-OdXGcgMtpvht-YObNd-pKmVDm1Mgx08RH8bGxF2K6utDoT1PYSeM0z9NmEnnG-XYmETbeguMN9DBOKZ5wIVq8NbVrmtna1B02dF6DeMAXNCjqX2SF-Qr7pdxCdhBqMdpT0gqoHHtjARGQT2JjjfQSWuQiU6EOZOqJFPYbER1b_mwbacHKwb3D8bX4JPcTvCMP5SNqUDIkjerWCq9j1DfnvRpnlcNrTnhlqG-S8mXr1Mt0bn40Kc9Y0-7d7Q3hvj3ObNW4jdEyf7Efb0d0ZjyFgZuAfPa4tkwIym9cBaz71Asz1-9mQ", "p": "1imC2zKZXeXONfKNoBJmq-JWyGziMlLvKWGnFdqIqqH9WNkxM8bAYPe2G8hd5SYOGg2CEWm4BN7cj4tKlegzOHmaVybo-a5odHB5UCo3ZsWtM0tnU6bkdYboLR4nu4j_Fhsu2cHe46IaG2BGMnZw4qgKyjoPd81QbOCBfGZR_ac", "q": "8cO9fplrHh01BQrxCe-yZwUOg2w14j-DkC9VtvgFDNeQOv-mGqxldhvCsBzAJ7Jog-lk-vPbMT4OT2lHJ4hhWBSt9mZbAHAL4hZR-95JTmh5crEpyfQYEoWhI_29uDfRl9nb9d7ef28B_o0yxQcnxaSAvUpk_t_ffrlHTHCiWJc", "dp": "XM_sp_ZLxQe80GBnxEF1QXR7y6x0cv7CKxrpAG2O4PQHCaRG0HuID_4KbAvvtUlCv0OvZMB-QY4b5BGnp0PUPttkafSw7tQI7L15taY2EFIG973r77LaO5zVrgftjDaY5gmtyi1c3SX5TnfwtBnkXjYZRv7WiULvYeAB_dmSGu0", "dq": "RKubjHUZVvFm2OrVskPSQa7PA4dd-sidnvvC9fiWvqIQBqIM1TC4lQankwxnjB8Bzs4hb6KKVP9SYz39Bv6W38Tn8L_8AzDQP0SlvsvRuKK2NFycCQ_7Mm-gaK-vDr9UGjS4ZKsMOdgXEe2bSRmSM3JZWurhqv995OnNlam8gzs", "qi": "lR7p9_WQgDdVW5Sq0r4CzqENOScLddmCDb2o9RXUGh0TU-mJ92wuEE92llrm4JlsaXNFryW7yhKunmPy0onMNjSnYIvwVcmYVV3MrnJbSZmMTRoAmTnTtAoSSf0eGgCepnphI3lZnfIZAONUXBmdT0zcT3WO6nveLDsCP_MTWLg" }]} |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |
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
	| id_token_hint             | idtokenhint                          |
	| scope                     | scope1                               |
	| client_notification_token | 04bcf708-dfba-4719-a3d3-b213322e2c38 |

	And execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| request              | $requestParameter$ |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='id_token_hint is invalid'

Scenario: Error is returned when hint is missing (confirm auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key                  | Value          |
	| X-Testing-ClientCert | mtlsClient.crt |
	| client_id            | $client_id$    |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='only one hint can be passed in the request'

Scenario: Error is returned when id_token_hint is invalid (confirm auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key                       | Value                                |
	| client_id                 | $client_id$                          |
	| id_token_hint             | idtokenhint                          |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='id_token_hint is invalid'	

Scenario: Error is returned when auth_req_id is missing (confirm auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key                  | Value          |
	| client_id            | $client_id$    |
	| login_hint           | administrator  |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter auth_req_id'

Scenario: Error is returned when authtorization request doesn't exist (confirm auth_req_id)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | scope1                  |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | firstMtlsClient         |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | scope1             |
	| grant_type           | client_credentials |

	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key                  | Value          |
	| client_id            | $client_id$    |
	| login_hint           | administrator  |
	| auth_req_id          | authreqid      |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='auth_req_id doesn't exist'