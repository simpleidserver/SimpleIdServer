// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class Group
    {
        [JsonPropertyName(GroupNames.Id)]
        public string Id { get; set; } = null!;
        [JsonPropertyName(GroupNames.Source)]
        public string? Source { get; set; } = null!;
        [JsonPropertyName(GroupNames.Name)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(GroupNames.FullPath)]
        public string FullPath { get; set; } = null!;
        [JsonPropertyName(GroupNames.Description)]
        public string? Description { get; set; } = null;
        [JsonPropertyName(GroupNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(GroupNames.UpdateDateTime)]
        public DateTime UpdateDateTime { get; set; }
        [JsonPropertyName(GroupNames.ParentGroupId)]
        public string? ParentGroupId { get; set; } = null;
        [JsonIgnore]
        public Group? ParentGroup { get; set; } = null;
        [JsonPropertyName(GroupNames.Children)]
        public ICollection<Group> Children { get; set; } = new List<Group>();
        [JsonPropertyName(GroupNames.Roles)]
        public ICollection<Scope> Roles { get; set; } = new List<Scope>();
        [JsonIgnore]
        public ICollection<GroupUser> Users { get; set; } = new List<GroupUser>();
        [JsonIgnore]
        public ICollection<GroupRealm> Realms { get; set; } = new List<GroupRealm>();

        public List<string> ResolveAllPath()
        {
            var result = new List<string> { FullPath };
            var splitted = Split(FullPath);
            if (splitted.Count() == 1) return result;
            for (var i = 1; i < splitted.Count(); i++) result.Add(string.Join('.', splitted.Take(i)));
            return result;
        }

        public int GetLevel()
            => GetLevel(FullPath);

        public static int GetLevel(string fullPath)
            => Split(fullPath).Count();

        public static IEnumerable<string> Split(string fullPath)
            => fullPath.Split('.');
    }
}
