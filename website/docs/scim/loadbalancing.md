# Load balancing

One notable advantage of a SCIM 2.0 server is that, unlike an administration site or an identity server, there are no extra prerequisites to operate through a load balancer. Thanks to the stateless nature of all incoming HTTP requests, there's no need to set up a distributed cache or persist cryptographic keys.

Cryptographic keys are generally used to encrypt and decrypt cookies. However, because a SCIM 2.0 server does not rely on such session management mechanisms, load balancing can be implemented more straightforwardly. This simplifies infrastructure design and can result in a more scalable and easily maintainable system.