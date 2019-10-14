Feature: ServiceProviderConfig
	Check the /ServiceProviderConfig endpoint

Scenario: Check informations returned by ServiceProviderConfig endpoint
	When execute HTTP GET request 'http://localhost/ServiceProviderConfig'

	And extract JSON from body

	Then HTTP status code equals to '200'	
	Then JSON exists 'authenticationSchemes'
	Then JSON 'patch.supported'='true'
	Then JSON 'bulk.maxOperations'='1000'
	Then JSON 'bulk.maxPayloadSize'='1048576'
	Then JSON 'filter.supported'='true'
	Then JSON 'filter.maxResults'='200'
	Then JSON 'changePassword.supported'='false'
	Then JSON 'etag.supported'='false'