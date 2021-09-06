OAuthHostOptions
================

SimpleIdServer OAUTH2.0 framework takes its configuration from the ``OAuthHostOptions`` option class. 
The properties can be changed in ``Startup`` class like this::

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddSIDOAuth(o =>
		{
			o.ClientSecretExpirationInSeconds = 2;
		})
	}

The OAuthHostOptions class contains the following properties:

``DefaultScopes``
    Default scopes assigned to OAUTH2.0 client. 

``ClientSecretExpirationInSeconds``
    Default expiration time in seconds of a client secret.

``DefaultTokenProfile``
    Default token profile assigned to a OAUTH2.0 client, possible values are :
	
	- mac
	
	- bearer

``SoftwareStatementTrustedParties``
    The property is used by the authorization server to check the ``software_statement`` parameter. The validation process is made of two steps :
	
	- Authorization server fetches the ``iss`` parameter from the JWS header and get the corresponding JWKS URL from the ``SoftwareStatementTrustedParties`` property.
	
	- Authorization server fetches the JSON Web Key (JWK) from the URL and checks the signature of the ``software_statement`` parameter.
	
``DefaultCulture``
    Default culture used by the UI when no ``ui_locales`` parameter is passed in the authorization request.