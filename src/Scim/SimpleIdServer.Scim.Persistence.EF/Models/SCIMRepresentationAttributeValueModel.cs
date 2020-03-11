// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Scim.Persistence.EF.Models
{
    public class SCIMRepresentationAttributeValueModel
    {
        public string Id { get; set; }
        public string ValueString { get; set; }
        public int ValueInteger { get; set; }
        public bool ValueBoolean { get; set; }
        public DateTime ValueDateTime { get; set; }
        public string ValueReference { get; set; }
        public string SCIMRepresentationAttributeId { get; set; }
        public virtual SCIMRepresentationAttributeModel RepresentationAttribute { get; set; }
    }
}
