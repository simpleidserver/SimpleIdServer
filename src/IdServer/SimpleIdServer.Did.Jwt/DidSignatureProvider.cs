using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto;

namespace SimpleIdServer.Did.Jwt
{
    public class DidSignatureProvider : SignatureProvider
    {
        private readonly ISignatureKey _signatureKey;

        public DidSignatureProvider(DidSecurityKey key, string alg, ISignatureKey signatureKey) : base(key, alg)
        {
            _signatureKey = signatureKey;
        }

        public override byte[] Sign(byte[] input) => Base64UrlEncoder.DecodeBytes(_signatureKey.Sign(input));

        public override bool Verify(byte[] input, byte[] signature) => _signatureKey.Check(input, signature);

        protected override void Dispose(bool disposing) { }
    }
}
