# Hosting

import DocsCards from '@site/src/components/global/DocsCards';
import DocsCard from '@site/src/components/global/DocsCard';

## NGINX

Once the solution is installed via [Copy and paste](./quickstart.md#copy-and-paste) on your server, you can configure a reverse proxy, such as NGINX, to redirect incoming HTTP traffic to the SimpleIdServer solution.

In a Linux environment, three systemd daemons will be installed, each running a different part of the SimpleIdServer solution.

There are three services hosted on different ports :

| Service  | Port |
| -------- | ---- |
| IdServer | 5001 |
| Website  | 5002 |
| Scim     | 5003 |

They share the same characteristics:
* Hosted under HTTPS.
* They use Forwarded Headers; these HTTP headers are employed to modify the Redirection URL returned by the Discovery endpoint. For example, when the parameter `X-Forwarded-Proto` equals http, the OPENID Well-Known configuration endpoint will return a redirection URL with an http scheme.

You can choose one of the following options to host the solution.

<DocsCards>
    <DocsCard header="Subdomain hosting" href="#subdomain-hosting">
        <p>Each service is hosted on a subdomain.</p>
    </DocsCard>
    <DocsCard header="Subpath hosting" href="#subpath-hosting">
        <p>Each service is hosted on a subpath.</p>
    </DocsCard>
</DocsCards>

### Subdomain hosting

In the NGINX configuration, for each service, replicate the `server` block with the following content.

Each block corresponds to a subdomain and handles one service.

Replace the `<SERVICE_URL>` variable with the URL of your service, and the `<SERVICE_NAME>` variable with the name of your service.

For example, for the IdServer service, replace the variables as follows:

| Parameter    | Value                  |
| ------------ | ---------------------- |
| SERVICE_NAME | openid                 |
| SERVICE_URL  | https://localhost:5001 |
| DOMAIN       | simpleidserver.com     |

``` 
server {
        listen 443 ssl;
        listen [::]:443 ssl;

        gzip on;
        gzip_types text/plain text/css application/xml application/json application/javascript;

        root /var/www/html;

        # Add index.php to the list if you are using PHP
        index index.html index.htm index.nginx-debian.html;

        large_client_header_buffers 4 32k;

        server_name <SERVICE_NAME>.<DOMAIN>;
        ssl_verify_client optional_no_ca;

        location / {
                proxy_pass <SERVICE_URL>;
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
                proxy_set_header X-Forwarded-Proto $scheme;
                proxy_set_header X-URL-SCHEME https;
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

### Subpath hosting

In the NGINX configuration, add one `server` block for each service and replicate the `location` block with the following content.

For example, for the IdServer service, replace the variables as follows:

| Parameter    | Value                  |
| ------------ | ---------------------- |
| PATH         | openid                 |
| SERVICE_URL  | https://localhost:5001 |
| DOMAIN       | simpleidserver.com     |

``` 
server {
        listen 443 ssl;
        listen [::]:443 ssl;

        gzip on;
        gzip_types text/plain text/css application/xml application/json application/javascript;

        root /var/www/html;

        # Add index.php to the list if you are using PHP
        index index.html index.htm index.nginx-debian.html;

        large_client_header_buffers 4 32k;

        server_name <DOMAIN>;
        ssl_verify_client optional_no_ca;

        location /<PATH> {
                proxy_pass <SERVICE_URL>;
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
                proxy_set_header X-Forwarded-Proto $scheme;
                proxy_set_header X-URL-SCHEME https;
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

### SSL Certificate

An SSL Certificate must be installed on your NGINX Server.

You can use [Let's Encrypt](https://letsencrypt.org/) to generate SSL Certificates valid for all your domains and/or subdomains.

For example, the following command line generates a certificate valid for three domains:

```
sudo certbot certonly -d  openid.simpleidserver.com -d scim.simpleidserver.com -d website.simpleidserver.com
```

Once the certificate is generated, you must update the `server` blocks add the `ssl_certificate` and `ssl_certificate_key` directives.

```
ssl_certificate /etc/letsencrypt/live/<DOMAIN>/fullchain.pem;
ssl_certificate_key /etc/letsencrypt/live/<DOMAIN>/privkey.pem;
```

Additionally, add a new server block to redirect all HTTP traffic to HTTPS:

```
server {
        listen 80;
        server_name <DOMAIN>;
        return 301 https://$host$request_uri;
}
```

For more information about NGINX, you can refer to the official website: https://www.nginx.com/blog/using-free-ssltls-certificates-from-lets-encrypt-with-nginx/