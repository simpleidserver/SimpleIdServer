Feature: SingleSignOnErrors
	Check Errors returned by SingleSignOn

Scenario: Check error is returned when SAMLRequest is missing
	When execute HTTP GET request 'https://localhost/SSO/Login'
	| Key | Value |
	And extract XML from body

	Then HTTP status code equals to '400'
	Then XML element 'saml:Status/saml:StatusMessage'='Parameter saml is missing'
	Then XML attribute 'saml:Status/saml:StatusCode/@Value'='urn:oasis:names:tc:SAML:2.0:status:Requester'

Scenario: Check error is returned when RelayState is missing
	When execute HTTP GET request 'https://localhost/SSO/Login?SAMLRequest=samlRequest'
	| Key | Value |
	And extract XML from body

	Then HTTP status code equals to '400'
	Then XML element 'saml:Status/saml:StatusMessage'='Parameter relayState is missing'
	Then XML attribute 'saml:Status/saml:StatusCode/@Value'='urn:oasis:names:tc:SAML:2.0:status:Requester'

Scenario: Check error is returned when SAMLRequest cannot be decompressed
	When execute HTTP GET request 'https://localhost/SSO/Login?SAMLRequest=samlRequest&RelayState=token'
	| Key | Value |
	And extract XML from body

	Then HTTP status code equals to '400'
	Then XML element 'saml:Status/saml:StatusMessage'='Saml cannot be decompressed'
	Then XML attribute 'saml:Status/saml:StatusCode/@Value'='urn:oasis:names:tc:SAML:2.0:status:Requester'

Scenario: Check error is returned when SAMLRequest is not an xml file
	When execute HTTP GET request 'https://localhost/SSO/Login?SAMLRequest=Ki4pAgAAAP%2f%2fAwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA%3d%3d&RelayState=token'
	| Key | Value |
	And extract XML from body

	Then HTTP status code equals to '400'
	Then XML element 'saml:Status/saml:StatusMessage'='Saml cannot be deserialized'
	Then XML attribute 'saml:Status/saml:StatusCode/@Value'='urn:oasis:names:tc:SAML:2.0:status:Requester'