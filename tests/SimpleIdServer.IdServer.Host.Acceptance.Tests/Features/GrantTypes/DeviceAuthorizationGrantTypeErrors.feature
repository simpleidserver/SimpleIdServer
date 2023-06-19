Feature: DeviceAuthorizationGrantTypeErrors
	Check all the errors from urn:ietf:params:oauth:grant-type:device_code
	
Scenario: client_id is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			                       |
	| grant_type    | urn:ietf:params:oauth:grant-type:device_code |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='missing client_id'

Scenario: device_code is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			                       |
	| grant_type    | urn:ietf:params:oauth:grant-type:device_code |
	| client_id     | sixtyOneClient                               |
	| client_secret | password                                     |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter device_code'

Scenario: device_code must exists
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			                       |
	| grant_type    | urn:ietf:params:oauth:grant-type:device_code |
	| client_id     | sixtyOneClient                               |
	| client_secret | password                                     |
	| device_code   | unknown                                      |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the device_code doesn't exist'

Scenario: device_code must not be issued
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			                       |
	| grant_type    | urn:ietf:params:oauth:grant-type:device_code |
	| client_id     | sixtyOneClient                               |
	| client_secret | password                                     |
	| device_code   | issuedDeviceCode                             |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the device code has already been used to get a token'

Scenario: device_code must not be expired
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			                       |
	| grant_type    | urn:ietf:params:oauth:grant-type:device_code |
	| client_id     | sixtyOneClient                               |
	| client_secret | password                                     |
	| device_code   | expiredDeviceCode                            |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	And JSON '$.error'='expired_token'
	And JSON '$.error_description'='the device code is expired'

Scenario: cannot get the device_code too many times
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			                       |
	| grant_type    | urn:ietf:params:oauth:grant-type:device_code |
	| client_id     | sixtyOneClient                               |
	| client_secret | password                                     |
	| device_code   | tooManyDeviceCode                            |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	And JSON '$.error'='slow_down'
	And JSON '$.error_description'='you tried too many times to get a token'

Scenario: cannot get an access token if the authorization device code has a PENDING state
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			                       |
	| grant_type    | urn:ietf:params:oauth:grant-type:device_code |
	| client_id     | sixtyOneClient                               |
	| client_secret | password                                     |
	| device_code   | pendingDeviceCode                            |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	And JSON '$.error'='authorization_pending'
	And JSON '$.error_description'='authorization request is still pending as the end user hasn't yet completed the user-interation steps'