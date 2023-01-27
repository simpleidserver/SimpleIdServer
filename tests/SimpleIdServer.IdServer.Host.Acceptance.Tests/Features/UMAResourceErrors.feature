Feature: UMAResourceErrors
	Check errors returned by the /rreguri	

Scenario: cannot retrieve unknown UMA resource
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP GET request 'http://localhost/rreguri/1'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'
	Then JSON 'error_description'='the UMA resource doesn't exist'

Scenario: Impossible to add UMA resource when resource_scopes parameter is missing
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter resource_scopes'

Scenario: Impossible to add UMA resource when subject parameter is missing
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP POST JSON request 'http://localhost/rreguri'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |
	| resource_scopes | [ "scope" ]           |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter subject'

Scenario: Impossible to update UMA resource when resource_scopes is missing
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP PUT JSON request 'http://localhost/rreguri/id'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter resource_scopes'

Scenario: Impossible to update an unknown UMA resource
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP PUT JSON request 'http://localhost/rreguri/unknown'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |
	| resource_scopes | [ "scope" ]           |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'
	Then JSON 'error_description'='the UMA resource doesn't exist'

Scenario: Impossible to remove an unknown UMA resource
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP DELETE request 'http://localhost/rreguri/unknown'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'
	Then JSON 'error_description'='the UMA resource doesn't exist'

Scenario: Impossible to update the permissions when the permissions parameter is missing
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP PUT JSON request 'http://localhost/rreguri/id/permissions'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter permissions'

Scenario: Impossible to update the permissions when the UMA resource doesn't exist
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP PUT JSON request 'http://localhost/rreguri/unknown/permissions'
	| Key             | Value                                                                   |
	| Authorization   | Bearer $access_token$                                                   |
	| permissions     | [ { claims: [ { name: "sub", value: "user" } ], scopes: [ "scope" ] } ] |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'
	Then JSON 'error_description'='the UMA resource doesn't exist'

Scenario: Impossible to remove permissions when the UMA resource doesn't exist
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP DELETE request 'http://localhost/rreguri/unknown/permissions'
	| Key             | Value                  |
	| Authorization   | Bearer $access_token$  |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'
	Then JSON 'error_description'='the UMA resource doesn't exist'

Scenario: Impossible to get permissions when the UMA resource doesn't exist
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftyThreeClient   |
	| client_secret | password           |
	| scope         | uma_protection     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP GET request 'http://localhost/rreguri/unknown/permissions'
	| Key             | Value                  |
	| Authorization   | Bearer $access_token$  |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'error'='not_found'
	Then JSON 'error_description'='the UMA resource doesn't exist'