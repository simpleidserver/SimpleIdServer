// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SimpleIdServer.Scim.Persistence.EF;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    [DbContext(typeof(SCIMDbContext))]
    partial class SCIMDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMRepresentationAttributeModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ParentId");

                    b.Property<string>("RepresentationId");

                    b.Property<string>("SchemaAttributeId");

                    b.Property<string>("ValuesBoolean");

                    b.Property<string>("ValuesDateTime");

                    b.Property<string>("ValuesInteger");

                    b.Property<string>("ValuesReference");

                    b.Property<string>("ValuesString");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("RepresentationId");

                    b.HasIndex("SchemaAttributeId");

                    b.ToTable("SCIMRepresentationAttributeLst");
                });

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMRepresentationModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("ExternalId");

                    b.Property<DateTime>("LastModified");

                    b.Property<string>("ResourceType");

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.ToTable("SCIMRepresentationLst");
                });

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMRepresentationSchemaModel", b =>
                {
                    b.Property<string>("SCIMSchemaId");

                    b.Property<string>("SCIMRepresentationId");

                    b.HasKey("SCIMSchemaId", "SCIMRepresentationId");

                    b.HasIndex("SCIMRepresentationId");

                    b.ToTable("SCIMRepresentationSchemaLst");
                });

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaAttributeModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CanonicalValues");

                    b.Property<bool>("CaseExact");

                    b.Property<string>("DefaultValueInt");

                    b.Property<string>("DefaultValueString");

                    b.Property<string>("Description");

                    b.Property<bool>("MultiValued");

                    b.Property<int>("Mutability");

                    b.Property<string>("Name");

                    b.Property<string>("ParentId");

                    b.Property<string>("ReferenceTypes");

                    b.Property<bool>("Required");

                    b.Property<int>("Returned");

                    b.Property<string>("SchemaId");

                    b.Property<int>("Type");

                    b.Property<int>("Uniqueness");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("SchemaId");

                    b.ToTable("SCIMSchemaAttributeModel");
                });

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaExtensionModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Required");

                    b.Property<string>("SCIMSchemaModelId");

                    b.Property<string>("Schema");

                    b.HasKey("Id");

                    b.HasIndex("SCIMSchemaModelId");

                    b.ToTable("SCIMSchemaExtensionModel");
                });

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<bool>("IsRootSchema");

                    b.Property<string>("Name");

                    b.Property<string>("ResourceType");

                    b.HasKey("Id");

                    b.ToTable("SCIMSchemaLst");
                });

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMRepresentationAttributeModel", b =>
                {
                    b.HasOne("SimpleIdServer.Scim.Persistence.EF.Models.SCIMRepresentationAttributeModel", "Parent")
                        .WithMany("Values")
                        .HasForeignKey("ParentId");

                    b.HasOne("SimpleIdServer.Scim.Persistence.EF.Models.SCIMRepresentationModel", "Representation")
                        .WithMany("Attributes")
                        .HasForeignKey("RepresentationId");

                    b.HasOne("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaAttributeModel", "SchemaAttribute")
                        .WithMany("RepresentationAttributes")
                        .HasForeignKey("SchemaAttributeId");
                });

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMRepresentationSchemaModel", b =>
                {
                    b.HasOne("SimpleIdServer.Scim.Persistence.EF.Models.SCIMRepresentationModel", "Representation")
                        .WithMany("Schemas")
                        .HasForeignKey("SCIMRepresentationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaModel", "Schema")
                        .WithMany("Representations")
                        .HasForeignKey("SCIMSchemaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaAttributeModel", b =>
                {
                    b.HasOne("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaAttributeModel", "ParentAttribute")
                        .WithMany("SubAttributes")
                        .HasForeignKey("ParentId");

                    b.HasOne("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaModel", "Schema")
                        .WithMany("Attributes")
                        .HasForeignKey("SchemaId");
                });

            modelBuilder.Entity("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaExtensionModel", b =>
                {
                    b.HasOne("SimpleIdServer.Scim.Persistence.EF.Models.SCIMSchemaModel")
                        .WithMany("SchemaExtensions")
                        .HasForeignKey("SCIMSchemaModelId");
                });
#pragma warning restore 612, 618
        }
    }
}
