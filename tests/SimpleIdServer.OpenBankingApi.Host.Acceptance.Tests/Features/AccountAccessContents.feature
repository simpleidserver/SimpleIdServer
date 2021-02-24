Feature: AccountAccessContents
	Check /account-requests endpoint

Scenario: Create Account Access Content
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                          | Value                   |
	| token_endpoint_auth_method   | tls_client_auth         |
	| response_types               | [token]                 |
	| grant_types                  | [client_credentials]    |
	| scope                        | accounts                |
	| redirect_uris                | [http://localhost:8080] |
	| tls_client_auth_san_dns      | mtlsClient              |
	| id_token_signed_response_alg | PS256                   |
	| token_signed_response_alg    | PS256                   |

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

	And execute HTTP POST JSON request 'https://localhost:8080/v3.1/account-access-consents'
    | Key                   | Value                                                             |
    | Authorization         | Bearer $accessToken$                                              |
    | x-fapi-interaction-id | guid                                                              |
    | X-Testing-ClientCert  | mtlsClient.crt                                                    |
    | data                  | { "permissions" : [ "ReadAccountsBasic", "ReadAccountsDetail" ] } |

	And extract JSON from body
	And extract HTTP headers
	
	Then HTTP status code equals to '200'
	Then JSON exists 'Data.ConsentId'
	Then JSON exists 'Links.Self'
	Then JSON 'Data.Permissions[0]'='ReadAccountsBasic'
	Then JSON 'Data.Permissions[1]'='ReadAccountsDetail'
	Then JSON 'Data.Status'='AwaitingAuthorisation'
	Then JSON 'Meta.TotalPages'='1'
	Then HTTP Header 'x-fapi-interaction-id'='guid'