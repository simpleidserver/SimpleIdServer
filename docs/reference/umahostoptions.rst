UMAHostOptions
==============

UMA options inherits properties from OAUTH2.0 options.

``SignInScheme``
    Default sign in scheme. Default value is **SimpleIdServerUMA**.

``CookieName``
    Default challenge authentication scheme. Default value is **SimpleIdServerUMA.Challenge**.

``RequestSubmittedInterval``
    The minimum amount of time in seconds that the client SHOULD wait between polling requests to the token endpoint. 

``DefaultClaimTokenFormat``
    Default token claim format. Default value is **http://openid.net/specs/openid-connect-core-1_0.html#IDToken**.
	
``ValidityPeriodPermissionTicketInSeconds``
    Validity of permission ticket in seconds. Default value is **300**.

``OpenIdRedirectUrl``
    Claims interation endpoint URI to which to redirect the end-user requesting party at the authorization server. Default value is **https://openid.net**.
	
``OpenIdJsonWebKeySignature``
    JSON Web Key Signature used to check the signature of received claim_token.