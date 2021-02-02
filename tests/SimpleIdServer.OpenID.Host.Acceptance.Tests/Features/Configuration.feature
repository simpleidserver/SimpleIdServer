Feature: Configuration
	Check the .well-known/openid-configuration endpoint

Scenario: Configuration endpoint contains the correct values
	When execute HTTP GET request 'http://localhost/.well-known/openid-configuration'
	| Key | Value |
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON 'issuer'='http://localhost'
	Then JSON 'authorization_endpoint'='http://localhost/authorization'
	Then JSON 'token_endpoint'='http://localhost/token'
	Then JSON 'userinfo_endpoint'='http://localhost/userinfo'
	Then JSON 'jwks_uri'='http://localhost/jwks'
	Then JSON 'registration_endpoint'='http://localhost/register'
	Then JSON contains 'scopes_supported'
	Then JSON contains 'response_types_supported'
	Then JSON contains 'response_modes_supported'
	Then JSON contains 'grant_types_supported'
	Then JSON contains 'acr_values_supported'
	Then JSON contains 'subject_types_supported'
	Then JSON contains 'id_token_signing_alg_values_supported'
	Then JSON contains 'id_token_encryption_alg_values_supported'
	Then JSON contains 'id_token_encryption_enc_values_supported'
	Then JSON contains 'userinfo_signing_alg_values_supported'
	Then JSON contains 'userinfo_encryption_alg_values_supported'
	Then JSON contains 'userinfo_encryption_enc_values_supported'
	Then JSON contains 'request_object_signing_alg_values_supported'
	Then JSON contains 'request_object_encryption_alg_values_supported'
	Then JSON contains 'request_object_encryption_enc_values_supported'
	Then JSON contains 'token_endpoint_auth_methods_supported'
	Then JSON contains 'token_endpoint_auth_signing_alg_values_supported'