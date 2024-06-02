// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("PresentationDefinitionFormat")]
public class SugarPresentationDefinitionFormat
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; }
    public string Format { get; set; }
    public string ProofType { get; set; }
    public string PresentationDefinitionInputDescriptorId {  get; set; }

    public PresentationDefinitionFormat ToDomain()
    {
        return new PresentationDefinitionFormat
        {
            Id = Id,
            Format = Format,
            ProofType = ProofType
        };
    }
}