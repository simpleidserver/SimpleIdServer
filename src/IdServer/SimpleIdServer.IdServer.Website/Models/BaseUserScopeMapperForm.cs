using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Models
{
    public record BaseUserScopeMapperForm
    {
        public string Name { get; set; } = null!;
        public string TokenClaimName { get; set; } = null!;
        public TokenClaimJsonTypes ClaimJsonType { get; set; } = TokenClaimJsonTypes.STRING;
        public bool IsOpenIdProtocolEnabled { get; set; } = true;
        public bool IsWsFederationProtocolEnabled { get; set; }

        public void Update(ScopeClaimMapper mapper)
        {
            Name = mapper.Name;
            TokenClaimName = mapper.TokenClaimName;
            ClaimJsonType = mapper.TokenClaimJsonType.Value;
            if (mapper.ApplicationScope.HasFlag(MapperApplicationScopes.IDTOKEN))
                IsOpenIdProtocolEnabled = true;
            if (mapper.ApplicationScope.HasFlag(MapperApplicationScopes.WSFEDERATION))
                IsWsFederationProtocolEnabled = true;
        }

        public ScopeClaimMapper Build()
        {
            MapperApplicationScopes scope = 0;
            if (IsOpenIdProtocolEnabled)
                scope |= MapperApplicationScopes.IDTOKEN | MapperApplicationScopes.USERINFO;
            if (IsWsFederationProtocolEnabled)
                scope |= MapperApplicationScopes.WSFEDERATION;

            return new ScopeClaimMapper
            {
                Name = Name,
                TokenClaimName = TokenClaimName,
                ApplicationScope = scope,
                TokenClaimJsonType = ClaimJsonType
            };
        }
    }
}
