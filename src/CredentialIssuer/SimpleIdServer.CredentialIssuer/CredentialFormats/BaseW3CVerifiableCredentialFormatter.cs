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

    public CredentialHeader ExtractHeader(JsonObject jsonObj)
    {
        return null;
    }

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
