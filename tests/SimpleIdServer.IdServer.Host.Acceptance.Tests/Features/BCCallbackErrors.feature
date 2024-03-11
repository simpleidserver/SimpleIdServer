Feature: BCCallbackErrors
	Check errors returned by the /bc-callback endpoint	

Scenario: authorization request must exists
	Given build access_token and sign with the key 'keyid'
	| Key | Value |
	| sub | user  |

	When execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                  |
	| Authorization | Bearer $access_token$  |
	| auth_req_id   | id                     |

	And extract JSON from body

	Then HTTP status code equals to '404'
	And JSON 'error'='invalid_request'
	And JSON 'error_description'='the back channel authorization id doesn't exist'

Scenario: authorization request must be active
	Given build access_token and sign with the key 'keyid'
	| Key | Value |
	| sub | user  |

	When execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                  |
	| Authorization | Bearer $access_token$  |
	| auth_req_id   | expiredBC              |

	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_request'
	And JSON 'error_description'='the authorization request is expired'

Scenario: authorization request must be pending
	Given build access_token and sign with the key 'keyid'
	| Key | Value |
	| sub | user  |

	When execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                  |
	| Authorization | Bearer $access_token$  |
	| auth_req_id   | confirmedBC            |

	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON 'error'='invalid_request'
	And JSON 'error_description'='the authorization request is not in pending'