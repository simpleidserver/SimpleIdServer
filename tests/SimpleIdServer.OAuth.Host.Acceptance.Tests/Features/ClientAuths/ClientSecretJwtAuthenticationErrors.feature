Feature: ClientSecretJwtAuthenticationErrors
	Check errors returned during the 'client_secret_jwt' authentication

Scenario: Error is returned when client_assertion is not a JWE token
	Given build JWS by signing with a random RS256 algorithm and store the result into 'clientAssertion'
	| Key  | Value       |
	| iss  | eightClient |

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_id             | eightClient                                            |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertion$                                      |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='client assertion must be encrypted JWT (JWE)'

Scenario: Error is returned when client_assertion is not encrypted with the correct client_secret
	Given build JWS by signing with a random RS256 algorithm and store the result into 'clientAssertionJws'
	| Key  | Value       |
	| iss  | eightClient |

	And build JWE by encrypting the '$clientAssertionJws$' JWS with the client secret 'ProEMLh5e_qnzdNA' and store the result into 'clientAssertionJwe'	

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_id             | eightClient                                            |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertionJwe$                                   |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='client assertion cannot be decryted by the client secret'

Scenario: Error is returned when json web token is not signed by the correct Json Web Key
	Given build JWS by signing with a random RS256 algorithm and store the result into 'clientAssertionJws'
	| Key  | Value       |
	| iss  | eightClient |

	And build JWE by encrypting the '$clientAssertionJws$' JWS with the client secret 'ProEMLh5e_qnzdNU' and store the result into 'clientAssertionJwe'	

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_id             | eightClient                                            |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertionJwe$                                   |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='client assertion is not signed by a known Json Web Key'

Scenario: Error is returned when iss != sub
	Given build JWS by signing with the key 'eightClientKeyId' coming from the client 'eightClient' and store the result into 'clientAssertionJws'
	| Key  | Value       |
	| iss  | eightClient |
	| sub  | invalid     |

	And build JWE by encrypting the '$clientAssertionJws$' JWS with the client secret 'ProEMLh5e_qnzdNU' and store the result into 'clientAssertionJwe'	

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_id             | eightClient                                            |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertionJwe$                                   |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client assertion issuer'

Scenario: Error is returned when aud is invalid
	Given build JWS by signing with the key 'eightClientKeyId' coming from the client 'eightClient' and store the result into 'clientAssertionJws'
	| Key  | Value       |
	| iss  | eightClient |
	| sub  | eightClient |
	| aud  | invalid     |

	And build JWE by encrypting the '$clientAssertionJws$' JWS with the client secret 'ProEMLh5e_qnzdNU' and store the result into 'clientAssertionJwe'	

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_id             | eightClient                                            |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertionJwe$                                   |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client assertion audiences'	

Scenario: Error is returned when client_assertion is expired
	Given build expired JWS by signing with the key 'eightClientKeyId' coming from the client 'eightClient' and store the result into 'clientAssertionJws'
	| Key  | Value                        |
	| iss  | eightClient                  |
	| sub  | eightClient                  |
	| aud  | https://localhost:8080/token |

	And build JWE by encrypting the '$clientAssertionJws$' JWS with the client secret 'ProEMLh5e_qnzdNU' and store the result into 'clientAssertionJwe'	

	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                                  |
	| grant_type            | client_credentials                                     |
	| scope                 | scope                                                  |
	| client_id             | eightClient                                            |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertionJwe$                                   |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='client assertion is expired'