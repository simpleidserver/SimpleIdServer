Feature: EndRegister
	Check result returned during the end of registration

Scenario: Http header must be equals to 'application/jws'
	Given build jwt signed with certificate and store the result into 'accessToken'
	| Key                   | Value                                                            |
	| iss                   | entityId                                                         |
	| aud                   | http://localhost                                                 |
	| provisioning_profiles | ["urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise"] |
	
	When execute HTTP POST request 'http://localhost/fastfed/register', content-type 'application/jws', content '$accessToken$'
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON '$['urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise'].scim_service_uri'='http://localhost/scim'