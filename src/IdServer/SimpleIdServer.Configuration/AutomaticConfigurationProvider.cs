// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Middlewares;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Configuration;

public class AutomaticConfigurationProvider : IConfigurationProvider, IDisposable
{
    private bool _isDisposed = false;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly AutomaticConfigurationOptions _options;
    private readonly IKeyValueConnector _connector;
    private readonly int _refreshIntervalInSeconds = 10;
    private Task? _pollTask;
    private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

    public AutomaticConfigurationProvider(AutomaticConfigurationOptions options, IKeyValueConnector connector)
    {
        _options = options;
        _connector = connector;
        Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
    }

    protected IDictionary<string, string?> Data { get; set; }

    public virtual bool TryGet(string key, out string? value)
    {
        value = null;
        if (string.IsNullOrWhiteSpace(RealmContext.Instance().Realm)) return false;
        var record = _options.ConfigurationDefinitions.SingleOrDefault(d => key.Contains(d.Name));
        if (record == null) return false;
        key = $"{RealmContext.Instance().Realm}:{key}";
        var result = Data.TryGetValue(key, out value);
        return result;
    }

    public virtual void Set(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(RealmContext.Instance().Realm)) return;
        var record = _options.ConfigurationDefinitions.SingleOrDefault(d => key.Contains(d.Name));
        if (record == null) return;
        key = $"{RealmContext.Instance().Realm}:{key}";
        _connector.Set(key, value, CancellationToken.None).Wait();
        if (!Data.ContainsKey(key)) Data.Add(key, value);
        else Data[key] = value;
    }

    public virtual void Load()
    {
        if (_pollTask != null) return;
        var cancellationToken = _cancellationTokenSource.Token;
        LoadConfigurations(cancellationToken).Wait();
        _pollTask = Task.Run(async () =>
        {
            await PoolConfigurations(cancellationToken);
        });
    }

    public virtual IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
    {
        var results = new List<string>();

        if (parentPath is null)
        {
            foreach (KeyValuePair<string, string?> kv in Data)
            {
                results.Add(Segment(kv.Key, 0));
            }
        }
        else
        {
            var realm = RealmContext.Instance().Realm;
            if(!string.IsNullOrWhiteSpace(realm)) parentPath = $"{realm}:{parentPath}";
            Debug.Assert(ConfigurationPath.KeyDelimiter == ":");

            foreach (KeyValuePair<string, string?> kv in Data)
            {
                if (kv.Key.Length > parentPath.Length &&
                    kv.Key.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase) &&
                    kv.Key[parentPath.Length] == ':')
                {
                    results.Add(Segment(kv.Key, parentPath.Length + 1));
                }
            }
        }

        results.AddRange(earlierKeys);

        results.Sort();

        return results;
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _isDisposed = true;
    }

    private static string Segment(string key, int prefixLength)
    {
        int indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
        return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
    }

    private async Task PoolConfigurations(CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            await LoadConfigurations(cancellationToken);
            await Task.Delay(_refreshIntervalInSeconds * 1000);
        }
    }

    private async Task LoadConfigurations(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _connector.GetAll(cancellationToken);
            var newData = result;
            var isSimilar = newData.Count() == Data.Count() && newData.All(d => Data.Any(e => e.Key == d.Key && e.Value == d.Value));
            if (!isSimilar)
            {
                OnReload();
                Data = newData;
            }
        }
        catch(Exception ex)
        {

        }
    }

    public IChangeToken GetReloadToken()
    {
        return _reloadToken;
    }

    protected void OnReload()
    {
        ConfigurationReloadToken previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
        previousToken.OnReload();
    }

    public override string ToString() => $"{GetType().Name}";
}
