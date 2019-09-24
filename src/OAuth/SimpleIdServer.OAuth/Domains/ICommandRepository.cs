// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Domains
{
    public interface ICommandRepository<T> : IDisposable
    {
        bool Add(T data);
        bool Update(T data);
        Task<int> SaveChanges();
    }
}