# Security profile

The FAPI Security profile defines the security prerequisites for API resources that are safeguarded by the OAuth 2.0 Authorization request.

There are two security profiles available. The Financial-grade Security Profile 1.0 baseline is appropriate for safeguarding APIs with a moderate inherent risk. However, if a higher level of security is desired, it is recommended to utilize the Financial-grade API Security Profile 1.0 - Part 2: advanced.

We establish the default configuration for each type of client and security profile.

**Public client**

| Profile  | PKCE     | Code Challenge Method |
| -------- | -------- | --------------------- |
| Baseline | REQUIRED | S256                  |

Public clients are not supported when utilizing the advanced security profile.

**Confidential client**

| Profile  | Client Authentication methods                    | Identity Token Signed Response Alg | Request Object Signing Alg | Authorization Signed Response Alg |
| -------- | ------------------------------------------------ | ---------------------------------- | -------------------------- | --------------------------------- |
| Baseline | Mutual TLS, client_secret_jwt or private_key_jwt | ES256                              | Not required               | Not required                      |
| Advanced | Mutual TLS, client_secret_jwt or private_key_jwt | ES256                              | ES256                      | ES256                             |

The latest version of the FAPI Security Profile, version 2.0, has a wider scope compared to FAPI 1.0. It is not only focused on financial applications but is designed to be universally applicable for protected APIs that expose high-value and sensitive data. This includes applications such as e-health and e-government applications.

Main Differences to FAPI 1.0

| FAPI 1.0                             | FAPI 2.0              |
| ------------------------------------ | --------------------- |
| JAR                                  | PAR                   |
| JARM                                 | Only code in response |
| response types code id_token or code | response type code    |