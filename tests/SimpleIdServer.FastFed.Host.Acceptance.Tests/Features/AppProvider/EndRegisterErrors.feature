Feature: EndRegisterErrors
	Check errors returned during the end of the registration

Scenario: Http header must be equals to 'application/jws'
	When execute HTTP POST request 'http://localhost/fastfed/register', content-type 'application/jwt', content 'jwt'
	
	And extract JSON from body

	Then HTTP status code equals to '401'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Content type must be equals to 'application/jws''

Scenario: Request content must be specified
	When execute HTTP POST request 'http://localhost/fastfed/register', content-type 'application/jws', content ''
	
	And extract JSON from body

	Then HTTP status code equals to '401'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='The registration request cannot be empty'

Scenario: Registration request must be a JWT
	When execute HTTP POST request 'http://localhost/fastfed/register', content-type 'application/jws', content 'jwt'
	
	And extract JSON from body

	Then HTTP status code equals to '401'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Registration request must be a JSON Web token'

Scenario: Jwt issuer must be a whitelisted identity provider
	Given build jwt and store the result into 'accessToken'
	| Key | Value   |
	| iss | invalid |
		
	When execute HTTP POST request 'http://localhost/fastfed/register', content-type 'application/jws', content '$accessToken$'
	
	And extract JSON from body

	Then HTTP status code equals to '401'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='The registration process cannot be completed because the issuer invalid is not in the whitelist'

Scenario: Jwt signature must be correct
	Given build jwt and store the result into 'accessToken'
	| Key | Value    |
	| iss | entityId |
	
	When execute HTTP POST request 'http://localhost/fastfed/register', content-type 'application/jws', content '$accessToken$'
	
	And extract JSON from body

	Then HTTP status code equals to '401'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='The JWK Kid kid doesn't exist'

Scenario: Jwt audience must be correct
	Given build jwt signed with certificate and store the result into 'accessToken'
	| Key | Value    |
	| iss | entityId |
	| aud | bad      |

	When execute HTTP POST request 'http://localhost/fastfed/register', content-type 'application/jws', content '$accessToken$'
	
	And extract JSON from body

	Then HTTP status code equals to '401'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='The aud attribute doesn't match the entity_id of the application provider'

Scenario: Federation whitelisting is expired
	Given build jwt signed with certificate and store the result into 'accessToken'
	| Key | Value              |
	| iss | expiredEntityId    |
	| aud | http://localhost   |

	When execute HTTP POST request 'http://localhost/fastfed/register', content-type 'application/jws', content '$accessToken$'
	
	And extract JSON from body

	Then HTTP status code equals to '401'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='The whitelisting process of the identity provider is expired'

Scenario: Capabilities must be adequate
	Given build jwt signed with certificate and store the result into 'accessToken'
	| Key                   | Value              |
	| iss                   | entityId           |
	| aud                   | http://localhost   |
	| provisioning_profiles | ["prov"]           |

	When execute HTTP POST request 'http://localhost/fastfed/register', content-type 'application/jws', content '$accessToken$'
	
	And extract JSON from body

	Then HTTP status code equals to '401'

	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Capabilities cannot be different'