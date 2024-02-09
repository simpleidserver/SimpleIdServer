Feature: HappyFlow
	Check the different client authentication methods

Scenario: Use client_secret_basic authentication method
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key                   | Value                                      |
	| grant_type            | client_credentials                         |
	| scope                 | firstScope                                 |
	| client_id				| seventyOneClient                           |
	| Authorization			| Basic c2V2ZW50eU9uZUNsaWVudDpwYXNzd29yZA== |

	And extract JSON from body
	Then HTTP status code equals to '200'