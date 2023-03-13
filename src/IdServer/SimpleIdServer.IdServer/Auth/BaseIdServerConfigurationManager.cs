// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Auth
{
    public abstract class BaseIdServerConfigurationManager<T> : IConfigurationManager<T> where T : class
    {
        private readonly string _metadataAddress;
        private readonly IConfigurationRetriever<T> _configRetriever;
        private readonly IDocumentRetriever _documentRetriever;
        private readonly List<StoredConfiguration<T>> _storedConfigurations = new List<StoredConfiguration<T>>();
        private readonly SemaphoreSlim _refreshLock;

        public BaseIdServerConfigurationManager(string metadataAddress, IConfigurationRetriever<T> configRetriever, IDocumentRetriever documentRetriever)
        {
            _metadataAddress = metadataAddress;
            _configRetriever = configRetriever;
            _documentRetriever = documentRetriever;
            _refreshLock = new SemaphoreSlim(1);
        }

        protected string MetadataAddress => _metadataAddress;
        public TimeSpan AutomaticRefreshInterval { get; set; } = new TimeSpan(0, 12, 0, 0);
        public TimeSpan RefreshInterval { get; set; } = new TimeSpan(0, 0, 5, 0);

        public async Task<T> GetConfigurationAsync(CancellationToken cancel)
        {
            var now = DateTimeOffset.UtcNow;
            var address = GetAddress();
            var record = _storedConfigurations.FirstOrDefault(c => c.MetadataAddress == address);
            if (record != null && record.SyncAfter > now)
            {
                return record.Configuration;
            }

            if (record == null)
            {
                record = new StoredConfiguration<T> { MetadataAddress = address };
                _storedConfigurations.Add(record);
            }

            await _refreshLock.WaitAsync(cancel).ConfigureAwait(false);
            try
            {
                if (record.SyncAfter <= now)
                {
                    try
                    {
                        var configuration = await _configRetriever.GetConfigurationAsync(address, _documentRetriever, CancellationToken.None).ConfigureAwait(false);
                        record.LastRefresh = now;
                        record.SyncAfter = DateTimeUtil.Add(now.UtcDateTime, AutomaticRefreshInterval);
                        record.Configuration = configuration;
                    }
                    catch (Exception ex)
                    {
                        record.SyncAfter = DateTimeUtil.Add(now.UtcDateTime, AutomaticRefreshInterval < RefreshInterval ? AutomaticRefreshInterval : RefreshInterval);
                    }
                }

                // Stale metadata is better than no metadata
                if (record.Configuration != null)
                    return record.Configuration;
            }
            finally
            {
                _refreshLock.Release();
            }

            return null;
        }

        public void RequestRefresh()
        {
            var address = GetAddress();
            var record = _storedConfigurations.SingleOrDefault(r => r.MetadataAddress == address);
            if (record != null)
            {
                record.SyncAfter = DateTimeOffset.Now;
            }
        }

        protected abstract string GetAddress();

        private class StoredConfiguration<T>
        {
            public T Configuration { get; set; }
            public string MetadataAddress { get; set; }
            public DateTimeOffset SyncAfter { get; set; } = DateTimeOffset.MinValue;
            public DateTimeOffset LastRefresh { get; set; } = DateTimeOffset.MinValue;
        }
    }
}
