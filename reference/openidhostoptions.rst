OpenIDHostOptions
=================

OPENID options inherits properties from OAUTH2.0 options.

``AuthenticationScheme``
    Default authentication scheme. Default value is **.AspNetCore.MultiAccount**.

``CookieName``
    Default cookie name. Default value is **MultiAccount**.

``DefaultAcrValue``
    Default Authentication Context Class Reference (ACR) used by the authorization server if cannot be deduced. Default value is **sid-load-01**.

``DefaultMaxAge``
    Default max age assigned to OPENID client.
	
``DefaultSubjectType``
    Default subject type assigned to an OPENID client. The possible values are : **pairwise** or **public**, the default value is **public**.