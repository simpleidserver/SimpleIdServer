Feature: AccountAccessContents
	Check /account-access-consents endpoint

Scenario: Create Account Access Content
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                   |
	| token_endpoint_auth_method | tls_client_auth         |
	| response_types             | [token]                 |
	| grant_types                | [client_credentials]    |
	| scope                      | accounts                |
	| redirect_uris              | [http://localhost:8080] |
	| tls_client_auth_san_dns    | mtlsClient              |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| client_id            | $client_id$        |
	| scope                | accounts           |
	| grant_type           | client_credentials |
	| X-Testing-ClientCert | mtlsClient.crt     |

	And extract JSON from body
	And extract parameter 'access_token' from JSON body into 'accessToken'

	And execute HTTP POST JSON request 'https://localhost:8080/account-access-consents'
    | Key                  | Value                                                             |
    | Authorization        | Bearer $accessToken$                                              |
    | X-Testing-ClientCert | mtlsClient.crt                                                    |
    | data                 | { "permissions" : [ "ReadAccountsBasic", "ReadAccountsDetail" ] } |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'data.permissions[0]'='ReadAccountsBasic'
	Then JSON 'data.permissions[1]'='ReadAccountsDetail'