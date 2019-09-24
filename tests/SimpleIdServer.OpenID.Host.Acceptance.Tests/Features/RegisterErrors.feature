Feature: RegisterErrors
	Check errors returned by client registration endpoint

Scenario: Error is returned when application_type is incorrect
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key				| Value													|
	| application_type	| unknown												|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='application type is invalid'

Scenario: Error is returned when sectore_identifier_uri is incorrect
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key					| Value													|
	| sector_identifier_uri	| unknown												|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='sector_identifier_uri is not a valid URI'

Scenario: Error is returned when sectore_identifier_uri doesn't contains HTTPS scheme
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key					| Value													|
	| sector_identifier_uri	| http://localhost										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='sector_identifier_uri doesn't contain https scheme'

Scenario: Error is returned when subject_type is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key					| Value											|
	| subject_type			| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='subject_type is invalid'

Scenario: Error is returned when id_token_signed_response_alg is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key									| Value											|
	| id_token_signed_response_alg			| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='id_token_signed_response_alg is not supported'

Scenario: Error is returned when id_token_encrypted_response_alg is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value											|
	| id_token_encrypted_response_alg			| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='id_token_encrypted_response_alg is not supported'

Scenario: Error is returned when id_token_encrypted_response_enc is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value											|
	| id_token_encrypted_response_enc			| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='id_token_encrypted_response_enc is not supported'

Scenario: Error is returned when id_token_encrypted_response_enc is passed but not id_token_signed_response_alg
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value												|
	| id_token_encrypted_response_enc			| A128CBC-HS256										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='missing parameter id_token_encrypted_response_alg'	

Scenario: Error is returned when userinfo_signed_response_alg is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key									| Value											|
	| userinfo_signed_response_alg			| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='userinfo_signed_response_alg is not supported'

Scenario: Error is returned when userinfo_encrypted_response_alg is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value											|
	| userinfo_encrypted_response_alg			| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='userinfo_encrypted_response_alg is not supported'

Scenario: Error is returned when userinfo_encrypted_response_enc is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value											|
	| userinfo_encrypted_response_enc			| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='userinfo_encrypted_response_enc is not supported'

Scenario: Error is returned when userinfo_signed_response_enc is passed but not userinfo_encrypted_response_alg
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value												|
	| userinfo_encrypted_response_enc			| A128CBC-HS256										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='missing parameter userinfo_encrypted_response_alg'	

Scenario: Error is returned when request_object_signing_alg is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key									| Value											|
	| request_object_signing_alg			| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='request_object_signing_alg is not supported'

Scenario: Error is returned when request_object_encryption_alg is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value											|
	| request_object_encryption_alg				| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='request_object_encryption_alg is not supported'

Scenario: Error is returned when request_object_encryption_enc is not supported
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value											|
	| request_object_encryption_enc				| unknown										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='request_object_encryption_enc is not supported'

Scenario: Error is returned when request_object_encryption_enc is passed but not request_object_encryption_alg
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value												|
	| request_object_encryption_enc				| A128CBC-HS256										|

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='missing parameter request_object_encryption_alg'

Scenario: Error is returned when a web client has invalid redirect_uri
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value												|
	| redirect_uris								| [invalid]											|
	| application_type							| web												|

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='redirect uri invalid is not correct'

Scenario: Error is returned when a native client has invalid redirect_uri
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value												|
	| redirect_uris								| [invalid]											|
	| application_type							| native											|

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='redirect uri invalid is not correct'

Scenario: Error is returned when a web client has redirect_uri without https scheme
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value												|
	| redirect_uris								| [http://localhost]								|

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='redirect_uri does not contain https scheme'

Scenario: Error is returned when a web client has redirect_uri with localhost
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value												|
	| redirect_uris								| [https://localhost]								|

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='redirect_uri must not contain localhost'