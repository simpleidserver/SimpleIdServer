using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Scim.Persistence.EF;
using System;

namespace SimpleIdServer.Scim.Migrations.SqlServer
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SCIMDbContext>())
                {
                    context.Database.Migrate();
                    EnsureSeedData(context);
                }
            }
        }

        private static void EnsureSeedData(SCIMDbContext context)
        {
            Console.WriteLine("Seeding database...");
            context.SCIMSchemaLst.Add(SCIMConstants.StandardSchemas.GroupSchema);
            context.SCIMSchemaLst.Add(SCIMConstants.StandardSchemas.UserSchema);
            context.SaveChanges();
        }
    }
}
