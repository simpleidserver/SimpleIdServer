// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Diagnostics;

namespace SimpleIdServer.Scim.Domains
{
    [DebuggerDisplay("{SourceAttributeSelector}, {SourceResourceType} => {TargetResourceType}")]
    public class SCIMAttributeMapping
    {
        public string Id { get; set; }
        public string SourceAttributeId { get; set; }
        public string SourceResourceType { get; set; }
        public string SourceAttributeSelector { get; set; }
        public string TargetResourceType { get; set; }
        public string TargetAttributeId { get; set; }
        public Mode Mode { get; set; } = Mode.STANDARD;
        public bool IsSelf
        {
            get
            {
                return SourceResourceType == TargetResourceType;
            }
        }
    }

    public enum Mode
    {
        STANDARD =  0,
        PROPAGATE_INHERITANCE = 1
    }
}
