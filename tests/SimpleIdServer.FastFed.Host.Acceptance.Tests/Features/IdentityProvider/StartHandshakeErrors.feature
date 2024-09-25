Feature: StartHandshakeErrors
	Check errors during the start handshake

Scenario: the parameter app_metadata_uri is required
	When execute HTTP GET request 'http://localhost/fastfed/start'
	| Key        | Value |
	| expiration | 2     |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Parameter app_metadata_uri is missing'

Scenario: the parameter app_metadata_uri must be valid
	When execute HTTP GET request 'http://localhost/fastfed/start'
	| Key              | Value |
	| expiration       | 2     |
	| app_metadata_uri | url   |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Provider metadata cannot be retrieved'

Scenario: the application provider must exists
	When execute HTTP GET request 'http://localhost/fastfed/start'
	| Key              | Value                                    |
	| expiration       | 2                                        |
	| app_metadata_uri | http://localhost/bad/provider-metadata   |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Application provider metadata cannot be retrieved'


Scenario: the application provider capabilities must be compatible
	When execute HTTP GET request 'http://localhost/fastfed/start'
	| Key              | Value                                        |
	| expiration       | 2                                            |
	| app_metadata_uri | http://localhost/bad/app-provider-metadata   |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON '$.error_code'='invalid_request'
	Then JSON '$.error_descriptions[0]'='Provisioning profile invalid:provisioning is not compatible'
	Then JSON '$.error_descriptions[1]'='Schema grammar invalid:schemagrammar is not compatible'
	Then JSON '$.error_descriptions[2]'='Signing algorithm invalid-sigalg is not compatible'