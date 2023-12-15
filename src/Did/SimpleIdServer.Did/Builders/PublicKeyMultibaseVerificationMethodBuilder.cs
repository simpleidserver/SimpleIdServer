using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Builders;

public class PublicKeyMultibaseVerificationMethodBuilder : IVerificationMethodBuilder
{
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/v3-unstable";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => "publicKeyMultibase";

    public IdentityDocumentVerificationMethod Build(IdentityDocument idDocument, ISignatureKey signatureKey)
    {
        throw new System.NotImplementedException();
    }
}
