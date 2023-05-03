# Security

You need an access token to execute operations on the APIS.

By default, there is one client configured.
An access token can be retrieved by executing the following HTTP POST REQUEST :

```
HTTP POST 
Target https://localhost:5001/master/token

Content-Type: x-www-form-urlencoded

client_id=managementClient
client_secret=password
scope=users
grant_type=client_credentials
```