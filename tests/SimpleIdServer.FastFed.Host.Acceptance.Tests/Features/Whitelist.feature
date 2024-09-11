Feature: Whitelist
	Check errors returned during whitelisting

Scenario: User agent is redirected to the identity provider handshake start uri after whitelisting
	When execute HTTP POST JSON request 'http://localhost/fastfed/whitelist'
	| Key					| Value						|
	| identity_provider_url | http://localhost/fastfed	|

	And extract parameter 'app_metadata_uri' from redirect url

	Then parameter 'app_metadata_uri'='http://localhost/fastfed/provider-metadata'