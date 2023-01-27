Feature: UMAPermissionsErrors
	Check errors returned by the /perm	

Scenario: resource_id parameter is required
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter resource_id'

Scenario: resource_scopes parameter is required
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |
	| resource_id   | id                    |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter resource_scopes'

Scenario: resource_id must exists
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |
	| resource_id     | invalid               |
	| resource_scopes | [ "scope" ]           |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_resource_id'
	Then JSON 'error_description'='At least one of the provided resource identifiers was not found at the authorization server.'	

Scenario: scope must be valid
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP POST JSON request 'http://localhost/perm'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |
	| resource_id     | id                    |
	| resource_scopes | [ "invalid" ]         |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_scope'
	Then JSON 'error_description'='At least one of the scopes included in the request does not match an available scope for any of the resources associated with requested permissions for the permission ticket provided by the client.'


