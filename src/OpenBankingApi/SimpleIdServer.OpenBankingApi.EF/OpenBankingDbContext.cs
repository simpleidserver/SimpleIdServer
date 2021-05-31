using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenID.EF;

namespace SimpleIdServer.OpenBankingApi.EF
{
    public class OpenBankingDbContext : BaseOpenIdDBContext<OpenBankingDbContext>
    {
        public OpenBankingDbContext(DbContextOptions<OpenBankingDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<AccountAccessConsentAggregate> AccountAccessConsents { get; set; }
        public DbSet<AccountAggregate> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<BaseClient>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OpenIdDBContext).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
