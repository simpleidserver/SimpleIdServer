// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Scim.Persistence.EF.Models
{
    public class SCIMRepresentationSchemaModel
    {
        public string SCIMSchemaId { get; set; }
        public string SCIMRepresentationId { get; set; }
        public virtual SCIMSchemaModel Schema { get; set; }
        public virtual SCIMRepresentationModel Representation { get; set; }
    }
}
