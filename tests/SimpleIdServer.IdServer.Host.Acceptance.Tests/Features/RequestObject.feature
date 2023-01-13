Feature: RequestObject
	Pass request object

Scenario: Identity Token and authorization code are returned when passing JWS request parameter
	Given authenticate a user
	And build JWS request object for client 'thirtyOneClient' and sign with the key 'keyId'
	| Key           | Value                 |
	| iss           | thirtyOneClient       |
	| aud           | aud                   |
	| response_type | code id_token         |
	| client_id     | thirtyOneClient       |
	| response_mode | query                 |
	| scope         | openid email          |
	| nonce         | nonce                 |
	| redirect_uri  | http://localhost:8080 |

	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| request       | $request$         |
	| response_type | code id_token     |
	| client_id     | thirtyOneClient   |
	| state         | state             |
	| scope         | openid email      |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'
	
	Then redirection url contains the parameter 'code'
	Then redirection url contains the parameter 'id_token'
	Then redirection url doesn't contain the parameter 'access_token'
	Then JWT contains 'iss'
	Then JWT contains 'aud'
	Then JWT contains 'exp'
	Then JWT contains 'iat'
	Then JWT contains 'azp'
	Then JWT contains 'c_hash'
	Then JWT has 'sub'='user'

Scenario: Identity Token and authorization code are returned when passing JWE request parameter encrypted with Public Key
	Given authenticate a user
	And build JWE request object for client 'thirtyTwoClient' and sign with the key 'keyId' and encrypt with the key 'keyid4'
	| Key           | Value                 |
	| iss           | thirtyTwoClient       |
	| aud           | aud                   |
	| response_type | code id_token         |
	| client_id     | thirtyTwoClient       |
	| response_mode | query                 |
	| scope         | openid email          |
	| nonce         | nonce                 |
	| redirect_uri  | http://localhost:8080 |

	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value             |
	| request       | $request$         |
	| response_type | code id_token     |
	| client_id     | thirtyTwoClient   |
	| state         | state             |
	| scope         | openid email      |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'
	
	Then redirection url contains the parameter 'code'
	Then redirection url contains the parameter 'id_token'
	Then redirection url doesn't contain the parameter 'access_token'
	Then JWT contains 'iss'
	Then JWT contains 'aud'
	Then JWT contains 'exp'
	Then JWT contains 'iat'
	Then JWT contains 'azp'
	Then JWT contains 'c_hash'
	Then JWT has 'sub'='user'