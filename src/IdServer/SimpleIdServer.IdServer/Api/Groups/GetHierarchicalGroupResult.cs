// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Groups
{
    public class GetHierarchicalGroupResult
    {
        [JsonPropertyName("root")]
        public Group Root { get; set; }
        [JsonPropertyName("children")]
        public List<GetHierarchicalGroupResult> Children { get; set; }
        [JsonIgnore]
        public string Name
        {
            get
            {
                return Root?.Name;
            }
        }

        public GetHierarchicalGroupResult Resolve(string fullPath)
        {
            if (Root.FullPath == fullPath) return this;
            foreach(var child in Children)
            {
                var resolved = child.Resolve(fullPath);
                if (resolved == null) continue;
                return resolved;
            }

            return null;
        }

        public static GetHierarchicalGroupResult BuildRoot(IEnumerable<Group> groups, Group root)
        {
            var children = BuildChildren(groups, root.FullPath);
            return new GetHierarchicalGroupResult
            {
                Children = children,
                Root = root
            };
        }

        private static List<GetHierarchicalGroupResult> BuildChildren(IEnumerable<Group> groups, string fullPath)
        {
            var rootLevel = Group.GetLevel(fullPath);
            var nextLevel = rootLevel + 1;
            var children = groups.Where(g => g.FullPath.StartsWith(fullPath) && g.GetLevel() == nextLevel);
            var result = new List<GetHierarchicalGroupResult>();
            foreach(var child in children)
            {
                result.Add(new GetHierarchicalGroupResult
                {
                    Root = child,
                    Children = BuildChildren(groups, child.FullPath)
                });
            }

            return result;
        }
    }
}
