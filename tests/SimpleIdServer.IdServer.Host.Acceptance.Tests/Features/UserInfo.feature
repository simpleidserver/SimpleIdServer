Feature: UserInfo
	Check the userinfo endpoint

Scenario: Claims are returned in JSON format (HTTP GET)
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                            |
	| response_type | code                             |
	| client_id     | thirtySevenClient                |
	| state         | state                            |
	| response_mode | query                            |
	| scope         | openid email role                |
	| redirect_uri  | http://localhost:8080            |
	| nonce         | nonce                            |
	| display       | popup                            |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirtySevenClient     |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|	

	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then HTTP header has 'Content-Type'='application/json'
	Then JSON 'sub'='user'
	Then JSON '$.role[0]'='role2'
	Then JSON '$.role[1]'='role1'
	Then JSON 'email'='email@outlook.fr'

Scenario: Claims are returned in JSON format (HTTP POST)
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                            |
	| response_type | code                             |
	| client_id     | thirtySevenClient                |
	| state         | state                            |
	| response_mode | query                            |
	| scope         | openid email role                |
	| redirect_uri  | http://localhost:8080            |
	| nonce         | nonce                            |
	| display       | popup                            |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirtySevenClient     |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|	

	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP POST request 'http://localhost/userinfo'
	| Key          | Value          |
	| access_token | $access_token$ |

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then HTTP header has 'Content-Type'='application/json'
	Then JSON 'sub'='user'
	Then JSON '$.role[0]'='role2'
	Then JSON '$.role[1]'='role1'
	Then JSON 'email'='email@outlook.fr'

Scenario: Claims are returned in JWS token
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                            |
	| response_type | code                             |
	| client_id     | thirtyEightClient                |
	| state         | state                            |
	| response_mode | query                            |
	| scope         | openid email role                |
	| redirect_uri  | http://localhost:8080            |
	| nonce         | nonce                            |
	| display       | popup                            |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirtyEightClient     |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|	

	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	And extract payload from HTTP body
	
	Then HTTP status code equals to '200'
	Then HTTP header has 'Content-Type'='application/jwt'
	Then JWT alg = 'RS256'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Claims are returned in JWE token
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                            |
	| response_type | code                             |
	| client_id     | thirtyNineClient                 |
	| state         | state                            |
	| response_mode | query                            |
	| scope         | openid email role                |
	| redirect_uri  | http://localhost:8080            |
	| nonce         | nonce                            |
	| display       | popup                            |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirtyNineClient      |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|	

	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |

	And extract payload from HTTP body
	
	Then HTTP status code equals to '200'
	Then HTTP header has 'Content-Type'='application/jwt'
	Then JWT is encrypted
	Then JWT alg = 'RSA1_5'
	Then JWT enc = 'A128CBC-HS256'

Scenario: Essential claims 'name' and 'email' are returned by the userinfo endpoint
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                                                                                  |
	| response_type | code                                                                                   |
	| client_id     | fortyClient                                                                            |
	| state         | state                                                                                  |
	| response_mode | query                                                                                  |
	| scope         | openid                                                                                 |
	| redirect_uri  | http://localhost:8080                                                                  |
	| nonce         | nonce                                                                                  |
	| display       | popup                                                                                  |
	| claims        | { "userinfo" : { "name":  { "essential": true } , "email" : { "essential" : true } } } |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | fortyClient           |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|

	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key           | Value                 |
	| Authorization | Bearer $access_token$ |	
	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then HTTP header has 'Content-Type'='application/json'
	Then JSON 'sub'='user'
	Then JSON 'email'='email@outlook.fr'

