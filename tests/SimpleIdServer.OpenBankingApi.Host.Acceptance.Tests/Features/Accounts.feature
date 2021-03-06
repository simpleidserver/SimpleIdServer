﻿Feature: Accounts
	Check /accounts endpoint

Scenario: Get accounts (Basic)
	When build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                          | Value                                            |
	| token_endpoint_auth_method   | tls_client_auth                                  |
	| response_types               | [token,code,id_token]                            |
	| grant_types                  | [client_credentials,authorization_code,implicit] |
	| scope                        | accounts                                         |
	| redirect_uris                | [https://localhost:8080/callback]                |
	| tls_client_auth_san_dns      | firstMtlsClient                                  |
	| id_token_signed_response_alg | PS256                                            |
	| token_signed_response_alg    | PS256                                            |
	| request_object_signing_alg   | RS256                                            |
	| jwks                         | $jwks_json$                                      |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | accounts           |
	| grant_type           | client_credentials |

	And extract JSON from body
	And extract parameter 'access_token' from JSON body into 'accessToken'

	And execute HTTP POST JSON request 'https://localhost:8080/v3.1/account-access-consents'
    | Key                   | Value                                       |
    | Authorization         | Bearer $accessToken$                        |
    | x-fapi-interaction-id | guid                                        |
    | X-Testing-ClientCert  | mtlsClient.crt                              |
    | data                  | { "permissions" : [ "ReadAccountsBasic" ] } |

	And extract JSON from body
	And extract parameter 'Data.ConsentId' from JSON body into 'consentId'

	And 'administrator' confirm consent '$consentId$' for accounts '22289', with scopes 'accounts'	
	
	And use '1' JWK from 'jwks' to build JWS and store into 'request'
	| Key           | Value                                                                               |
	| response_type | id_token code                                                                       |
	| client_id     | $client_id$                                                                         |
	| state         | MTkCNSYlem                                                                          |
	| response_mode | fragment                                                                            |
	| scope         | accounts                                                                            |
	| redirect_uri  | https://localhost:8080/callback                                                     |
	| nonce         | nonce                                                                               |
	| claims        | { id_token: { openbanking_intent_id : { value: "$consentId$", essential: true } } } |
	| exp           | $tomorrow$                                                                          |
	| aud           | https://localhost:8080                                                              |
		
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key                  | Value          |
	| X-Testing-ClientCert | mtlsClient.crt |
	| response_type        | id_token code  |
	| scope                | accounts       |
	| client_id            | $client_id$    |
	| request              | $request$      |

	And extract 'code' from callback

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                           |
	| X-Testing-ClientCert | mtlsClient.crt                  |
	| client_id            | $client_id$                     |
	| scope                | accounts                        |
	| grant_type           | authorization_code              |
	| code                 | $code$                          |
	| redirect_uri         | https://localhost:8080/callback |

	And extract JSON from body
	And extract parameter 'access_token' from JSON body into 'accessToken'

	And execute HTTP GET request 'https://localhost:8080/v3.1/accounts'
	| Key                  | Value                |
	| Authorization        | Bearer $accessToken$ |
	| X-Testing-ClientCert | mtlsClient.crt       |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'Data.Account[0].AccountId'='22289'
	Then JSON 'Data.Account[0].AccountSubType'='CurrentAccount'
	Then JSON doesn't exist 'Data.Account[0].Accounts[0].Identification'
	Then JSON doesn't exist 'Data.Account[0].Accounts[0].SecondaryIdentification'

Scenario: Get accounts (Detail)
	When build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                          | Value                                            |
	| token_endpoint_auth_method   | tls_client_auth                                  |
	| response_types               | [token,code,id_token]                            |
	| grant_types                  | [client_credentials,authorization_code,implicit] |
	| scope                        | accounts                                         |
	| redirect_uris                | [https://localhost:8080/callback]                |
	| tls_client_auth_san_dns      | firstMtlsClient                                  |
	| id_token_signed_response_alg | PS256                                            |
	| token_signed_response_alg    | PS256                                            |
	| request_object_signing_alg   | RS256                                            |
	| jwks                         | $jwks_json$                                      |

	And extract JSON from body	
	And extract parameter 'client_id' from JSON body

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| X-Testing-ClientCert | mtlsClient.crt     |
	| client_id            | $client_id$        |
	| scope                | accounts           |
	| grant_type           | client_credentials |

	And extract JSON from body
	And extract parameter 'access_token' from JSON body into 'accessToken'

	And execute HTTP POST JSON request 'https://localhost:8080/v3.1/account-access-consents'
    | Key                   | Value                                        |
    | Authorization         | Bearer $accessToken$                         |
    | x-fapi-interaction-id | guid                                         |
    | X-Testing-ClientCert  | mtlsClient.crt                               |
    | data                  | { "permissions" : [ "ReadAccountsDetail" ] } |

	And extract JSON from body
	And extract parameter 'Data.ConsentId' from JSON body into 'consentId'

	And 'administrator' confirm consent '$consentId$' for accounts '22289', with scopes 'accounts'	
	
	And use '1' JWK from 'jwks' to build JWS and store into 'request'
	| Key           | Value                                                                               |
	| response_type | id_token code                                                                       |
	| client_id     | $client_id$                                                                         |
	| state         | MTkCNSYlem                                                                          |
	| response_mode | fragment                                                                            |
	| scope         | accounts                                                                            |
	| redirect_uri  | https://localhost:8080/callback                                                     |
	| nonce         | nonce                                                                               |
	| claims        | { id_token: { openbanking_intent_id : { value: "$consentId$", essential: true } } } |
	| exp           | $tomorrow$                                                                          |
	| aud           | https://localhost:8080                                                              |
		
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key                  | Value          |
	| X-Testing-ClientCert | mtlsClient.crt |
	| response_type        | id_token code  |
	| scope                | accounts       |
	| client_id            | $client_id$    |
	| request              | $request$      |

	And extract 'code' from callback

	And execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value                           |
	| X-Testing-ClientCert | mtlsClient.crt                  |
	| client_id            | $client_id$                     |
	| scope                | accounts                        |
	| grant_type           | authorization_code              |
	| code                 | $code$                          |
	| redirect_uri         | https://localhost:8080/callback |

	And extract JSON from body
	And extract parameter 'access_token' from JSON body into 'accessToken'

	And execute HTTP GET request 'https://localhost:8080/v3.1/accounts'
	| Key                  | Value                |
	| Authorization        | Bearer $accessToken$ |
	| X-Testing-ClientCert | mtlsClient.crt       |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'Data.Account[0].AccountId'='22289'
	Then JSON 'Data.Account[0].AccountSubType'='CurrentAccount'
	Then JSON 'Data.Account[0].Accounts[0].Identification'='80200110203345'
	Then JSON 'Data.Account[0].Accounts[0].SecondaryIdentification'='00021'