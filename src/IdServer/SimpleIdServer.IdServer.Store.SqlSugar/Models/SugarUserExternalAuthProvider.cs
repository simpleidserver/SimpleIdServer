// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models
{
    [SugarTable("UserExternalAuthProvider")]
    public class SugarUserExternalAuthProvider
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Scheme { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public DateTime CreateDateTime { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; }

        public UserExternalAuthProvider ToDomain()
        {
            return new UserExternalAuthProvider
            {
                Scheme = Scheme,
                Subject = Subject,
                CreateDateTime = CreateDateTime
            };
        }
    }
}
