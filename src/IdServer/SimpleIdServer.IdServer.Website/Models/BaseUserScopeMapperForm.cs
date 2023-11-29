using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Models
{
    public record BaseUserScopeMapperForm
    {
        public string Name { get; set; } = null!;
        public string? TokenClaimName { get; set; } = null;
        public string? SAMLAttributeName { get; set; } = null;
        public bool IncludeInAccessToken { get; set; }
        public TokenClaimJsonTypes ClaimJsonType { get; set; } = TokenClaimJsonTypes.STRING;

        public void Update(ScopeClaimMapper mapper)
        {
            Name = mapper.Name;
            TokenClaimName = mapper.TargetClaimPath;
            SAMLAttributeName = mapper.SAMLAttributeName;
            IncludeInAccessToken = mapper.IncludeInAccessToken;
            if (mapper.TokenClaimJsonType != null)
                ClaimJsonType = mapper.TokenClaimJsonType.Value;
        }

        public ScopeClaimMapper Build()
        {
            return new ScopeClaimMapper
            {
                Name = Name,
                TargetClaimPath = TokenClaimName,
                SAMLAttributeName = SAMLAttributeName,
                TokenClaimJsonType = ClaimJsonType,
                IncludeInAccessToken = IncludeInAccessToken
            };
        }
    }
}
