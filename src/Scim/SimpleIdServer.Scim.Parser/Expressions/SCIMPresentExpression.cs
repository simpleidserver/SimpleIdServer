// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Parser.Expressions
{
    public class SCIMPresentExpression : SCIMExpression
    {
        public SCIMPresentExpression(SCIMAttributeExpression content)
        {
            Content = content;
        }

        public SCIMAttributeExpression Content { get; private set; }

        public override object Clone()
        {
            return new SCIMPresentExpression((SCIMAttributeExpression)Content.Clone());
        }

        public override ICollection<SCIMRepresentationAttribute> BuildEmptyAttributes() => new List<SCIMRepresentationAttribute>();
    }
}
