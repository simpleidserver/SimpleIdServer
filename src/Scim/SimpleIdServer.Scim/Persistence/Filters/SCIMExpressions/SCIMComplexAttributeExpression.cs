// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Persistence.Filters.SCIMExpressions
{
    public class SCIMComplexAttributeExpression : SCIMAttributeExpression
    {
        public SCIMComplexAttributeExpression(string name, SCIMExpression groupingFilter) : base(name)
        {
            GroupingFilter = groupingFilter;
        }

        public SCIMComplexAttributeExpression(string name, SCIMExpression groupingFilter, SCIMAttributeExpression child) : this(name, child) { }

        public SCIMExpression GroupingFilter { get; set; }

        public override object Clone()
        {
            return new SCIMComplexAttributeExpression(Name, (SCIMExpression)GroupingFilter.Clone(), (SCIMAttributeExpression)Child.Clone());
        }
    }
}