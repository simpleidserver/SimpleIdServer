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
        [Navigate(NavigateType.ManyToOne, nameof(UserId))]
        public SugarUser User { get; set; }

        public static SugarUserExternalAuthProvider Transform(UserExternalAuthProvider e)
        {
            return new SugarUserExternalAuthProvider
            {
                CreateDateTime = e.CreateDateTime,
                Subject = e.Subject,
                Scheme = e.Scheme
            };
        }

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
