using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Store.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Store.EF.Configurations
{
    public class CastbinConfiguration : IEntityTypeConfiguration<CasbinPolicy>
    {
        private static string _tableName;

        public CastbinConfiguration(string tableName)
        {
            _tableName = tableName;    
        }

        public void Configure(EntityTypeBuilder<CasbinPolicy> builder)
        {
            builder.ToTable(_tableName);
            builder.HasKey(x => x.Id);
        }
    }
}
