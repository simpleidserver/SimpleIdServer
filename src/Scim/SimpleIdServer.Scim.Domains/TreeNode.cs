// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Domains
{
    public class TreeNode<T> where T : class
    {
        public TreeNode(T leaf)
        {
            Leaf = leaf;
            Children = new List<TreeNode<T>>();
        }

        public T Leaf { get; set; }
        public List<TreeNode<T>> Children { get; set; }

        public void AddChildren(List<TreeNode<T>> children)
        {
            Children.AddRange(children);
        }

        public List<T> ToFlat()
        {
            var result = new List<T>();
            result.Add(Leaf);
            foreach(var child in Children)
            {
                result.AddRange(child.ToFlat());
            }

            return result;
        }
    }
}
