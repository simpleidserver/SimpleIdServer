using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OpenID.Domains;

namespace SimpleIdServer.OpenID.EF
{
    public class BaseOpenIdDBContext<TContext> : DbContext where TContext : DbContext
    {
        public BaseOpenIdDBContext(DbContextOptions<TContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<JsonWebKey> JsonWebKeys { get; set; }
        public DbSet<OpenIdClient> OpenIdClients { get; set; }
        public DbSet<OAuthUser> Users { get; set; }
        public DbSet<BCAuthorize> BCAuthorizeLst { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<OAuthScope> OAuthScopes { get; set; }
        public DbSet<AuthenticationContextClassReference> Acrs { get; set; }
    }
}
