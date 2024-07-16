Feature: AutomaticRegistration
	Check automatic registration

Scenario: Check redirect url is returned when automatic registration is used
	Given authenticate a user
	And build JWS request object for Relying Party
	| Key           | Value                               |
	| redirect_uri  | https://openid.sunet.se/rp/callback |
	| response_type | code                                |
	| scope         | openid profile                      |
	| client_id     | http://rp.com                       |

	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                        |
	| client_id     | http://rp.com                |
	| request       | $request$                    |