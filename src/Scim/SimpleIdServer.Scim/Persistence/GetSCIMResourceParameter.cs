// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Scim.Parser.Expressions;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence
{
    public class GetSCIMResourceParameter
    {
        public IEnumerable<SCIMAttributeExpression> IncludedAttributes { get; set; }
        public IEnumerable<SCIMAttributeExpression> ExcludedAttributes { get; set; }
    }
}
