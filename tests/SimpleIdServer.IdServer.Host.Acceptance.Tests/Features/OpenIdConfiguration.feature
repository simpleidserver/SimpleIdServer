Feature: OpenIdConfiguration
	Get the OpenIdConfiguration and check its content

Scenario: Get the configuration
	When execute HTTP GET request 'https://localhost:8080/.well-known/openid-configuration'
	| Key | Value |
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON 'userinfo_endpoint'='https://localhost:8080/userinfo'
	And JSON 'pushed_authorization_request_endpoint '='https://localhost:8080/par'
	And JSON 'require_pushed_authorization_requests'='false'