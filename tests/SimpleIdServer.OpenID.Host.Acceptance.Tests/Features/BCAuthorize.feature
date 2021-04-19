Feature: BCAuthorize
	Check /mtls/bc-authorize endpoint

Scenario: Use push notification mode
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |
	
	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                                      | Value                                      |
	| token_endpoint_auth_method               | tls_client_auth                            |
	| response_types                           | [token]                                    |
	| grant_types                              | [client_credentials]                       |
	| scope                                    | openid profile                             |
	| redirect_uris                            | [http://localhost:8080]                    |
	| tls_client_auth_san_dns                  | firstMtlsClient                            |
	| backchannel_token_delivery_mode          | push                                       |
	| backchannel_client_notification_endpoint | https://localhost:8080/pushNotificationEdp |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST JSON request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value                                |
	| X-Testing-ClientCert      | mtlsClient.crt                       |
	| client_id                 | $client_id$                          |
	| login_hint                | administrator                        |
	| scope                     | openid profile                       |
	| client_notification_token | 7dc3061e-bad9-4817-bd33-8db789bfb516 |

	And extract JSON from body
	And extract parameter 'auth_req_id' from JSON body
	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key         | Value         |
	| login_hint  | administrator |
	| auth_req_id | $auth_req_id$ |

	And poll until 'callbackResponse' is received
	And extract JSON from 'callbackResponse'

	Then JSON contains 'auth_req_id'
	Then JSON contains 'id_token'
	Then JSON contains 'access_token'
	Then JSON contains 'refresh_token'

Scenario: Use ping notification mode
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |
	
	And execute HTTP POST JSON request 'https://localhost:8080/register'
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

	And execute HTTP POST JSON request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value                                |
	| X-Testing-ClientCert      | mtlsClient.crt                       |
	| client_id                 | $client_id$                          |
	| login_hint                | administrator                        |
	| scope                     | openid profile                       |
	| client_notification_token | 7dc3061e-bad9-4817-bd33-8db789bfb516 |

	And extract JSON from body
	And extract parameter 'auth_req_id' from JSON body
	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key         | Value         |
	| login_hint  | administrator |
	| auth_req_id | $auth_req_id$ |

	And poll until 'callbackResponse' is received
	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key                  | Value          |
	| client_id            | $client_id$    |
	| login_hint           | administrator  |
	| auth_req_id          | $auth_req_id$  |

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                             |
	| client_id            | $client_id$                       |
	| scope                | openid profile                    |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| X-Testing-ClientCert | mtlsClient.crt                    |
	| auth_req_id          | $auth_req_id$                     |
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	
Scenario: Use poll notification mode
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |
	
	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                                      | Value                                      |
	| token_endpoint_auth_method               | tls_client_auth                            |
	| response_types                           | [token]                                    |
	| grant_types                              | [urn:openid:params:grant-type:ciba]        |
	| scope                                    | openid profile                             |
	| redirect_uris                            | [http://localhost:8080]                    |
	| tls_client_auth_san_dns                  | firstMtlsClient                            |
	| backchannel_token_delivery_mode          | poll                                       |
	| backchannel_client_notification_endpoint | https://localhost:8080/pushNotificationEdp |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST JSON request 'https://localhost:8080/mtls/bc-authorize'
	| Key                       | Value                                |
	| X-Testing-ClientCert      | mtlsClient.crt                       |
	| client_id                 | $client_id$                          |
	| login_hint                | administrator                        |
	| scope                     | openid profile                       |
	| client_notification_token | 7dc3061e-bad9-4817-bd33-8db789bfb516 |

	And extract JSON from body
	And extract parameter 'auth_req_id' from JSON body
	And execute HTTP POST JSON request 'https://localhost:8080/bc-authorize/confirm'
	| Key         | Value         |
	| login_hint  | administrator |
	| auth_req_id | $auth_req_id$ |
	
	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                             |
	| client_id            | $client_id$                       |
	| scope                | openid profile                    |
	| grant_type           | urn:openid:params:grant-type:ciba |
	| X-Testing-ClientCert | mtlsClient.crt                    |
	| auth_req_id          | $auth_req_id$                     |
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON contains 'id_token'
	Then JSON contains 'access_token'
	Then JSON contains 'refresh_token'