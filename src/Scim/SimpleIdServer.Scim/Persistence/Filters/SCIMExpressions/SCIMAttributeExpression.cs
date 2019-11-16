// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text;

namespace SimpleIdServer.Persistence.Filters.SCIMExpressions
{
    public class SCIMAttributeExpression : SCIMExpression
    {
        public SCIMAttributeExpression(string name)
        {
            Name = name;
        }

        public SCIMAttributeExpression(string name, SCIMAttributeExpression child) : this(name)
        {
            Child = child;
        }

        public string Name { get; private set; }
        public SCIMAttributeExpression Child { get; set;}

        public string GetFullPath()
        {
            var names = new List<string>();
            GetFullPath(names);
            return string.Join(".", names);
        }

        public void GetFullPath(List<string> names)
        {
            names.Add(Name);
            if (Child != null)
            {
                Child.GetFullPath(names);
            }
        }

        public void SetChild(SCIMAttributeExpression child)
        {
            Child = child;
        }

        public override object Clone()
        {
            return ProtectedClone();
        }

        protected virtual object ProtectedClone()
        {
            return new SCIMAttributeExpression(Name, (SCIMAttributeExpression)Child.Clone());
        }
    }
}
