// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.EF.Configurations;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class SCIMDbContext : DbContext
    {
        public SCIMDbContext(DbContextOptions<SCIMDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<SCIMAttributeMapping> SCIMAttributeMappingLst { get; set; }
        public DbSet<SCIMSchema> SCIMSchemaLst { get; set; }
        public DbSet<SCIMRepresentation> SCIMRepresentationLst { get; set; }
        public DbSet<SCIMRepresentationAttribute> SCIMRepresentationAttributeLst { get; set; }
        public DbSet<ProvisioningConfiguration> ProvisioningConfigurations { get; set; }
        public DbSet<ProvisioningConfigurationHistory> ProvisioningConfigurationHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ProvisioningConfConfiguration());
            modelBuilder.ApplyConfiguration(new ProvisioningConfigurationHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProvisioningConfigurationRecordConfiguration());
            modelBuilder.ApplyConfiguration(new SCIMAttributeMappingConfiguration());
            modelBuilder.ApplyConfiguration(new SCIMRepresentationAttributeConfiguration());
            modelBuilder.ApplyConfiguration(new SCIMRepresentationConfiguration());
            modelBuilder.ApplyConfiguration(new SCIMSchemaAttributeConfiguration());
            modelBuilder.ApplyConfiguration(new SCIMSchemaConfiguration());
            modelBuilder.ApplyConfiguration(new SCIMSchemaExtensionConfiguration());
            modelBuilder.ApplyConfiguration(new SCIMRepresentationIndirectReferenceConfiguration());
        }
    }
}
