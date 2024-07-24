Feature: AutomaticRegistration
	Check automatic registration

Scenario: Check access token can be returned when using automatic client registration (authorization request)
	Given authenticate a user

	And build JWS request object for Relying Party
	| Key           | Value                               |
	| redirect_uri  | https://openid.sunet.se/rp/callback |
	| response_type | code                                |
	| scope         | openid profile                      |
	| client_id     | http://rp.com                       |
	| state         | state								  |
	
	And build client assertion for Relying Party
	| Key  | Value                        |
	| iss  | http://rp.com                |
	| sub  | http://rp.com                |
	| aud  | https://localhost:8080/token |

	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                        |
	| client_id     | http://rp.com                |
	| request       | $request$                    |
	
	And extract parameter 'code' from redirect url
	And extract parameter 'state' from redirect url

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value        			                                 |
	| client_id             | http://rp.com			                                 |
	| grant_type            | authorization_code	                                 |
	| code			        | $code$				                                 |	
	| redirect_uri          | https://openid.sunet.se/rp/callback                    |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertion$                                      |
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.token_type'='Bearer'

Scenario: Check access token can be returned when using automatic client registration (pushed authorization request)
	Given authenticate a user

	And build JWS request object for Relying Party
	| Key           | Value                               |
	| redirect_uri  | https://openid.sunet.se/rp/callback |
	| response_type | code                                |
	| scope         | openid profile                      |
	| client_id     | http://rp.com                       |
	| state         | state								  |
	
	And build client assertion for Relying Party
	| Key  | Value                        |
	| iss  | http://rp.com                |
	| sub  | http://rp.com                |
	| aud  | https://localhost:8080/token |

	When execute HTTP POST request 'https://localhost:8080/par'
	| Key           | Value                 |
	| client_id     | http://rp.com         |
	| request       | $request$             |	

	And extract JSON from body
	And extract parameter 'request_uri' from JSON body

	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value              |
	| client_id     | http://rp.com      |
	| request_uri   | $request_uri$      |
	
	And extract parameter 'code' from redirect url
	And extract parameter 'state' from redirect url

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value        			                                 |
	| client_id             | http://rp.com			                                 |
	| grant_type            | authorization_code	                                 |
	| code			        | $code$				                                 |	
	| redirect_uri          | https://openid.sunet.se/rp/callback                    |
	| client_assertion_type | urn:ietf:params:oauth:client-assertion-type:jwt-bearer |
	| client_assertion      | $clientAssertion$                                      |
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.token_type'='Bearer'