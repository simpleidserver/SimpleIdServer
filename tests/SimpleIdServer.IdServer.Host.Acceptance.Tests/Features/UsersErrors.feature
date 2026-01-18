Feature: UsersErrors
	Check errors returned by the /users endpoint

Scenario: access token must be passed (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/users'
	| Key | Value |

	And extract JSON from body

	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: access token must be passed (HTTP GET)
	When execute HTTP GET request 'https://localhost:8080/users/id'
	| Key | Value |

	And extract JSON from body

	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: access token must be passed (HTTP DELETE)
	When execute HTTP DELETE request 'http://localhost/users/id'
	| Key | Value |

	And extract JSON from body

	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: access token must be passed (HTTP PUT Credentials)
	When execute HTTP PUT JSON request 'http://localhost/users/id/credentials/cred'
	| Key | Value |

	And extract JSON from body

	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: name is required
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP POST JSON request 'http://localhost/users'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |	

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='missing parameter name'

Scenario: name must be unique
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP POST JSON request 'http://localhost/users'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |	
	| name            | user                  |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='the user user already exists'

Scenario: get an unknown user
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP GET request 'http://localhost/users/id'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		
	
	Then HTTP status code equals to '404'

Scenario: remove an unknown user
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP DELETE request 'http://localhost/users/id'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		
	
	Then HTTP status code equals to '404'

Scenario: cannot update credential when user doesn't exist
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP PUT JSON request 'http://localhost/users/id/credentials/cred'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		
	| type            | pwd                   |
	| value           | value                 |
	
	Then HTTP status code equals to '404'

Scenario: access token must be passed (HTTP PUT Password)
	When execute HTTP PUT JSON request 'http://localhost/users/id/pwd'
	| Key | Value |

	And extract JSON from body

	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: value is required to update password
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP PUT JSON request 'http://localhost/users/id/pwd'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |		

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='missing parameter value'

Scenario: old_value is required to update password
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP PUT JSON request 'http://localhost/users/id/pwd'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |
	| value           | NewPassword1!         |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='missing parameter old_value'

Scenario: cannot update password when user doesn't exist
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	When execute HTTP PUT JSON request 'http://localhost/users/unknownid/pwd'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |
	| old_value       | oldpassword           |
	| value           | NewPassword1!         |
	
	Then HTTP status code equals to '404'

Scenario: cannot update password when user has no active password credential
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	And execute HTTP POST JSON request 'http://localhost/users'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |	
	| name            | userWithoutPwd        |

	And extract JSON from body
	And extract parameter 'id' from JSON body

	When execute HTTP PUT JSON request 'http://localhost/users/$id$/pwd'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |
	| old_value       | oldpassword           |
	| value           | NewPassword1!         |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	And JSON 'error_description'='no active password credential found for this user'

Scenario: cannot update password when old password is invalid
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | users              |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	
	
	And execute HTTP POST JSON request 'http://localhost/users'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |	
	| name            | userWithPwd           |

	And extract JSON from body
	And extract parameter 'id' from JSON body

	And execute HTTP POST JSON request 'http://localhost/users/$id$/credentials'
	| Key             | Value                                              |
	| Authorization   | Bearer $access_token$                              |
	| credential      | { "type": "pwd", "value": "Password1!" }           |
	| active          | true                                               |

	When execute HTTP PUT JSON request 'http://localhost/users/$id$/pwd'
	| Key             | Value                 |
	| Authorization   | Bearer $access_token$ |
	| old_value       | wrongpassword         |
	| value           | NewPassword1!         |

	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_request'
	And JSON 'error_description'='the old password is invalid'