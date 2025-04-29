# NGINX

[NGINX](https://nginx.org/) is a robust, high-performance web server and reverse proxy that is widely used to manage high-traffic websites and applications. Known for its scalability and efficient handling of concurrent connections, NGINX is an ideal choice for deploying identity servers behind a secure and reliable proxy layer.

## Ensuring Proper HTTP Header Forwarding

For an identity server to operate correctly when behind NGINX, it is critical that certain HTTP headers are passed along to the identity server. In particular:

* **X-Forwarded-Proto**: This header identifies whether the client connected using HTTP or HTTPS. Its presence is essential because, without it, the URLs returned by the `/.well-known/openid-configuration` endpoint might not use the https schema, potentially leading to authentication issues.

* **X-Forwarded-For**: This header carries the original client IP address. Maintaining this information is important for monitoring and security purposes.

To handle these headers seamlessly, the identity server’s API includes a function called `ForwardHttpHeader` in the fluent API.
This function ensures that the required HTTP headers are forwarded properly, maintaining the integrity of the client’s request information.

## Meeting FAPI 2.0 Security Requirements

When operating under the security standards mandated by FAPI 2.0, additional precautions are necessary. 
The identity server must enable the FAPI security profile by invoking the `EnableFapiSecurityProfile` method available in the fluent API. 
This action internally triggers the `AddCertificateForwarding` function, which extracts client certificates (specifically X509Certificate2 objects) from the HTTP header `ssl-client-cert`.

As a consequence, your NGINX configuration must be updated to forward this client certificate information. 
This ensures that the identity server can perform the necessary certificate validations and maintain a secure authentication process.

## C# Implementation Example

Below is an example of how you might configure your identity server in C# to comply with FAPI 2.0 standards and work seamlessly behind an NGINX reverse proxy:

```csharp title="Program.cs"
var users = new List<User>
{
    UserBuilder.Create("administrator", "password", "Administrator")
               .SetEmail("adm@mail.com")
               .SetFirstname("Administrator")
               .Build()
};

var builder = WebApplication.CreateBuilder(args);
builder.AddSidIdentityServer()
       .AddDeveloperSigningCredential()
       .AddInMemoryUsers(users)
       .AddInMemoryLanguages(DefaultLanguages.All)
       .AddPwdAuthentication(true)
       .EnableFapiSecurityProfile() // Enable FAPI security profile.
       .ForwardHttpHeader();        // Forward HTTP headers to the identity server.

var app = builder.Build();
app.Services.SeedData();
app.UseSid();
app.Run();
```

This code demonstrates the setup of an identity server that complies with FAPI 2.0 security standards. The fluent API is used to both forward HTTP headers and enable certificate forwarding, ensuring the proper handling of client authentication and secure communications.

## Updating Your NGINX Configuration

Once your identity server is deployed, it is crucial to adjust your NGINX configuration to work with the server’s requirements. A sample configuration might look like the following:

```json title="nginx.conf"
server {
    listen 443 ssl;
    listen [::]:443 ssl;

    gzip on;
    gzip_types text/plain text/css application/xml application/json application/javascript;

    root /var/www/html;
    index index.html index.htm index.nginx-debian.html;

    large_client_header_buffers 4 32k;

    server_name simpleidserver;

    ssl_certificate /etc/letsencrypt/live/simpleidserver/fullchain.pem; // SSL certificate.
    ssl_certificate_key /etc/letsencrypt/live/simpleidserver/privkey.pem;
    ssl_verify_client optional_no_ca;

    location /openid {
        proxy_pass https://localhost:5001;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_pass_header Set-Cookie;
        proxy_pass_request_headers on;
        proxy_cache_bypass $http_upgrade;
        proxy_cookie_domain localhost $host;
        proxy_set_header X-Scheme https;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme; // Forward the HTTP scheme.
        proxy_set_header X-URL-SCHEME https;
        proxy_set_header ssl-client-cert $ssl_client_escaped_cert; // Forward the client certificate.
        client_max_body_size 1M;
        client_body_buffer_size 4096k;
        proxy_connect_timeout 90;
        proxy_send_timeout 90;
        proxy_read_timeout 90;
        proxy_buffer_size 128k;
        proxy_buffers 32 256k;
    }
}
```

This configuration ensures that:

* SSL is correctly set up to secure incoming HTTP connections.
* The HTTP scheme and client certificate information are forwarded to the identity server.
* Proper proxy settings are applied to maintain a seamless and secure connection between NGINX and the identity server.

## Enable web-socket

To enable WebSocket support in NGINX, the following directives must be added:

```json
proxy_set_header Upgrade $http_upgrade;
proxy_set_header Connection "Upgrade";
proxy_http_version 1.1;
```

In the end, the configuration file will look something like this:

```json title="nginx.conf"
server {
    listen 443 ssl;
    listen [::]:443 ssl;

    gzip on;
    gzip_types text/plain text/css application/xml application/json application/javascript;

    root /var/www/html;
    index index.html index.htm index.nginx-debian.html;

    large_client_header_buffers 4 32k;

    server_name simpleidserver;

    ssl_certificate /etc/letsencrypt/live/simpleidserver/fullchain.pem; // SSL certificate.
    ssl_certificate_key /etc/letsencrypt/live/simpleidserver/privkey.pem;
    ssl_verify_client optional_no_ca;

    location /openid {
        proxy_pass https://localhost:5001;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "Upgrade"; // Enable web-socket
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_pass_header Set-Cookie;
        proxy_pass_request_headers on;
        proxy_cache_bypass $http_upgrade;
        proxy_cookie_domain localhost $host;
        proxy_set_header X-Scheme https;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme; // Forward the HTTP scheme.
        proxy_set_header X-URL-SCHEME https;
        proxy_set_header ssl-client-cert $ssl_client_escaped_cert; // Forward the client certificate.
        client_max_body_size 1M;
        client_body_buffer_size 4096k;
        proxy_connect_timeout 90;
        proxy_send_timeout 90;
        proxy_read_timeout 90;
        proxy_buffer_size 128k;
        proxy_buffers 32 256k;
    }
}
```

## Sticky Sessions for Load Balancing

If your NGINX server is configured as a load balancer and you require that a user’s session consistently connects to the same backend server, you can enable sticky sessions. This feature, also known as session persistence, ensures that once a user is routed to a particular identity server instance, subsequent requests will be directed to the same instance. For more detailed information on configuring sticky sessions, please refer to the official NGINX documentation under the section [Enabling Session Persistence](https://docs.nginx.com/nginx/admin-guide/load-balancer/http-load-balancer/) at NGINX Load Balancer Documentation.

For more detailed reference, you can check out the example project available on GitHub: [SimpleIdServer Sample](https://github.com/simpleidserver/SimpleIdServer/tree/master/samples/IdserverNginx).