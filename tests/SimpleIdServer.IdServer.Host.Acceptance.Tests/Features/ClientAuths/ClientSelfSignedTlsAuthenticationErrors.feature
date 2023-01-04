Feature: ClientSelfSignedTlsAuthenticationErrors
	Check errors returned during the 'self_signed_tls_client_auth' authentication

Scenario: Error is returned when there is no certificate
	When execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key         | Value              |
	| grant_type  | client_credentials |
	| scope       | scope              |
	| client_id   | elevenClient       |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='certificate is required'

Scenario: Error is returned when certificate is not correct
	Given build random X509Certificate2 and store into 'clientCertificate'
	
	When execute HTTP POST request 'https://localhost:8080/mtls/token'
	| Key                  | Value              |
	| grant_type           | client_credentials |
	| scope                | scope              |
	| client_id            | elevenClient       |
	| X-Testing-ClientCert | clientCertificate  |

	And extract JSON from body
	Then HTTP status code equals to '401'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='the certificate is not correct'