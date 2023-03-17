openssl genrsa -out sidClient.key 2048

openssl req -new -key sidClient.key -out sidClient.csr

openssl x509 -req -in sidClient.csr -CA simpleIdServer.pem -CAkey simpleIdServer.key -CAcreateserial -out sidClient.crt -days 1024 -sha256 -extfile sidClient.ext