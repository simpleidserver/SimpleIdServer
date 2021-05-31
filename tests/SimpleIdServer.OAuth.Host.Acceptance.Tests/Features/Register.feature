Feature: Register
	Check registration endpoint
	
Scenario: Create minimalist client
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key           | Value              |
	| redirect_uris | [http://localhost] |

	And extract JSON from body
	
	Then HTTP status code equals to '201'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'token_endpoint_auth_method'
	Then JSON exists 'response_types'
	Then JSON exists 'client_name'

Scenario: Create client
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                     |
	| redirect_uris              | [http://localhost]        |
	| response_types             | [token]                   |
	| grant_types                | [implicit]                |
	| client_name                | name                      |
	| client_name#fr             | nom                       |
	| client_name#en             | name                      |
	| client_uri                 | http://localhost          |
	| client_uri#fr              | http://localhost/fr       |
	| logo_uri                   | http://localhost/1.png    |
	| logo_uri#fr                | http://localhost/fr/1.png |
	| software_id                | software                  |
	| software_version           | 1.0                       |
	| token_endpoint_auth_method | client_secret_basic       |
	| scope                      | scope1                    |
	| contacts                   | [addr1,addr2]             |
	| tos_uri                    | http://localhost/tos      |
	| policy_uri                 | http://localhost/policy   |
	| jwks_uri                   | http://localhost/jwks     |

	And extract JSON from body
	
	Then HTTP status code equals to '201'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'response_types'
	Then JSON exists 'contacts'
	Then JSON 'token_endpoint_auth_method'='client_secret_basic'
	Then JSON 'client_uri'='http://localhost'
	Then JSON 'client_uri#fr'='http://localhost/fr'
	Then JSON 'logo_uri'='http://localhost/1.png'
	Then JSON 'logo_uri#fr'='http://localhost/fr/1.png'
	Then JSON 'scope'='scope1'
	Then JSON 'tos_uri'='http://localhost/tos'
	Then JSON 'policy_uri'='http://localhost/policy'
	Then JSON 'jwks_uri'='http://localhost/jwks'
	Then JSON 'client_name'='name'
	Then JSON 'client_name#fr'='nom'
	Then JSON 'client_name#en'='name'
	Then JSON 'software_id'='software'
	Then JSON 'software_version'='1.0'

Scenario: Use software_statement parameter to create a client
	When build software statement
	| Key           | Value              |
	| iss           | iss                |
	| redirect_uris | [http://localhost] |
	
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                | Value               |
	| software_statement | $softwareStatement$ |
	
	And extract JSON from body

	Then HTTP status code equals to '201'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'token_endpoint_auth_method'
	Then JSON exists 'response_types'
	Then JSON exists 'client_name'

Scenario: Get client
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                     |
	| redirect_uris              | [http://localhost]        |
	| response_types             | [token]                   |
	| grant_types                | [implicit]                |
	| client_name                | name                      |
	| client_name#fr             | nom                       |
	| client_name#en             | name                      |
	| client_uri                 | http://localhost          |
	| client_uri#fr              | http://localhost/fr       |
	| logo_uri                   | http://localhost/1.png    |
	| logo_uri#fr                | http://localhost/fr/1.png |
	| software_id                | software                  |
	| software_version           | 1.0                       |
	| token_endpoint_auth_method | client_secret_basic       |
	| scope                      | scope1                    |
	| contacts                   | [addr1,addr2]             |
	| tos_uri                    | http://localhost/tos      |
	| policy_uri                 | http://localhost/policy   |
	| jwks_uri                   | http://localhost/jwks     |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body into 'clientId'
	And extract parameter 'registration_access_token' from JSON body into 'registrationAccessToken'
	And execute HTTP GET request 'http://localhost/register/$clientId$'
	| Key           | Value                            |
	| Authorization | Bearer $registrationAccessToken$ |

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'response_types'
	Then JSON exists 'contacts'
	Then JSON 'token_endpoint_auth_method'='client_secret_basic'
	Then JSON 'client_uri'='http://localhost'
	Then JSON 'client_uri#fr'='http://localhost/fr'
	Then JSON 'logo_uri'='http://localhost/1.png'
	Then JSON 'logo_uri#fr'='http://localhost/fr/1.png'
	Then JSON 'scope'='scope1'
	Then JSON 'tos_uri'='http://localhost/tos'
	Then JSON 'policy_uri'='http://localhost/policy'
	Then JSON 'jwks_uri'='http://localhost/jwks'
	Then JSON 'client_name'='name'
	Then JSON 'client_name#fr'='nom'
	Then JSON 'client_name#en'='name'
	Then JSON 'software_id'='software'
	Then JSON 'software_version'='1.0'

Scenario: Update client
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                     |
	| redirect_uris              | [http://localhost]        |
	| response_types             | [token]                   |
	| grant_types                | [implicit]                |
	| client_name                | name                      |
	| client_name#fr             | nom                       |
	| client_name#en             | name                      |
	| client_uri                 | http://localhost          |
	| client_uri#fr              | http://localhost/fr       |
	| logo_uri                   | http://localhost/1.png    |
	| logo_uri#fr                | http://localhost/fr/1.png |
	| software_id                | software                  |
	| software_version           | 1.0                       |
	| token_endpoint_auth_method | client_secret_basic       |
	| scope                      | scope1                    |
	| contacts                   | [addr1,addr2]             |
	| tos_uri                    | http://localhost/tos      |
	| policy_uri                 | http://localhost/policy   |
	| jwks_uri                   | http://localhost/jwks     |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body into 'clientId'
	And extract parameter 'registration_access_token' from JSON body into 'registrationAccessToken'

	And execute HTTP PUT JSON request 'https://localhost:8080/register/$clientId$'
	| Key                        | Value                            |
	| Authorization              | Bearer $registrationAccessToken$ |
	| client_id                  | $clientId$                       |
	| redirect_uris              | [http://localhost]               |
	| response_types             | [token]                          |
	| grant_types                | [implicit]                       |
	| client_name                | name                             |
	| client_name#fr             | nom                              |
	| client_name#en             | name                             |
	| client_uri                 | http://clienturi                 |
	| client_uri#fr              | http://clienturi/fr              |
	| logo_uri                   | http://logouri/1.png             |
	| logo_uri#fr                | http://logouri/fr/1.png          |
	| software_id                | software                         |
	| software_version           | 1.0                              |
	| token_endpoint_auth_method | client_secret_basic              |
	| scope                      | scope1                           |
	| contacts                   | [addr1,addr2]                    |
	| tos_uri                    | http://tosuri/tos                |
	| policy_uri                 | http://policyuri/policy          |
	| jwks_uri                   | http://jwksuri/jwks              |

	And execute HTTP GET request 'https://localhost:8080/register/$clientId$'
	| Key           | Value                            |
	| Authorization | Bearer $registrationAccessToken$ |

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'client_id'
	Then JSON exists 'client_secret'
	Then JSON exists 'client_id_issued_at'
	Then JSON exists 'grant_types'
	Then JSON exists 'redirect_uris'
	Then JSON exists 'response_types'
	Then JSON exists 'contacts'
	Then JSON 'token_endpoint_auth_method'='client_secret_basic'
	Then JSON 'client_uri'='http://clienturi'
	Then JSON 'client_uri#fr'='http://clienturi/fr'
	Then JSON 'logo_uri'='http://logouri/1.png'
	Then JSON 'logo_uri#fr'='http://logouri/fr/1.png'
	Then JSON 'scope'='scope1'
	Then JSON 'tos_uri'='http://tosuri/tos'
	Then JSON 'policy_uri'='http://policyuri/policy'
	Then JSON 'jwks_uri'='http://jwksuri/jwks'
	Then JSON 'client_name'='name'
	Then JSON 'client_name#fr'='nom'
	Then JSON 'client_name#en'='name'
	Then JSON 'software_id'='software'
	Then JSON 'software_version'='1.0'


Scenario: Delete client
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key                        | Value                     |
	| redirect_uris              | [http://localhost]        |
	| response_types             | [token]                   |
	| grant_types                | [implicit]                |
	| client_name                | name                      |
	| client_name#fr             | nom                       |
	| client_name#en             | name                      |
	| client_uri                 | http://localhost          |
	| client_uri#fr              | http://localhost/fr       |
	| logo_uri                   | http://localhost/1.png    |
	| logo_uri#fr                | http://localhost/fr/1.png |
	| software_id                | software                  |
	| software_version           | 1.0                       |
	| token_endpoint_auth_method | client_secret_basic       |
	| scope                      | scope1                    |
	| contacts                   | [addr1,addr2]             |
	| tos_uri                    | http://localhost/tos      |
	| policy_uri                 | http://localhost/policy   |
	| jwks_uri                   | http://localhost/jwks     |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body into 'clientId'
	And extract parameter 'registration_access_token' from JSON body into 'registrationAccessToken'

	And execute HTTP DELETE request 'https://localhost:8080/register/$clientId$'
	| Key           | Value                            |
	| Authorization | Bearer $registrationAccessToken$ |

	
	And execute HTTP GET request 'https://localhost:8080/register/$clientId$'
	| Key           | Value                            |
	| Authorization | Bearer $registrationAccessToken$ |

	And extract JSON from body

	Then HTTP status code equals to '401'
	Then JSON 'error'='invalid_token'
	Then JSON 'error_description'='access token is not correct'