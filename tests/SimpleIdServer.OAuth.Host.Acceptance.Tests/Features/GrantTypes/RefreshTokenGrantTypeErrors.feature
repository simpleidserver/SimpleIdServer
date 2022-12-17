Feature: RefreshTokenGrantTypeErrors
	Check errors returned when using 'refresh_token' grant-type

Scenario: Send 'grant_type=refresh_token' with no refresh_token parameter
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value         |
	| grant_type | refresh_token |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter refresh_token'

Scenario: Send 'grant_type=refresh_token,refresh_token=rt' with no client_id
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value         |
	| grant_type    | refresh_token |
	| refresh_token | rt            |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='missing client_id'	
	
Scenario: Send 'grant_type=refresh_token,refresh_token=rt' with invalid client_id
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value         |
	| grant_type    | refresh_token |
	| refresh_token | rt            |
	| client_id     | c             |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='unknown client c'
	
Scenario: Send 'grant_type=refresh_token,refresh_token=rt,client_id=firstClient' with no client_secret
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value         |
	| grant_type    | refresh_token |
	| refresh_token | rt            |
	| client_id     | firstClient   |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client credential'
	
Scenario: Send 'grant_type=refresh_token,refresh_token=rt,client_id=firstClient,client_secret=bad' with bad client_secret
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value         |
	| grant_type    | refresh_token |
	| refresh_token | rt            |
	| client_id     | firstClient   |
	| client_secret | bad           |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client credential'
	
Scenario: Send 'grant_type=refresh_token,refresh_token=rt,client_id=firstClient,client_secret=password' with unauthorized grant_type
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value         |
	| grant_type    | refresh_token |
	| refresh_token | rt            |
	| client_id     | firstClient   |
	| client_secret | password      |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='grant type refresh_token is not supported by the client'
	
Scenario: Send 'grant_type=refresh_token,refresh_token=rt,client_id=fifthClient,client_secret=password' with missing refresh_token
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value         |
	| grant_type    | refresh_token |
	| refresh_token | rt            |
	| client_id     | fifthClient   |
	| client_secret | password      |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='bad refresh token'	
	
Scenario: Send 'grant_type=refresh_token,refresh_token=$refreshtoken$,client_id=fifthClient,client_secret=password' with expired refresh_token
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| grant_type    | client_credentials |
	| scope         | secondScope        |
	| client_id     | fifthClient        |
	| client_secret | password           |

	And extract JSON from body
	And extract parameter '$.refresh_token' from JSON body into 'refreshToken'
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value          |
	| grant_type    | refresh_token  |
	| refresh_token | $refreshToken$ |
	| client_id     | fifthClient    |
	| client_secret | password       |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='refresh token is expired'