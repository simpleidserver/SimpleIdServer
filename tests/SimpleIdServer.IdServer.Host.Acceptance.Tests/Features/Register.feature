Feature: Register
	Check client can be registered

Scenario: Register a complete client
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                      |
	| redirect_uris                   | [https://web.com]          |
	| response_types                  | [token]                    |
	| grant_types                     | [implicit]                 |
	| client_name                     | name                       |
	| client_name#fr                  | nom                        |
	| client_name#en                  | name                       |
	| application_type                | web                        |
	| token_endpoint_auth_method      | client_secret_jwt          |
	| sector_identifier_uri           | https://localhost/sector   |
	| subject_type                    | public                     |
	| id_token_signed_response_alg    | RS256                      |
	| id_token_encrypted_response_alg | RSA-OAEP                   |
	| id_token_encrypted_response_enc | A256CBC-HS512              |
	| userinfo_signed_response_alg    | RS256                      |
	| userinfo_encrypted_response_alg | RSA-OAEP                   |
	| userinfo_encrypted_response_enc | A256CBC-HS512              |
	| request_object_signing_alg      | RS256                      |
	| request_object_encryption_alg   | RSA-OAEP                   |
	| request_object_encryption_enc   | A256CBC-HS512              |
	| default_max_age                 | 2                          |
	| require_auth_time               | true                       |
	| default_acr_values              | [a,b]                      |
	| post_logout_redirect_uris       | [http://localhost/logout]  |
	| initiate_login_uri              | https://localhost/loginuri |

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
	Then JSON 'id_token_encrypted_response_alg'='RSA-OAEP-256'
	Then JSON 'id_token_encrypted_response_enc'='A256CBC-HS512'
	Then JSON 'userinfo_signed_response_alg'='RS256'
	Then JSON 'userinfo_encrypted_response_alg'='RSA-OAEP-256'
	Then JSON 'userinfo_encrypted_response_enc'='A256CBC-HS512'
	Then JSON 'request_object_signing_alg'='RS256'
	Then JSON 'request_object_encryption_alg'='RSA-OAEP-256'
	Then JSON 'request_object_encryption_enc'='A256CBC-HS512'
	Then JSON 'default_max_age'='2'
	Then JSON 'require_auth_time'=True
	Then JSON 'initiate_login_uri'='https://localhost/loginuri'