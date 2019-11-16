// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class SCIMDbContext : DbContext
    {
        public SCIMDbContext(DbContextOptions<SCIMDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<SCIMSchema> SCIMSchemaLst { get; set; } 
        public DbSet<SCIMRepresentation> SCIMRepresentationLst { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SCIMSchema>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<SCIMSchemaAttribute>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<SCIMSchemaAttribute>()
                .Property(s => s.CanonicalValues)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
            modelBuilder.Entity<SCIMSchemaAttribute>()
                .Property(s => s.ReferenceTypes)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
            modelBuilder.Entity<SCIMSchemaAttribute>()
                .Property(s => s.DefaultValueString)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
            modelBuilder.Entity<SCIMSchemaAttribute>()
                .Property(s => s.DefaultValueInt)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<int>>(v));
            modelBuilder.Entity<SCIMRepresentation>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<SCIMRepresentationAttribute>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<SCIMRepresentationAttribute>()
                .Property(s => s.ValuesString)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
            modelBuilder.Entity<SCIMRepresentationAttribute>()
                .Property(s => s.ValuesInteger)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<int>>(v));
            modelBuilder.Entity<SCIMRepresentationAttribute>()
                .Property(s => s.ValuesBoolean)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<bool>>(v));
            modelBuilder.Entity<SCIMRepresentationAttribute>()
                .Property(s => s.ValuesDateTime)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<DateTime>>(v));
            modelBuilder.Entity<SCIMRepresentationAttribute>()
                .Property(s => s.ValuesReference)
                .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<string>>(v));
        }
    }
}
