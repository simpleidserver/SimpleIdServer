Feature: RegisterErrors
	Check errors returned during client registration

Scenario: Error is returned when trying to get a client and authorization header is missing
	When execute HTTP GET request 'https://localhost:8080/register/clientid'
	| Key | Value |
	
	Then HTTP status code equals to '401'

Scenario: Error is returned when authorization header is missing
	When execute HTTP GET request 'https://localhost:8080/register/clientid'
	| Key | Value |
	
	Then HTTP status code equals to '401'

Scenario: Error is returned when access token is invalid
	When execute HTTP GET request 'https://localhost:8080/register/clientid'
	| Key           | Value       |
	| Authorization | accesstoken |
	
	Then HTTP status code equals to '401'

Scenario: application_type must be correct
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key				| Value		              |
	| Authorization     | Bearer $access_token$   |	
	| application_type	| unknown	              |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='application type is invalid'

Scenario: sectore_identifier_uri must be correct
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key					| Value		              |
	| sector_identifier_uri	| unknown	              |
	| Authorization         | Bearer $access_token$   |	

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='sector_identifier_uri is not a valid URI'

Scenario: sectore_identifier_uri must contain HTTPS scheme
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key					| Value				      |
	| sector_identifier_uri	| http://localhost	      |
	| Authorization         | Bearer $access_token$   |	

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='sector_identifier_uri doesn't contain https scheme'

Scenario: initiate_login_uri must be correct
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                   | Value                   |
	| initiate_login_uri    | unknown                 |
	| Authorization         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='initiate_login_uri is not a valid URI'
		
Scenario: initiate_login_uri must contains HTTPS scheme
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                   | Value                   |
	| initiate_login_uri    | http://localhost        |
	| Authorization         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='initiate_login_uri doesn't contain https scheme'

Scenario: subject_type must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key			        | Value	                  |
	| subject_type	        | unknow                  |
	| Authorization         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='subject_type is invalid'

Scenario: id_token_signed_response_alg must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key									| Value	                  |
	| id_token_signed_response_alg			| unknow                  |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='id_token_signed_response_alg is not supported'

Scenario: id_token_encrypted_response_alg must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key								    | Value	                  |
	| id_token_encrypted_response_alg	    | unknown                 |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='id_token_encrypted_response_alg is not supported'

Scenario: id_token_encrypted_response_enc must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key								    | Value		              |
	| id_token_encrypted_response_enc	    | unknown	              |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='id_token_encrypted_response_enc is not supported'

Scenario: id_token_encrypted_response_alg is required when id_token_encrypted_response_enc is specified
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key							        | Value			          |
	| id_token_encrypted_response_enc       | A128CBC-HS256	          |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='missing parameter id_token_encrypted_response_alg'

Scenario: userinfo_signed_response_alg must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key							        | Value		              |
	| userinfo_signed_response_alg	        | unknown	              |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='userinfo_signed_response_alg is not supported'

Scenario: userinfo_encrypted_response_alg must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key								    | Value		              |
	| userinfo_encrypted_response_alg	    | unknown	              |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='userinfo_encrypted_response_alg is not supported'

Scenario: userinfo_encrypted_response_enc must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key								    | Value		              |
	| userinfo_encrypted_response_enc	    | unknown	              |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='userinfo_encrypted_response_enc is not supported'

Scenario: userinfo_encrypted_response_alg is required when userinfo_encrypted_response_enc is specified
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key								    | Value			          |
	| userinfo_encrypted_response_enc	    | A128CBC-HS256	          |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='missing parameter userinfo_encrypted_response_alg'	


Scenario: request_object_signing_alg must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key						            | Value	                  |
	| request_object_signing_alg            | unknown	              |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='request_object_signing_alg is not supported'

Scenario: request_object_encryption_alg must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key							        | Value		              |
	| request_object_encryption_alg	        | unknown	              |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='request_object_encryption_alg is not supported'


Scenario: request_object_encryption_enc must be supported
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key							        | Value		              |
	| request_object_encryption_enc	        | unknown	              |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='request_object_encryption_enc is not supported'


Scenario: request_object_encryption_alg is required when request_object_encryption_enc is specified
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key							        | Value			          |
	| request_object_encryption_enc	        | A128CBC-HS256           |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_client_metadata'
	Then JSON 'error_description'='missing parameter request_object_encryption_alg'

Scenario: web client must have a valid redirect_uri
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key				                    | Value		              |
	| redirect_uris		                    | ["invalid"]             |
	| application_type	                    | web		              |
	| Authorization                         | Bearer $access_token$   |

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='redirect_uri invalid is not correct'

Scenario: redirect_uri cannot contains fragment
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key                                   | Value                       |
	| redirect_uris                         | ["http://localhost#foobar"] |
	| application_type                      | web                         |
	| Authorization                         | Bearer $access_token$       |

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='the redirect_uri cannot contains fragment'

Scenario: native client must have a valid redirect_uri
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key										| Value		                |
	| redirect_uris								| ["invalid"]               |
	| application_type							| native	                |
	| Authorization                             | Bearer $access_token$     |

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='redirect_uri invalid is not correct'

Scenario: redirect_uri must have https scheme
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key			                            | Value				        |
	| redirect_uris	                            | ["http://localhost"]      |
	| Authorization                             | Bearer $access_token$     |

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='redirect_uri does not contain https scheme'

Scenario: web client cannot have redirect_uri pointing to localhost
	When execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | fiftySevenClient   |
	| client_secret | password           |
	| scope         | register           |
	| grant_type    | client_credentials |	
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body	

	And execute HTTP POST JSON request 'http://localhost/register'
	| Key			| Value				        |
	| redirect_uris	| ["https://localhost"]     |
	| Authorization | Bearer $access_token$     |

	And extract JSON from body

	Then JSON 'error'='invalid_redirect_uri'
	Then JSON 'error_description'='redirect_uri must not contain localhost'