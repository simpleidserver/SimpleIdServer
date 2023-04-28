// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using EFCore.BulkExtensions;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class SCIMEFOptions
    {
        public string DefaultSchema { get; set; } = "dbo";

        public Dictionary<EF.BulkOperations, Action<BulkConfig>> BulkOperations { get; set; } = new Dictionary<EF.BulkOperations, Action<BulkConfig>>();

        public void SetDeleteBulkOperation(Action<BulkConfig> callback) => SetBulkOperation(EF.BulkOperations.DELETE, callback);

        public void SetUpdateBulkOperation(Action<BulkConfig> callback) => SetBulkOperation(EF.BulkOperations.UPDATE, callback);

        public void SetInsertBulkOperation(Action<BulkConfig> callback) => SetBulkOperation(EF.BulkOperations.INSERT, callback);

        private void SetBulkOperation(EF.BulkOperations operation, Action<BulkConfig> callback)
        {
            if(!BulkOperations.ContainsKey(operation)) BulkOperations.Add(operation, callback);
            else BulkOperations[operation] = callback;
        }
    }

    public enum BulkOperations
    {
        UPDATE = 0,
        DELETE = 1,
        INSERT = 2
    }
}
