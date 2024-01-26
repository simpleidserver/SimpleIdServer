using SimpleIdServer.CredentialIssuer.CredentialFormats.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public abstract class BaseW3CVerifiableCredentialFormatter : ICredentialFormatter
{
    public abstract string Format { get; }

    public CredentialHeader ExtractHeader(JsonObject jsonObj)
    {
        return null;
    }

    protected W3CVerifiableCredential BuildCredential(BuildCredentialRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        var verifiableCredential = new W3CVerifiableCredential
        {
            Id = request.Id,
            Issuer = request.Issuer,
            Type = new List<string>
            {
                "VerifiableCredential",
                request.Type
            },
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            CredentialSubject = new W3CCredentialSubject
            {
                Id = request.Subject,
            }
        };
        return verifiableCredential;
    }
}
