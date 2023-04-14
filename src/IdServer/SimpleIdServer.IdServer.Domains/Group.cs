// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class Group
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string FullPath { get; set; } = null!;
        public string? Description { get; set; } = null;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string? ParentGroupId { get; set; } = null;
        public Group? ParentGroup { get; set; } = null;
        public ICollection<Group> Children { get; set; } = new List<Group>();
        public ICollection<Scope> Roles { get; set; } = new List<Scope>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();

        public List<string> ResolveAllPath()
        {
            var result = new List<string> { FullPath };
            var splitted = FullPath.Split('.');
            if (splitted.Length == 1) return result;
            for (var i = 1; i < splitted.Length; i++) result.Add(string.Join('.', splitted.Take(i)));
            return result;
        }
    }
}
