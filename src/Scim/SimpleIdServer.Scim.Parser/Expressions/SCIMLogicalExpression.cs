// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Operators;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Parser.Expressions
{
    public class SCIMLogicalExpression : SCIMExpression
    {
        public SCIMLogicalExpression(SCIMLogicalOperators logicalOperator, SCIMExpression leftExpression, SCIMExpression rightExpression)
        {
            LogicalOperator = logicalOperator;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        public SCIMLogicalOperators LogicalOperator { get; private set; }
        public SCIMExpression LeftExpression { get; private set; }
        public SCIMExpression RightExpression{ get; private set; }

        public override object Clone()
        {
            return new SCIMLogicalExpression(LogicalOperator, (SCIMExpression)LeftExpression.Clone(), (SCIMExpression)RightExpression.Clone());
        }

        public override ICollection<SCIMRepresentationAttribute> BuildEmptyAttributes()
        {
            var result = new List<SCIMRepresentationAttribute>();
            result.AddRange(LeftExpression.BuildEmptyAttributes());
            result.AddRange(RightExpression.BuildEmptyAttributes());
            return result;
        }
    }
}
