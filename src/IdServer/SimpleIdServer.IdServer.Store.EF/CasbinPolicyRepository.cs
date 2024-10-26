using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Store.EF.Configurations;
using SimpleIdServer.IdServer.Store.EF.Models;

namespace SimpleIdServer.IdServer.Store.EF
{
    public interface ICasbinPolicyRepository
    {
        Task<List<CasbinPolicy>> GetAll(string tableName, CancellationToken cancellationToken);
        Task<int> SaveChanges(string tableName,CancellationToken cancellationToken);
    }

    public class CasbinPolicyRepository : ICasbinPolicyRepository
    {
        private readonly DbContextOptions<CustomDataContext> _options;
        private readonly Dictionary<string, CustomDataContext> _dic = new Dictionary<string, CustomDataContext>();

        public CasbinPolicyRepository(DbContextOptions<CustomDataContext> options)
        {
            _options = options;
        }

        public async Task<List<CasbinPolicy>> GetAll(string tableName, CancellationToken cancellationToken)
        {
            var ctx = new CustomDataContext(_options);
            ctx.TableName = tableName;
            _dic.Add(tableName, ctx);
            return await ctx.CasbinPolices.ToListAsync(cancellationToken);
        }

        public async Task<int> SaveChanges(string tableName, CancellationToken cancellationToken)
        {
            var ctx = _dic[tableName];
            return await ctx.SaveChangesAsync(cancellationToken);
        }
    }

    public class CustomDataContext : DbContext
    {

        public CustomDataContext(DbContextOptions<CustomDataContext> options) : base(options)
        {
        }

        public DbSet<CasbinPolicy> CasbinPolices { get; set; }
        public string TableName { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CastbinConfiguration(TableName));
        }
    }
}
