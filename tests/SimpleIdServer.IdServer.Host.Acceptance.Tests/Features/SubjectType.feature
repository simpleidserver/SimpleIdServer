Feature: SubjectType
	Check the different subject type

Scenario: Get pairwise subject
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | thirtyThreeClient     |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'
	
	Then JWT has 'sub'='ayv4P9i7vUdFDHPXKEY21d2zBHryA4k4PEO80sh4AiQ'