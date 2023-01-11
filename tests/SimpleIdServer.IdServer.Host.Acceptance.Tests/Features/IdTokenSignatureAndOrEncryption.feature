Feature: IdTokenSignatureAndOrEncryption
	Execute different scenarios to sign and/or encrypt id_token

Scenario: Identity Token must be returned in JWS format with alg set to 'none'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | fifteenClient         |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'none'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWS format with alg set to 'ES256'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | sixteenClient         |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'ES256'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWS format with alg set to 'ES384'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | seventeenClient       |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'ES384'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWS format with alg set to 'ES512'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | eighteenClient        |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'ES512'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWS format with alg set to 'HS256'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | nineteenClient        |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'HS256'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWS format with alg set to 'HS384'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentyClient          |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'HS384'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWS format with alg set to 'HS512'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentyOneClient       |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'HS512'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWS format with alg set to 'RS256'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentyTwoClient       |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'RS256'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWS format with alg set to 'RS384'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentyThreeClient     |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'RS384'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWS format with alg set to 'RS512'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentyFourClient      |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT alg = 'RS512'
	Then JWT contains 'iss'
	Then JWT contains 'iat'
	Then JWT contains 'exp'
	Then JWT contains 'azp'
	Then JWT contains 'aud'
	Then JWT has 'sub'='user'
	Then JWT has 'email'='email@outlook.fr'
	Then JWT has 'role'='role1'
	Then JWT has 'role'='role2'

Scenario: Identity Token must be returned in JWE format with alg set to 'RSA1_5' and enc set to 'A128CBC-HS256'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentyFiveClient      |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT is encrypted
	Then JWT alg = 'RSA1_5'
	Then JWT enc = 'A128CBC-HS256'

Scenario: Identity Token must be returned in JWE format with alg set to 'RSA1_5' and enc set to 'A192CBC-HS384'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentySixClient       |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT is encrypted
	Then JWT alg = 'RSA1_5'
	Then JWT enc = 'A192CBC-HS384'

Scenario: Identity Token must be returned in JWE format with alg set to 'RSA1_5' and enc set to 'A256CBC-HS512'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentySevenClient     |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT is encrypted
	Then JWT alg = 'RSA1_5'
	Then JWT enc = 'A256CBC-HS512'

Scenario: Identity Token must be returned in JWE format with alg set to 'RSA-OAEP' and enc set to 'A128CBC-HS256'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentyEightClient     |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT is encrypted
	Then JWT alg = 'RSA-OAEP'
	Then JWT enc = 'A128CBC-HS256'

Scenario: Identity Token must be returned in JWE format with alg set to 'RSA-OAEP' and enc set to 'A192CBC-HS384'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | twentyNineClient      |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT is encrypted
	Then JWT alg = 'RSA-OAEP'
	Then JWT enc = 'A192CBC-HS384'

Scenario: Identity Token must be returned in JWE format with alg set to 'RSA-OAEP' and enc set to 'A256CBC-HS512'
	Given authenticate a user
	When execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value                 |
	| response_type | id_token              |
	| client_id     | thirtyClient          |
	| state         | state                 |
	| response_mode | query                 |
	| scope         | openid email role     |
	| redirect_uri  | http://localhost:8080 |
	| nonce         | nonce                 |
	| display       | popup                 |
	
	And extract parameter 'id_token' from redirect url
	And extract payload from JWT '$id_token$'

	Then JWT is encrypted
	Then JWT alg = 'RSA-OAEP'
	Then JWT enc = 'A256CBC-HS512'