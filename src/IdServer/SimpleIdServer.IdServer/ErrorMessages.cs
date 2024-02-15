// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer
{
    public class ErrorMessages
    {
        public const string NO_CLIENT_CERTIFICATE = "no client certificate";
        public const string CERTIFICATE_SUBJECT_INVALID = "certificate subject is invalid";
        public const string CERTIFICATE_SAN_DNS_INVALID = "certificate san DNS is invalid";
        public const string CERTIFICATE_SAN_EMAIL_INVALID = "certificate san EMAIL is invalid";
        public const string CERTIFICATE_SAN_IP_INVALID = "certificate san IP is invalid";
        public const string CERTIFICATE_IS_REQUIRED = "certificate is required";
        public const string CERTIFICATE_IS_NOT_TRUSTED = "certificate is not trusted";
        public const string CERTIFICATE_IS_NOT_SELF_SIGNED = "the certificate is not self signed";
        public const string CERTIFICATE_CANNOT_BE_GENERATED = "the certificate cannot be generated";
        public const string CERTIFICATE_CLIENT_CANNOT_BE_GENERATED = "the client certificate cannot be generated";
        public const string REQUEST_OBJECT_IS_EXPIRED = "request object is expired";
        public const string REQUEST_OBJECT_BAD_AUDIENCE = "request object has bad audience";
        public const string REQUEST_DENIED = "The client is not authorized to have these permissions.";
        public const string REQUESTED_EXPIRY_MUST_BE_POSITIVE = "requested_expiry must be positive";
        public const string ONLY_HYBRID_WORKFLOWS_ARE_SUPPORTED = "only hybrid workflow are supported";
        public const string UNKNOWN_JSON_WEB_KEY = "unknown json web key '{0}'";
        public const string TOO_MANY_AUTH_REQUEST = "you tried too many times to get a token";
        public const string TOO_MANY_DPOP_HEADER = "too many DPoP parameters are passed in the header";
        public const string AUTH_REQUEST_EXPIRED = "the authorization request is expired";
        public const string REDIRECT_URI_CONTAINS_FRAGMENT = "the redirect_uri cannot contains fragment";
        public const string CLIENT_ID_CANNOT_BE_EXTRACTED = "client identifier cannot be extracted from the initial request";
        public const string SCOPE_ALREADY_EXISTS = "scope '{0}' already exists";
        public const string USER_ALREADY_EXISTS = "user '{0}' already exists";
        public const string SIGKEY_ALREADY_EXISTS = "signature key {0} already exists";
        public const string NOT_SAME_REDIRECT_URI = "not the same redirect_uri";
        public const string ONLY_PINGORPOLL_MODE_CAN_BE_USED = "only ping or poll mode can be used to get tokens";
        public const string ONE_HINT_MUST_BE_PASSED = "only one hint can be passed in the request";
        public const string AUTH_REQUEST_CLIENT_NOT_AUTHORIZED = "the client is not authorized to use the auth_req_id";
        public const string AUTH_REQUEST_BAD_AUDIENCE = "the request doesn't contain correct audience";
        public const string AUTH_REQUEST_NO_AUDIENCE = "the request doesn't contain audience";
        public const string AUTH_REQUEST_NO_ISSUER = "the request doesn't contain issuer";
        public const string AUTH_REQUEST_BAD_ISSUER = "the request doesn't contain correct issuer";
        public const string AUTH_REQUEST_NO_EXPIRATION = "the request doesn't contain expiration time";
        public const string AUTH_REQUEST_NO_JTI = "the request doesn't contain jti";
        public const string AUTH_REQUEST_NO_NBF = "the request doesn't contain nbf";
        public const string AUTH_REQUEST_BAD_NBF = "the request cannot be received before '{0}'";
        public const string AUTH_REQUEST_NO_IAT = "the request doesn't contain iat";
        public const string AUTH_REQUEST_MAXIMUM_LIFETIME = "the maximum lifetime of the request is '{0}' seconds";
        public const string AUTH_REQUEST_NOT_CONFIRMED = "the authorization request has not been confirmed";
        public const string AUTH_REQUEST_ALG_NOT_VALID = "the request must be signed with '{0}' algorithm";
        public const string AUTH_REQUEST_IS_EXPIRED = "the request is expired";
        public const string AUTH_REQUEST_REJECTED = "the authorization request has been rejected";
        public const string AUTH_REQUEST_SENT = "the authorization request is finished";
        public const string AUTH_REQUEST_NOT_AUTHORIZED_TO_REJECT = "you're not authorized to reject the authorization request";
        public const string CONTENT_TYPE_NOT_SUPPORTED = "the content-type is not correct";
        public const string JWT_MUST_BE_ENCRYPTED = "JWT must be encrypted with the algorithm {0}";
        public const string JWT_MUST_BE_SIGNED = "JWT must be signed with the algorithm {0}";
        public const string JWT_CANNOT_BE_ENCRYPTED = "JWT cannnot be encrypted";
        public const string JWT_CANNOT_BE_DECRYPTED = "An unexcepted error occured while trying to decrypt the JWT";
        public const string JWT_BAD_SIGNATURE = "JWT doesn't have a correct signature";
        public const string CANNOT_GENERATE_PAIRWISE_SUBJECT_MORE_THAN_ONE_REDIRECT_URLS = "pairwise subject cannot be generated because the sectore_identifier_uri is empty and the client contains more than one redirect_uri";
        public const string CANNOT_GENERATE_PAIRWISE_SUBJECT_BECAUSE_NO_SECTOR_IDENTIFIER = "pairwise subject cannot be generated because the sector_identifier_uri is empty and there is no redirect_uri";
        public const string POLLING_DEVICE_ALREADY_REGISTERED = "only one polling device can be registrered";
        public const string POLLING_DEVICE_NOT_REGISTERED = "polling device is not registered";
        public const string GRANT_ID_CANNOT_BE_SPECIFIED = "grant_id cannot be specified because the grant_management_action is equals to create";
        public const string BC_AUTHORIZE_NOT_PENDING = "the authorization request is not in pending";
        public const string UNEXPECTED_REQUEST_URI_PARAMETER = "the request cannot contains request_uri";
        public const string REQUEST_URI_IS_REQUIRED = "the request_uri is required";
        public const string REQUEST_URI_IS_INVALID = "the request_uri is invalid";
        public const string AUTHORIZATION_DETAILS_TYPE_REQUIRED = "the authorization_details type is required";
        public const string GRANT_IS_NOT_ACCEPTED = "grant is not accepted";
        public const string USER_EXISTS = "the user {0} already exists";
        public const string CONTRACT_ALREADY_DEPLOYED = "contract is already deployed";
        public const string NO_CONTRACT = "there is no contract";
        public const string ACR_WITH_SAME_NAME_EXISTS = "an acr with the same name already exists";
        public const string AUTHSCHEMEPROVIDER_WITH_SAME_NAME_EXISTS = "an authentication scheme provider with the same name already exists";
        public const string NOT_WELL_FORMED_DPOP_TOKEN = "the DPoP proof must be a Json Web Token";
        public const string USE_DPOP_NONCE = "Authorization Server required nonce in DPoP proof";
        public const string DPOP_JKT_MISMATCH = "there is a mismatch between the dpop_jkt and the DPoP proof";
        public const string USER_NOT_AUTHENTICATED = "you're not authenticated";
        public const string INACTIVE_SESSION = "the session is not active";
        public const string REGISTRATION_WORFKLOW_EXISTS = "a registration worklow with the same name already exists";
        public const string APIRESOURCE_ALREADY_EXISTS = "API resource {0} already exists";
        public const string CANNOT_READ_CERTIFICATE_STORE = "You don't have the permission to read the certificate store";
        public const string CERTIFICATE_DOESNT_EXIST = "The certificate doesn't exist";
        public const string CERTIFICATE_DOESNT_HAVE_PRIVATE_KEY = "The certificate doesn't contain private key";
        public const string SCOPE_CLAIM_MAPPER_NAME_MUSTBEUNIQUE = "Name must be unique";
        public const string SCOPE_CLAIM_MAPPER_TOKENCLAIMNAME_MUSTBEUNIQUE = "Token claim name must be unique";
        public const string SCOPE_CLAIM_MAPPER_SAML_ATTRIBUTE_NAME = "SAML Attribute name must be unique";
        public const string REALM_EXISTS = "realm {0} already exists";
        public const string GROUP_EXISTS = "group {0} already exists";
        public const string IDPROVISIONING_TYPE_UNIQUE = "The same mapping type cannot be added twice.";
        public const string SCOPE_MAPPER_TYPE_UNIQUE = "The same mapping type cannot be added twice.";
        public const string IDPROVISIONING_PROCESS_ISNOTEXTRACTED = "users are not extracted";
        public const string IDPROVISIONING_PROCESS_STARTED = "process cannot be started twice";
	    public const string NO_ACTIVE_OTP = "the user doesn't have an active OTP";
        public const string EXPIRED_BC_AUTHORIZE = "the authorization request is expired";
        public const string UNAUTHORIZED_TO_VALIDATE_BC_AUTHORIZATION = "you are not authorized to validate the authorization request";
    }
}
