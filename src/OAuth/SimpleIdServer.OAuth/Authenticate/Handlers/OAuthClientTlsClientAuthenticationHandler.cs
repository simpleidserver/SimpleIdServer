// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Authenticate.Handlers
{
    public class OAuthClientTlsClientAuthenticationHandler : IOAuthClientAuthenticationHandler
    {
        public OAuthClientTlsClientAuthenticationHandler()
        {

        }


        public const string AUTH_METHOD = "tls_client_auth";
        public string AuthMethod => "tls_client_auth";

        public Task<bool> Handle(AuthenticateInstruction authenticateInstruction, OAuthClient client, string expectedIssuer, CancellationToken cancellationToken)
        {
            var certificate = authenticateInstruction.Certificate;
            if (certificate == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.NO_CLIENT_CERTIFICATE);
            }

            if (!string.IsNullOrWhiteSpace(client.TlsClientAuthSubjectDN) && client.TlsClientAuthSubjectDN != certificate.Subject)
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.CERTIFICATE_SUBJECT_INVALID);
            }

            var subjectAlternative = certificate.GetSubjectAlternativeName();
            if (!string.IsNullOrWhiteSpace(client.TlsClientAuthSanDNS) && !Check(client.TlsClientAuthSanDNS, SubjectAlternativeNameTypes.DNSNAME, subjectAlternative))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.CERTIFICATE_SAN_DNS_INVALID);
            }

            if (!string.IsNullOrWhiteSpace(client.TlsClientAuthSanEmail) && !Check(client.TlsClientAuthSanEmail, SubjectAlternativeNameTypes.EMAIL, subjectAlternative))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.CERTIFICATE_SAN_EMAIL_INVALID);
            }

            if (!string.IsNullOrWhiteSpace(client.TlsClientAuthSanIP) && !Check(client.TlsClientAuthSanIP, SubjectAlternativeNameTypes.IPADDRESS, subjectAlternative))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT_AUTH, ErrorMessages.CERTIFICATE_SAN_IP_INVALID);
            }

            return Task.FromResult(true);
        }

        private static bool Check(string value, SubjectAlternativeNameTypes type, List<KeyValuePair<SubjectAlternativeNameTypes, string>> san)
        {
            if (!san.Any(_ => _.Key == type && _.Value == value))
            {
                return false;
            }

            return true;
        }
    }
}
