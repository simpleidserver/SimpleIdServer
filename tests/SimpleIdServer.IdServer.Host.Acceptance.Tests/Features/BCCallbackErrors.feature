Feature: BCCallbackErrors
	Check errors returned by the /bc-callback endpoint	
	
Scenario: access token is required
	When execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                 |

	And extract JSON from body
	
	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: access token must be valid
	Given build JWS by signing with a random RS256 algorithm and store the result into 'accessToken'
	| Key  | Value |
	| user | user  |

	When execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |

	And extract JSON from body
	
	Then HTTP status code equals to '401'
	And JSON 'error'='access_denied'

Scenario: authorization request must exists
	Given build access_token and sign with the key 'keyid'
	| Key | Value |
	| sub | user  |

	When execute HTTP POST JSON request 'http://localhost/bc-callback'
	| Key           | Value                  |
	| Authorization | Bearer $access_token$  |
	| auth_req_id   | invalid                |

	And extract JSON from body

	Then HTTP status code equals to '404'
	And JSON 'error'='invalid_request'
	And JSON 'error_description'='the back channel authorization invalid doesn't exist'