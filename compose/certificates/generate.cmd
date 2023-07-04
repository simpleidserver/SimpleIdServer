mkcert -cert-file localhost.com.crt -key-file localhost.com.key localhost.com *.localhost.com
mkcert -pkcs12 localhost.com.pfx localhost.com *.localhost.com
openssl pkcs12 -inkey localhost.com.key -in localhost.com.crt -export -out localhost.com.pfx

mkcert -cert-file sid.crt -key-file sid.key sid.svc.cluster.local *.sid.svc.cluster.local
mkcert -pkcs12 sid.pfx sid.svc.cluster.local *.sid.svc.cluster.local
openssl pkcs12 -inkey sid.key -in sid.crt -export -out sid.pfx