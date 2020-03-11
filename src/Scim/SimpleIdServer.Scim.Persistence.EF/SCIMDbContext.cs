// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimpleIdServer.Scim.Persistence.EF.Models;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class SCIMDbContext : DbContext
    {
        public SCIMDbContext(DbContextOptions<SCIMDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<SCIMSchemaModel> SCIMSchemaLst { get; set; } 
        public DbSet<SCIMRepresentationModel> SCIMRepresentationLst { get; set; }
        public DbSet<SCIMRepresentationAttributeModel> SCIMRepresentationAttributeLst { get; set; }
        public DbSet<SCIMRepresentationSchemaModel> SCIMRepresentationSchemaLst { get; set; }
        public DbSet<SCIMRepresentationAttributeValueModel> SCIMRepresentationAttributeValueLst { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SCIMRepresentationSchemaModel>()
                .HasKey(s => new { s.SCIMSchemaId, s.SCIMRepresentationId });
            modelBuilder.Entity<SCIMSchemaModel>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<SCIMSchemaModel>()
                .HasMany(s => s.Representations)
                .WithOne(s => s.Schema)
                .HasForeignKey(s => s.SCIMSchemaId);
            modelBuilder.Entity<SCIMSchemaModel>()
                .HasMany(s => s.Attributes)
                .WithOne(s => s.Schema)
                .HasForeignKey(s => s.SchemaId);
            modelBuilder.Entity<SCIMSchemaExtensionModel>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<SCIMSchemaAttributeModel>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<SCIMSchemaAttributeModel>()
                .HasMany(s => s.SubAttributes)
                .WithOne(s => s.ParentAttribute)
                .HasForeignKey(s => s.ParentId);
            modelBuilder.Entity<SCIMSchemaAttributeModel>()
                .Property(s => s.CanonicalValues)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
            modelBuilder.Entity<SCIMSchemaAttributeModel>()
                .Property(s => s.ReferenceTypes)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
            modelBuilder.Entity<SCIMSchemaAttributeModel>()
                .Property(s => s.DefaultValueString)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
            modelBuilder.Entity<SCIMSchemaAttributeModel>()
                .Property(s => s.DefaultValueInt)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<int>>(v));
            modelBuilder.Entity<SCIMRepresentationModel>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<SCIMRepresentationModel>()
                .HasMany(r => r.Schemas)
                .WithOne(r => r.Representation)
                .HasForeignKey(r => r.SCIMRepresentationId);
            modelBuilder.Entity<SCIMRepresentationModel>()
                .HasMany(r => r.Attributes)
                .WithOne(r => r.Representation)
                .HasForeignKey(r => r.RepresentationId);
            modelBuilder.Entity<SCIMRepresentationAttributeModel>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<SCIMRepresentationAttributeModel>()
                .HasMany(r => r.Children)
                .WithOne(r => r.Parent)
                .HasForeignKey(r => r.ParentId);
            modelBuilder.Entity<SCIMRepresentationAttributeModel>()
                .HasOne(r => r.SchemaAttribute)
                .WithMany(r => r.RepresentationAttributes)
                .HasForeignKey(r => r.SchemaAttributeId);
            modelBuilder.Entity<SCIMRepresentationAttributeModel>()
                .HasMany(r => r.Values)
                .WithOne(r => r.RepresentationAttribute)
                .HasForeignKey(r => r.SCIMRepresentationAttributeId);
        }
    }
}
