// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Parser.Expressions
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
        public SCIMSchemaAttribute SchemaAttribute { get; set; }

        public bool TryContainsGroupingExpression(out SCIMComplexAttributeExpression result)
        {
            result = null;
            if (this is SCIMComplexAttributeExpression)
            {
                result = this as SCIMComplexAttributeExpression;
                return true;
            }

            if (Child == null)
            {
                return false;
            }

            return Child.TryContainsGroupingExpression(out result);

        }

        public string GetNamespace()
        {
            return ExtractNamespace(Name);
        }

        public string GetFullPath(bool withNamespace = true)
        {
            var names = new List<string>();
            GetFullPath(names);
            var result = string.Join(".", names);
            if (!withNamespace)
            {
                result = RemoveNamespace(result);
            }

            return result;
        }

        protected void GetFullPath(List<string> names)
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

        public SCIMAttributeExpression GetLastChild()
        {
            if (Child != null)
            {
                return Child.GetLastChild();
            }

            return this;
        }

        public int NbChildren()
        {
            int nbChildren = 0;
            IncrementNbChildren(ref nbChildren);
            return nbChildren;
        }

        public override object Clone()
        {
            return ProtectedClone();
        }

        public static string RemoveNamespace(string name)
        {
            var result = name;
            var ns = ExtractNamespace(name);
            if (!string.IsNullOrWhiteSpace(ns))
            {
                result = result.Replace(ns, string.Empty).TrimStart(':');
            }

            return result;
        }

        public static string ExtractNamespace(string name)
        {
            var index = name.LastIndexOf(':');
            if (index == -1)
            {
                return string.Empty;
            }

            return new string(name.Take(index).ToArray());
        }

        public static bool HasNamespace(string name)
        {
            return name.Contains(':');
        }

        protected virtual object ProtectedClone()
        {
            return new SCIMAttributeExpression(Name, (SCIMAttributeExpression)Child.Clone());
        }

        protected void IncrementNbChildren(ref int nbChildren)
        {
            if (Child != null)
            {
                nbChildren++;
                Child.IncrementNbChildren(ref nbChildren);
            }
        }
    }
}
