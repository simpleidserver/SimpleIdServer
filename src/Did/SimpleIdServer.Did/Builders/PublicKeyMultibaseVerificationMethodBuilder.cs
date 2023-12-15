using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System.Linq;

namespace SimpleIdServer.Did.Builders;

/// <summary>
/// https://www.w3.org/community/reports/credentials/CG-FINAL-di-eddsa-2020-20220724/
/// </summary>
public class PublicKeyMultibaseVerificationMethodBuilder : IVerificationMethodBuilder
{
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/ed25519-2020/v1";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => "Ed25519VerificationKey2020";

    public IdentityDocumentVerificationMethod Build(IdentityDocument idDocument, ISignatureKey signatureKey)
    {
        return new IdentityDocumentVerificationMethod
        {
            Id = $"{idDocument.Id}#keys-{(idDocument.VerificationMethod.Where(m => m.Type == Type).Count() + 1)}",
            PublicKeyMultibase = PublicKeyMultibase.Compute(signatureKey)
        };
    }
}
