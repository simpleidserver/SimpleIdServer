// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.Configurations;

public class RegistrationWorkflowConfiguration : IEntityTypeConfiguration<RegistrationWorkflow>
{
    public void Configure(EntityTypeBuilder<RegistrationWorkflow> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
