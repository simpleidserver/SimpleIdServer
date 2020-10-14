// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Infrastructure.Lock;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.SqlServer
{
    public class SqlDistributedLock : IDistributedLock
    {
        private readonly Dictionary<string, SqlConnection> _locks = new Dictionary<string, SqlConnection>();
        private readonly AutoResetEvent _mutex = new AutoResetEvent(true);
        private readonly string _connectionString;

        public SqlDistributedLock(IOptions<SqlDistributedLockOptions> options)
        {
            var csb = new SqlConnectionStringBuilder(options.Value.ConnectionString);
            csb.Pooling = true;
            _connectionString = csb.ToString();
        }

        public async Task WaitLock(string id, CancellationToken token)
        {
            while (true)
            {
                Thread.Sleep(10);
                if (!await TryAcquireLock(id, token))
                {
                    continue;
                }

                return;
            }
        }

        public async Task<bool> TryAcquireLock(string id, CancellationToken token)
        {
            if (_mutex.WaitOne())
            {
                try
                {
                    var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync(token);
                    try
                    {
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "sp_getapplock";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Resource", id);
                        cmd.Parameters.AddWithValue("@LockOwner", $"Session");
                        cmd.Parameters.AddWithValue("@LockMode", $"Exclusive");
                        cmd.Parameters.AddWithValue("@LockTimeout", 0);
                        var returnParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;
                        await cmd.ExecuteNonQueryAsync(token);
                        var result = Convert.ToInt32(returnParameter.Value);
                        if (result >= 0)
                        {
                            _locks[id] = connection;
                            return true;
                        }
                        else
                        {
                            connection.Close();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        throw ex;
                    }
                }
                finally
                {
                    _mutex.Set();
                }
            }
            return false;
        }

        public async Task ReleaseLock(string id, CancellationToken token)
        {
            if (_mutex.WaitOne())
            {
                try
                {
                    if (!_locks.ContainsKey(id))
                    {
                        return;
                    }

                    var connection = _locks[id];
                    try
                    {
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "sp_releaseapplock";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Resource", id);
                        cmd.Parameters.AddWithValue("@LockOwner", $"Session");
                        var returnParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();
                        var result = Convert.ToInt32(returnParameter.Value);
                    }
                    finally
                    {
                        connection.Close();
                        _locks.Remove(id);
                    }
                }
                finally
                {
                    _mutex.Set();
                }
            }
        }
    }
}