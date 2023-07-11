Feature: Register
	Check client can be registered

Scenario: Register a complete client
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                        |
	| Authorization                   | Bearer $access_token$        |	
	| redirect_uris                   | ["https://web.com"]          |
	| response_types                  | ["token"]                    |
	| grant_types                     | ["implicit"]                 |
	| client_name                     | name                         |
	| client_name#fr                  | nom                          |
	| client_name#en                  | name                         |
	| application_type                | web                          |
	| token_endpoint_auth_method      | client_secret_jwt            |
	| sector_identifier_uri           | https://localhost/sector     |
	| subject_type                    | public                       |
	| id_token_signed_response_alg    | RS256                        |
	| id_token_encrypted_response_alg | RSA-OAEP                     |
	| id_token_encrypted_response_enc | A256CBC-HS512                |
	| userinfo_signed_response_alg    | RS256                        |
	| userinfo_encrypted_response_alg | RSA-OAEP                     |
	| userinfo_encrypted_response_enc | A256CBC-HS512                |
	| request_object_signing_alg      | RS256                        |
	| request_object_encryption_alg   | RSA-OAEP                     |
	| request_object_encryption_enc   | A256CBC-HS512                |
	| default_max_age                 | 2                            |
	| require_auth_time               | true                         |
	| default_acr_values              | ["a","b"]                    |
	| post_logout_redirect_uris       | ["http://localhost/logout"]  |
	| initiate_login_uri              | https://localhost/loginuri   |
	| dpop_bound_access_tokens        | true                         |

	And extract JSON from body
	
	Then HTTP status code equals to '201'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'response_types'
	Then JSON exists 'default_acr_values'
	Then JSON exists 'post_logout_redirect_uris'
	Then JSON 'token_endpoint_auth_method'='client_secret_jwt'
	Then JSON 'client_name'='name'
	Then JSON 'client_name#fr'='nom'
	Then JSON 'client_name#en'='name'
	Then JSON 'application_type'='web'
	Then JSON 'subject_type'='public'
	Then JSON 'id_token_signed_response_alg'='RS256'
	Then JSON 'id_token_encrypted_response_alg'='RSA-OAEP'
	Then JSON 'id_token_encrypted_response_enc'='A256CBC-HS512'
	Then JSON 'userinfo_signed_response_alg'='RS256'
	Then JSON 'userinfo_encrypted_response_alg'='RSA-OAEP'
	Then JSON 'userinfo_encrypted_response_enc'='A256CBC-HS512'
	Then JSON 'request_object_signing_alg'='RS256'
	Then JSON 'request_object_encryption_alg'='RSA-OAEP'
	Then JSON 'request_object_encryption_enc'='A256CBC-HS512'
	Then JSON 'default_max_age'='2'
	Then JSON 'require_auth_time'='true'
	Then JSON 'initiate_login_uri'='https://localhost/loginuri'	
	Then JSON 'dpop_bound_access_tokens'='true'

Scenario: Get a client
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                        |
	| Authorization                   | Bearer $access_token$        |
	| redirect_uris                   | ["https://web.com"]          |
	| response_types                  | ["token"]                    |
	| grant_types                     | ["implicit"]                 |
	| client_name                     | name                         |
	| client_name#fr                  | nom                          |
	| client_name#en                  | name                         |
	| application_type                | web                          |
	| token_endpoint_auth_method      | client_secret_jwt            |
	| sector_identifier_uri           | https://localhost/sector     |
	| subject_type                    | public                       |
	| id_token_signed_response_alg    | RS256                        |
	| id_token_encrypted_response_alg | RSA-OAEP                     |
	| id_token_encrypted_response_enc | A256CBC-HS512                |
	| userinfo_signed_response_alg    | RS256                        |
	| userinfo_encrypted_response_alg | RSA-OAEP                     |
	| userinfo_encrypted_response_enc | A256CBC-HS512                |
	| request_object_signing_alg      | RS256                        |
	| request_object_encryption_alg   | RSA-OAEP                     |
	| request_object_encryption_enc   | A256CBC-HS512                |
	| default_max_age                 | 2                            |
	| require_auth_time               | true                         |
	| default_acr_values              | ["a","b"]                    |
	| post_logout_redirect_uris       | ["http://localhost/logout"]  |
	| initiate_login_uri              | https://localhost/loginuri   |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'registration_access_token' from JSON body
	And execute HTTP GET request 'http://localhost/register/$client_id$'
	| Key           | Value                              |
	| Authorization | Bearer $registration_access_token$ |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'response_types'
	Then JSON exists 'default_acr_values'
	Then JSON exists 'post_logout_redirect_uris'
	Then JSON 'token_endpoint_auth_method'='client_secret_jwt'
	Then JSON 'client_name'='name'
	Then JSON 'client_name#fr'='nom'
	Then JSON 'client_name#en'='name'
	Then JSON 'application_type'='web'
	Then JSON 'subject_type'='public'
	Then JSON 'id_token_signed_response_alg'='RS256'
	Then JSON 'id_token_encrypted_response_alg'='RSA-OAEP'
	Then JSON 'id_token_encrypted_response_enc'='A256CBC-HS512'
	Then JSON 'userinfo_signed_response_alg'='RS256'
	Then JSON 'userinfo_encrypted_response_alg'='RSA-OAEP'
	Then JSON 'userinfo_encrypted_response_enc'='A256CBC-HS512'
	Then JSON 'request_object_signing_alg'='RS256'
	Then JSON 'request_object_encryption_alg'='RSA-OAEP'
	Then JSON 'request_object_encryption_enc'='A256CBC-HS512'
	Then JSON 'default_max_age'='2'
	Then JSON 'require_auth_time'='true'
	Then JSON 'initiate_login_uri'='https://localhost/loginuri'