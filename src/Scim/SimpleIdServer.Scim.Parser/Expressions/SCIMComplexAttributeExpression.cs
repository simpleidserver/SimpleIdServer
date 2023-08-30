// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Parser.Expressions
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

        protected override ICollection<SCIMRepresentationAttribute> InternalBuildEmptyAttributes()
        {
            var result = base.InternalBuildEmptyAttributes();
            var first = result.First();
            var groupingAttrs = GroupingFilter.BuildEmptyAttributes();
            foreach (var child in groupingAttrs)
            {
                if (first.Children.Any(c => c.FullPath == child.FullPath)) continue;
                first.Children.Add(child);
                child.ParentAttributeId = first.Id;
            }

            return result;
        }
    }
}