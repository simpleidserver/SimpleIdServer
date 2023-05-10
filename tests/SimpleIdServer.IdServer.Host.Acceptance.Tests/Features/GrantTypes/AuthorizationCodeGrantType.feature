Feature: AuthorizationCodeGrantType
	Check all the alternatives scenarios in code grant-type	
	
Scenario: resource parameter can be overridden and scopes 'admin', 'calendar' are returned
	Given authenticate a user
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                        |
	| response_type | code                         |
	| client_id     | fortySixClient               |
	| state         | state                        |
	| response_mode | query                        |
	| redirect_uri  | http://localhost:8080        |
	| nonce         | nonce                        |
	| resource      | https://cal.example.com      |
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fortySixClient               |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| redirect_uri  | http://localhost:8080	       |	
	| resource      | https://cal.example.com      |
	| resource      | https://contacts.example.com |

	And extract JSON from body

	Then HTTP status code equals to '200'
	
	And JSON 'scope'='admin calendar'
	And access_token audience contains 'https://cal.example.com'
	And access_token audience contains 'https://contacts.example.com'
	And access_token contains the claim 'scope'='admin'
	And access_token contains the claim 'scope'='calendar'

Scenario: scope of a resource can be filtered
	Given authenticate a user
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                        |
	| response_type | code                         |
	| client_id     | fortySixClient               |
	| state         | state                        |
	| response_mode | query                        |
	| redirect_uri  | http://localhost:8080        |
	| nonce         | nonce                        |
	| resource      | https://cal.example.com      |
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fortySixClient               |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| redirect_uri  | http://localhost:8080	       |	
	| resource      | https://cal.example.com      |
	| resource      | https://contacts.example.com |
	| scope         | admin                        |

	And extract JSON from body

	Then HTTP status code equals to '200'
	And access_token audience contains 'https://cal.example.com'
	And access_token audience contains 'https://contacts.example.com'
	And access_token contains the claim 'scope'='admin'
	And access_token doesn't contain the claim 'scope'='calendar'

Scenario: scopes 'admin', 'calendar' are returned thanks to the original request
	Given authenticate a user
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                        |
	| response_type | code                         |
	| client_id     | fortySixClient               |
	| state         | state                        |
	| response_mode | query                        |
	| redirect_uri  | http://localhost:8080        |
	| nonce         | nonce                        |
	| resource      | https://cal.example.com      |
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fortySixClient               |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| redirect_uri  | http://localhost:8080	       |

	And extract JSON from body
	
	Then JSON 'scope'='admin calendar'
	And access_token audience contains 'https://cal.example.com'
	And access_token contains the claim 'scope'='admin'
	And access_token contains the claim 'scope'='calendar'

Scenario: authorization_details are returned
	Given authenticate a user
	
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                   |
	| response_type           | code token                                              |
	| client_id               | fiftyFiveClient                                         |
	| state                   | state                                                   |
	| response_mode           | query                                                   |
	| redirect_uri            | http://localhost:8080                                   |
	| nonce                   | nonce                                                   |
	| authorization_details   |  { "type" : "firstDetails", "actions": [ "read" ] }     |
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fiftyFiveClient              |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| redirect_uri  | http://localhost:8080	       |

	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	Then JWT has authorization_details type 'firstDetails'
	And JWT has authorization_details action 'read'

Scenario: only one authorization_details is returned because there is one resource parameter
	Given authenticate a user
	
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                                                                |
	| response_type           | code token                                                                                           |
	| client_id               | fiftyFiveClient                                                                                      |
	| state                   | state                                                                                                |
	| response_mode           | query                                                                                                |
	| redirect_uri            | http://localhost:8080                                                                                |
	| nonce                   | nonce                                                                                                |
	| authorization_details   |  { "type" : "secondDetails", "locations": [ "https://cal.example.com" ], "actions": [ "read" ] }     |
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fiftyFiveClient              |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |	
	| resource      | https://cal.example.com      |
	| redirect_uri  | http://localhost:8080	       |

	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	Then JWT has authorization_details type 'secondDetails'
	And JWT has authorization_details action 'read'

Scenario: access token contains authorization_details with openid_credential
	Given authenticate a user
	
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                                                                                                                         |
	| response_type           | code token                                                                                                                    |
	| client_id               | fiftyEightClient                                                                                                              |
	| state                   | state                                                                                                                         |
	| response_mode           | query                                                                                                                         |
	| redirect_uri            | http://localhost:8080                                                                                                         |
	| nonce                   | nonce                                                                                                                         |
	| authorization_details   |  { "type" : "openid_credential", "format": "jwt_vc_json", "types": [ "VerifiableCredential", "UniversityDegreeCredential" ] } |
	
	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			       |
	| client_id     | fiftyEightClient             |
	| client_secret | password     			       |
	| grant_type    | authorization_code	       |
	| code			| $code$				       |
	| redirect_uri  | http://localhost:8080	       |

	And extract JSON from body
	And extract parameter 'access_token' from JSON body
	And extract payload from JWT '$access_token$'

	Then JWT has authorization_details type 'openid_credential'