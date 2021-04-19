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
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='auth_req_id doesn't exist'

Scenario: auth_req_id must be confirmed
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

	And execute HTTP POST request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value                                |
	| X-Testing-ClientCert      | mtlsClient.crt                       |
	| client_id                 | $client_id$                          |
	| login_hint                | administrator                        |
	| scope                     | openid profile                       |
	| client_notification_token | 7dc3061e-bad9-4817-bd33-8db789bfb516 |

	And extract JSON from body
	And extract parameter 'auth_req_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
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