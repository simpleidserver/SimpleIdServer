Feature: TokenExchangePreAuthorizedCodeErrors
	Check errors returned when using 'urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code' grant-type

Scenario: client_id is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value                                                            |
	| grant_type | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code	|

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='missing client_id'

Scenario: client must exists
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value                                                            |
	| grant_type | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code	|
	| client_id  | invalid                                                          |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='unknown client invalid'

Scenario: client must supports the grant-type
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value                                                         |
	| grant_type    | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code |
	| client_id     | fiftySevenClient                                              |
	| client_secret | password                                                      |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='grant type urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code is not supported by the client'

Scenario: subject_token is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value                                                         |
	| grant_type    | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code |
	| client_id     | seventyTwoClient                                              |
	| client_secret | password                                                      |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter subject_token'

Scenario: subject_token_type is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                 | Value                                                         |
	| grant_type          | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code |
	| client_id           | seventyTwoClient                                              |
	| client_secret       | password                                                      |
	| subject_token       | sub                                                           |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter subject_token_type'

Scenario: scope is required
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                 | Value                                                         |
	| grant_type          | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code |
	| client_id           | seventyTwoClient                                              |
	| client_secret       | password                                                      |
	| subject_token       | sub                                                           |
	| subject_token_type  | type                                                          |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter scope'

Scenario: subject_token_type must be supported
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                 | Value                                                         |
	| grant_type          | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code |
	| client_id           | seventyTwoClient                                              |
	| client_secret       | password                                                      |
	| subject_token       | sub                                                           |
	| subject_token_type  | type                                                          |
	| scope               | Credential                                                    |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the token type type is not supported'

Scenario: Subject cannot be extracted from the subject_token
	Given build access_token and sign with the key 'keyid'
	| Key   | Value |
	| key   | value |

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                 | Value                                                         |
	| grant_type          | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code |
	| client_id           | seventyTwoClient                                              |
	| client_secret       | password                                                      |
	| subject_token       | $access_token$                                                |
	| subject_token_type  | type                                                          |
	| scope               | Credential                                                    |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='the token type type is not supported'

Scenario: Scopes must be supported
	Given build access_token and sign with the key 'keyid'
	| Key       | Value  |
	| sub       | user   |
	| client_id | client |
	| iss | http://localhost |

	When execute HTTP POST request 'http://localhost/token'
	| Key                 | Value                                                         |
	| grant_type          | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code |
	| client_id           | seventyTwoClient                                              |
	| client_secret       | password                                                      |
	| subject_token       | $access_token$                                                |
	| subject_token_type  | urn:ietf:params:oauth:token-type:access_token                 |
	| scope               | Credential                                                    |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_scope'
	And JSON '$.error_description'='unknown scope Credential'

Scenario: User must have an active OTP
	Given build access_token and sign with the key 'keyid'
	| Key   | Value |
	| sub   | user  |
	| iss | http://localhost |

	When execute HTTP POST request 'http://localhost/token'
	| Key                 | Value                                                         |
	| grant_type          | urn:ietf:params:oauth:grant-type:exchange-pre-authorized_code |
	| client_id           | seventyTwoClient                                              |
	| client_secret       | password                                                      |
	| subject_token       | $access_token$                                                |
	| subject_token_type  | urn:ietf:params:oauth:token-type:access_token                 |
	| scope               | UniversityCredential                                          |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='no_active_otp'
	And JSON '$.error_description'='the user doesn't have an active OTP'