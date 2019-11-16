// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Persistence.Filters.SCIMExpressions
{
    public class SCIMNotExpression : SCIMExpression
    {
        public SCIMNotExpression(SCIMExpression content)
        {
            Content = content;
        }

        public SCIMExpression Content { get; private set; }

        public override object Clone()
        {
            return new SCIMNotExpression((SCIMExpression)Content.Clone());
        }
    }
}
