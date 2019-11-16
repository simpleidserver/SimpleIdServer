// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Persistence.Filters.SCIMExpressions
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
    }
}
