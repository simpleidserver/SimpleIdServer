// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Auth
{

    public class IdServerConfigurationManager : IConfigurationManager<OpenIdConnectConfiguration>
    {
        private readonly string _metadataAddress;
        private readonly IConfigurationRetriever<OpenIdConnectConfiguration> _configRetriever;
        private readonly IDocumentRetriever _documentRetriever;
        private readonly List<StoredOpenIdConfiguration> _storedOpenIdConfigurations = new List<StoredOpenIdConfiguration>();
        private readonly SemaphoreSlim _refreshLock;

        public IdServerConfigurationManager(string metadataAddress, IConfigurationRetriever<OpenIdConnectConfiguration> configRetriever, IDocumentRetriever documentRetriever)
        {
            _metadataAddress = metadataAddress;
            _configRetriever = configRetriever;
            _documentRetriever = documentRetriever;
            _refreshLock = new SemaphoreSlim(1);
        }

        public TimeSpan AutomaticRefreshInterval { get; set; } = new TimeSpan(0, 12, 0, 0);
        public TimeSpan RefreshInterval { get; set; } = new TimeSpan(0, 0, 5, 0);

        public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
        {
            var now = DateTimeOffset.UtcNow;
            var address = GetAddress();
            var record = _storedOpenIdConfigurations.FirstOrDefault(c => c.MetadataAddress == address);
            if (record != null && record.SyncAfter > now)
            {
                return record.Configuration;
            }

            if (record == null)
            {
                record = new StoredOpenIdConfiguration { MetadataAddress = address };
                _storedOpenIdConfigurations.Add(record);
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
            var record = _storedOpenIdConfigurations.SingleOrDefault(r => r.MetadataAddress == address);
            if (record != null)
            {
                record.SyncAfter = DateTimeOffset.Now;
            }
        }

        private string GetAddress()
        {
            var address = _metadataAddress;
            if (!address.EndsWith("/"))
                address += "/";

            if (!string.IsNullOrWhiteSpace(RealmContext.Instance().Realm))
                address += RealmContext.Instance().Realm + "/";

            address += ".well-known/openid-configuration";
            return address;
        }

        private class StoredOpenIdConfiguration
        {
            public OpenIdConnectConfiguration Configuration { get; set; }
            public string MetadataAddress { get; set; }
            public DateTimeOffset SyncAfter { get; set; } = DateTimeOffset.MinValue;
            public DateTimeOffset LastRefresh { get; set; } = DateTimeOffset.MinValue;
        }
    }
}
