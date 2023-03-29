Feature: AlternativeScenarios
	Execute alternative scenarios

Scenario: User-agent is redirected to the login page when elapsed time > authentication time + default client max age
	Given authenticate a user and add '-10' seconds to the authentication time
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | thirtyFourClient      |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	Then redirection url contains 'http://localhost/pwd/Authenticate'

Scenario: User-agent is redirected to the login page when elapsed time > authentication time + max age
	Given authenticate a user and add '-10' seconds to the authentication time
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | thirtyFiveClient      |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	| max_age       | 2                     |
	
	Then redirection url contains 'http://localhost/pwd/Authenticate'

Scenario: auth_time claim is returned in the identity token when it is marked as essential claim
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                                                   |
	| response_type | id_token                                                |
	| client_id     | thirtyFiveClient                                        |
	| state         | state                                                   |
	| response_mode | query                                                   |
	| scope         | openid email role                                       |
	| redirect_uri  | http://localhost:8080                                   |
	| nonce         | nonce                                                   |
	| display       | popup                                                   |
	| claims        | { "id_token": { "auth_time": { "essential" : true } } } |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'	

	Then JWT contains 'auth_time'

Scenario: amr and acr are returned in the identity token when acr_values is passed
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                  |
	| response_type | id_token               |
	| client_id     | thirtyFiveClient       |
	| state         | state                  |
	| response_mode | query                  |
	| scope         | openid email role      |
	| redirect_uri  | http://localhost:8080  |
	| nonce         | nonce                  |
	| display       | popup                  |
	| acr_values    | sid-load-01            |
		
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'	

	Then JWT contains 'amr'
	Then JWT contains 'acr'
	Then JWT has 'amr'='pwd'
	Then JWT has 'acr'='sid-load-01'

Scenario: when using offline_scope the client can access to the userinfo even if the end-user is not authenticated
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                            |
	| response_type | code                             |
	| client_id     | thirtySixClient                  |
	| state         | state                            |
	| response_mode | query                            |
	| scope         | openid email role offline_access |
	| redirect_uri  | http://localhost:8080            |
	| nonce         | nonce                            |
	| display       | popup                            |
	
	And extract parameter 'refresh_token' from redirect url
	And disconnect the user
	
	And execute HTTP POST request 'http://localhost/token'
	| Key           | Value            |
	| grant_type    | refresh_token    |
	| refresh_token | $refresh_token$  |
	| client_id     | thirtySixClient  |
	| client_secret | password         |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON 'sub'='user'
	Then JSON '$.role[0]'='role1'
	Then JSON '$.role[1]'='role2'
	Then JSON 'email'='email@outlook.fr'