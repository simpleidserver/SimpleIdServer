Feature: Authorization
	Check the authorization endpoint

Scenario: Check a refresh token is returned when the scope offline_access is used
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key            | Value                         |
	| redirect_uris  | [https://web.com]             |
	| grant_types    | [authorization_code]			 |
	| response_types | [code]					     |
	| scope          | email offline_access 		 |
	| subject_type   | public	                     |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='email offline_access', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                       |
	| response_type | code                        |
	| client_id     | $client_id$                 |
	| state         | state                       |
	| response_mode | query                       |
	| scope         | openid offline_access email |
	| redirect_uri  | https://web.com             |
	| ui_locales    | en fr                       |

	Then redirect url contains 'code'
	Then redirect url contains 'refresh_token'