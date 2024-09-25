Feature: ProviderMetadata
	Check result returned by the /fastfed/provider-metadata endpoint

Scenario: Provider metadata is correct
	When execute HTTP GET request 'http://localhost/fastfed/provider-metadata'
	| Key | Value |

	And extract JSON from body

	Then JSON '$.application_provider.provider_domain'='localhost'
	Then JSON '$.application_provider.capabilities.provisioning_profiles[0]'='urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise'
	Then JSON '$.application_provider.capabilities.schema_grammars[0]'='urn:ietf:params:fastfed:1.0:schemas:scim:2.0'
	Then JSON '$.application_provider.capabilities.signing_algorithms[0]'='RS256'
	Then JSON '$.identity_provider.provider_domain'='localhost'
	Then JSON '$.identity_provider.capabilities.provisioning_profiles[0]'='urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise'
	Then JSON '$.identity_provider.capabilities.schema_grammars[0]'='urn:ietf:params:fastfed:1.0:schemas:scim:2.0'
	Then JSON '$.identity_provider.capabilities.signing_algorithms[0]'='RS256'
	Then JSON '$.identity_provider.provider_contact_information.email'='support@example.com'
	Then JSON '$.identity_provider.provider_contact_information.organization'='Example Inc.'
	Then JSON '$.identity_provider.provider_contact_information.phone'='+1-800-555-5555'
	Then JSON '$.identity_provider.display_settings.display_name'='Example Identity Provider'