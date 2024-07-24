using Microsoft.Extensions.Options;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Builders;
using SimpleIdServer.OpenidFederation.Stores;

namespace SimpleIdServer.Authority.Federation.Builders;

public interface IAuthorityFederationEntityBuilder
{
    Task<OpenidFederationResult> BuildSelfIssued(BuildFederationEntityRequest request, CancellationToken cancellationToken);
}

public class AuthorityFederationEntityBuilder : BaseFederationEntityBuilder, IAuthorityFederationEntityBuilder
{
    private readonly AuthorityFederationOptions _options;

    public AuthorityFederationEntityBuilder(
        IOptions<AuthorityFederationOptions> options, 
        IFederationEntityStore federationEntityStore) : base(federationEntityStore)
    {
        _options = options.Value;
    }

    protected override bool IsFederationEnabled => true;

    protected override string OrganizationName => _options.OrganizationName;

    protected override Task EnrichSelfIssued(OpenidFederationResult federationEntity, BuildFederationEntityRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}