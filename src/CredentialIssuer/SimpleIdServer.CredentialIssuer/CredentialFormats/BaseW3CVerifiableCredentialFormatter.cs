using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public abstract class BaseW3CVerifiableCredentialFormatter : ICredentialFormatter
{
    private const string VerifiableCredentialJsonLdContext = "https://www.w3.org/2018/credentials/v1";
    public abstract string Format { get; }

    public JsonObject ExtractCredentialIssuerMetadata(CredentialConfiguration configuration)
    {
        var ctx = new JsonArray
        {
            VerifiableCredentialJsonLdContext,
            configuration.JsonLdContext
        };
        var type = new JsonArray
        {
            "VerifiableCredential",
            configuration.Id
        };
        var credentialSubject = new JsonObject();
        foreach(var claim in configuration.Claims)
        {
            var record = new JsonObject();
            if (claim.Mandatory != null)
                record.Add("mandatory", claim.Mandatory.Value);
            if (!string.IsNullOrWhiteSpace(claim.ValueType))
                record.Add("value_type", claim.ValueType);
            var displays = new JsonArray();
            foreach(var display in claim.Translations)
            {
                var translation = new JsonObject();
                if (!string.IsNullOrWhiteSpace(display.Name))
                    translation.Add("name", display.Name);
                if (!string.IsNullOrWhiteSpace(display.Locale))
                    translation.Add("locale", display.Locale);
                displays.Add(translation);
            }

            record.Add("display", displays);
            credentialSubject.Add(claim.Name, record);
        }

        var result = new JsonObject
        {
            { "@context", ctx },
            { "type", type },
            { "credentialSubject", credentialSubject }
        };
        return result;
    }

    public CredentialHeader ExtractHeader(JsonObject jsonObj)
    {
        return null;
    }

    public abstract JsonNode Build(BuildCredentialRequest request, DidDocument didDocument, string verificationMethodId);

    protected W3CVerifiableCredential BuildCredential(BuildCredentialRequest request, bool includeContext = true)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        var verifiableCredential = new W3CVerifiableCredential
        {
            Id = request.Id,
            Issuer = request.Issuer,
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            CredentialSubject = new JsonObject
            {
                { "id", request.Subject }
            }
        };
        if(includeContext)
        {
            verifiableCredential.Context.Add(VerifiableCredentialJsonLdContext);
            verifiableCredential.Context.Add(request.JsonLdContext);
        }

        var flatNodes = request.UserCredentialClaims.Select(c => new CredentialSubjectClaim
        {
            Level = c.Template.Name.Split('.').Count(),
            Name = c.Template.Name,
            Value = c.Value
        });
        Build(flatNodes, verifiableCredential.CredentialSubject, 0);
        verifiableCredential.Type.Add(request.Type);
        return verifiableCredential;
    }

    private static void Build(IEnumerable<CredentialSubjectClaim> claims, JsonObject jsonObj, int level)
    {
        var filteredClaims = claims.Where(c => c.Level == level);
        foreach(var cl in filteredClaims)
        {
            jsonObj.Add(cl.Name, cl.Value);
        }

        level = level + 1;
        var nextClaims = claims.Where(c => c.Level == level);
        if (!nextClaims.Any()) return;
        foreach (var grp in nextClaims.Select(c =>
        {
            var splittedName = c.Name.Split('.');
            var parentName = splittedName.ElementAt(splittedName.Length - 2);
            return new { ParentName = parentName, Claim = c };
        }).GroupBy(g => g.ParentName))
        {
            var parent = new JsonObject();
            jsonObj.Add(grp.Key, parent);
            Build(claims, parent, level);
        }
    }
}
