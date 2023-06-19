# Level of assurance

The Requested Authentication Context Class Reference (ACR) is a parameter used in authentication protocols, such as OpenID Connect, to specify the level or type of authentication required by the client. 
It helps in defining the authentication requirements for a particular request.

When a client initiates an authentication request, it can include the ACR parameter to indicate the desired level of assurance or specific authentication method. 
The value of the ACR parameter can be a predefined string or a custom value agreed upon by the client and the server.

The server receiving the authentication request can then evaluate the ACR value and determine how to fulfill the requested authentication requirements. 
It may prompt the user for specific authentication factors, such as a username and password, multi-factor authentication, or any other mechanism supported by the server.

By utilizing the Requested ACR, clients can communicate their authentication preferences to the server, allowing for flexible and tailored authentication experiences based on the desired security level or specific requirements of the client application.

### Api Call

The `acr_values` parameter is defined by the OpenID Specification and serves to indicate the desired level of authentication requested by the client application.

If there is an ACR named `sid-2` configured with authentication methods such as pwd (password) and email, you can proceed by executing the following request :

```
GET /authorization?
   response_type=code&
   &client_id=your_client_id
   &redirect_uri=https://your-app.com/callback
   &scope=openid profile
   &state=1234567890
   &acr_values=sid-2
```

First, the user-agent will be redirected to the login page where the user will enter their login credentials (username and password). Upon successful submission, the user-agent will be further redirected to the email page, where the user will input the OTP (One-Time Password) code.

The identity token includes the acr claim, which signifies that the user has successfully attained the requested level of authentication.