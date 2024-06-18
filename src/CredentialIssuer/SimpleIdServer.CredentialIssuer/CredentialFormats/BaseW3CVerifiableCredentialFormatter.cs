using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc;
using SimpleIdServer.Vc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public abstract class BaseW3CVerifiableCredentialFormatter : ICredentialFormatter
{
    public abstract string Format { get; }

    public JsonObject ExtractCredentialIssuerMetadata(CredentialConfiguration configuration)
    {
        var ctx = new JsonArray
        {
            VcConstants.VerifiableCredentialJsonLdContext,
            configuration.JsonLdContext
        };
        var types = new JsonArray
        {
            VcConstants.VerifiableCredentialType,
            configuration.Type
        };
        if(configuration.AdditionalTypes != null)
        {
            foreach (var additionalType in configuration.AdditionalTypes.Where(a => !string.IsNullOrWhiteSpace(a)))
                types.Add(additionalType);
        }

        var credentialSubject = new JsonObject();
        var flatNodes = configuration.Claims.Select(c =>
        {
            return new CredentialConfigurationClaimNode
            {
                Level = c.Name.Split('.').Count(),
                Name = c.Name,
                ConfigurationClaim = c
            };
        });
        Build(flatNodes, credentialSubject, 1);
        var result = new JsonObject
        {
            { "@context", ctx },
            { "types", types },
            { "credentialSubject", credentialSubject }
        };
        return result;
    }

    public CredentialHeader ExtractHeader(JsonObject jsonObj)
    {
        if (!jsonObj.ContainsKey("credential_definition")) return null;
        var credentialDefinition = jsonObj["credential_definition"].AsObject();
        if (credentialDefinition == null || !credentialDefinition.ContainsKey("type")) return null;
        var jArrTypes = credentialDefinition["type"].AsArray();
        if (jArrTypes == null) return null;
        var filteredTypes = jArrTypes.Select(t => t.ToString()).Where(c => c != VcConstants.VerifiableCredentialType);
        if (filteredTypes.Count() != 1) return null;
        return new CredentialHeader
        {
            Type = filteredTypes.Single()
        };
    }

    public abstract JsonNode Build(BuildCredentialRequest request, DidDocument didDocument, string verificationMethodId, IAsymmetricKey asymmetricKey);

    protected W3CVerifiableCredential BuildCredential(BuildCredentialRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        var builder = VcBuilder.New(request.Id,
            request.JsonLdContext,
            request.Issuer,
            request.Type,
            request.ValidFrom,
            request.ValidUntil);
        builder.AddCredentialSubject(request.Subject, (b) =>
        {
            Build(request.UserClaims, b.Build(), 1);
        });
        return builder.Build();
    }

    private static void Build(IEnumerable<CredentialConfigurationClaimNode> claims, JsonObject jsonObj, int level)
    {
        Build(claims, jsonObj, level, (c) => Build(c.ConfigurationClaim));
    }

    private static void Build(IEnumerable<CredentialUserClaimNode> claims, JsonObject jsonObj, int level)
    {
        Build(claims, jsonObj, level, (c) => c.Value);
    }

    private static void Build<T>(IEnumerable<T> claims, JsonObject jsonObj, int level, Func<T, JsonNode> callback) where T : INode
    {
        var filteredClaims = claims.Where(c => c.Level == level);
        foreach (var cl in filteredClaims)
        {
            var eltName = cl.Name.Split('.').ElementAt(level - 1);
            jsonObj.Add(eltName, callback(cl));
        }

        level = level + 1;
        var nextClaims = claims.Where(c => c.Level == level);
        if (!nextClaims.Any()) return;
        foreach (var grp in nextClaims.Select(c =>
        {
            var splittedName = c.Name.Split('.');
            var parentName = string.Empty;
            if (splittedName.Length > 1)
            {
                parentName = splittedName.ElementAt(splittedName.Length - 2);
            }

            return new { ParentName = parentName, Claim = c };
        }).GroupBy(g => g.ParentName))
        {
            var parent = new JsonObject();
            jsonObj.Add(grp.Key, parent);
            Build<T>(claims, parent, level, callback);
        }
    }

    private static JsonObject Build(CredentialConfigurationClaim claim)
    {
        var record = new JsonObject();
        if (claim.Mandatory != null)
            record.Add("mandatory", claim.Mandatory.Value);
        if (!string.IsNullOrWhiteSpace(claim.ValueType))
            record.Add("value_type", claim.ValueType);
        var displays = new JsonArray();
        foreach (var display in claim.Translations)
        {
            var translation = new JsonObject();
            if (!string.IsNullOrWhiteSpace(display.Name))
                translation.Add("name", display.Name);
            if (!string.IsNullOrWhiteSpace(display.Locale))
                translation.Add("locale", display.Locale);
            displays.Add(translation);
        }

        record.Add("display", displays);
        return record;
    }
}

public class CredentialConfigurationClaimNode : INode
{
    public int Level { get; set; }
    public string Name { get; set; }
    public CredentialConfigurationClaim ConfigurationClaim { get; set; }
}