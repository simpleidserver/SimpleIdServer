Feature: ImplicitGrantType
	Check all the alternatives scenarios in implicit grant-type

Scenario: No access token is issued then resulting claims are returned in the ID Token
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | fourteenClient        |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'
	
	Then redirection url doesn't contain the parameter 'access_token'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Display parameter is passed in the redirection url
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | fourteenClient        |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	Then redirection url contains the parameter value 'display'='popup'