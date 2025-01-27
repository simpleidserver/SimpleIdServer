Feature: TokenExchangeGrantTypeErrors
	Check errors returned when using 'urn:ietf:params:oauth:grant-type:token-exchange' grant-type

Scenario: Parameter 'subject_token' is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value                                           |
	| grant_type    | urn:ietf:params:oauth:grant-type:token-exchange |
	| client_id     | sixtySixClient                                  |
	| client_secret | password                                        |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter subject_token'

Scenario: Parameter 'subject_token_type' is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value                                           |
	| grant_type    | urn:ietf:params:oauth:grant-type:token-exchange |
	| client_id     | sixtySixClient                                  |
	| client_secret | password                                        |
	| subject_token | subject                                         |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter subject_token_type'

Scenario: Parameter 'subject_token_type' is not recognized
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                | Value                                           |
	| grant_type         | urn:ietf:params:oauth:grant-type:token-exchange |
	| client_id          | sixtySixClient                                  |
	| client_secret      | password                                        |
	| subject_token      | subject                                         |
	| subject_token_type | tokentype                                       |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the token type tokentype is not supported'

Scenario: Parameter 'requested_token_type' is not recognized
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                                           |
	| grant_type           | urn:ietf:params:oauth:grant-type:token-exchange |
	| client_id            | sixtySixClient                                  |
	| client_secret        | password                                        |
	| subject_token        | subject                                         |
	| subject_token_type   | urn:ietf:params:oauth:token-type:access_token   |
    | requested_token_type | invalid                                         |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the requested token type invalid is not supported'

Scenario: Subject cannot be extracted from the subject_token
	Given build access_token and sign with the key 'keyid'
	| Key   | Value                  |
	| key   | value                  |
	| iss   | https://localhost:8080 |

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                                           |
	| grant_type           | urn:ietf:params:oauth:grant-type:token-exchange |
	| client_id            | sixtySixClient                                  |
	| client_secret        | password                                        |
	| subject_token        | $access_token$                                  |
	| subject_token_type   | urn:ietf:params:oauth:token-type:access_token   |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the subject cannot be extracted from the subject_token'

Scenario: The parameter 'actor_subject_type' is not recognized
	Given build access_token and sign with the key 'keyid'
	| Key         | Value                  |
	| client_id   | clientId               |
	| iss         | https://localhost:8080 |

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                  | Value                                           |
	| grant_type           | urn:ietf:params:oauth:grant-type:token-exchange |
	| client_id            | sixtySixClient                                  |
	| client_secret        | password                                        |
	| subject_token        | $access_token$                                  |
	| subject_token_type   | urn:ietf:params:oauth:token-type:access_token   |
	| actor_token_type     | invalid                                         |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the actor type invalid is not supported'



