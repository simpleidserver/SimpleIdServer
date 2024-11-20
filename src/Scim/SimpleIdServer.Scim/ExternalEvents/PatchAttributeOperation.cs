// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.ExternalEvents;

public class PatchAttributeOperation
{
    public string Path { get; set; }
    public SCIMPatchOperations Operation { get; set; }
    public SCIMSchemaAttributeTypes Type {  get; set; }
    public string ValueString { get; set; }
    public bool? ValueBoolean { get; set; }
    public int? ValueInteger { get; set; }
    public DateTime? ValueDateTime { get; set; }
    public string ValueReference { get; set; }
    public decimal? ValueDecimal { get; set; }
    public string ValueBinary { get; set; }

    public static List<PatchAttributeOperation> Transform(List<SCIMPatchResult> patchOperations)
    {
        return patchOperations.Select(o => new PatchAttributeOperation
        {
            Operation = o.Operation,
            Path = o.Path,
            Type = o.Attr.SchemaAttribute.Type,
            ValueBinary = o.Attr.ValueBinary,
            ValueBoolean = o.Attr.ValueBoolean,
            ValueDateTime = o.Attr.ValueDateTime,
            ValueDecimal = o.Attr.ValueDecimal,
            ValueInteger = o.Attr.ValueInteger,
            ValueReference = o.Attr.ValueReference,
            ValueString = o.Attr.ValueString
        }).ToList();
    }
}
