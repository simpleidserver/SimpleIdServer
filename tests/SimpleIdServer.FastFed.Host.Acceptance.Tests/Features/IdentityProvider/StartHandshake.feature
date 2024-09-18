Feature: StartHandshake
	Check result returned during the start handshake

Scenario: start handshake and check confirmation view is returned
	When execute HTTP GET request 'http://localhost/fastfed/start'
	| Key              | Value                                        |
	| expiration       | 2                                            |
	| app_metadata_uri | http://localhost/fastfed/provider-metadata   |

	And extract return url

	Then redirect uri equals to 'http://localhost/FastFedDiscovery/Confirm/http%3A%2F%2Flocalhost'