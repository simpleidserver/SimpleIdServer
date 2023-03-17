openssl genrsa -aes256 -out simpleIdServer.key 2048

openssl req -x509 -new -nodes -key simpleIdServer.key -sha256 -days 10240 -out simpleIdServer.pem

openssl x509 -outform der -in simpleIdServer.pem -out simpleIdServer.crt