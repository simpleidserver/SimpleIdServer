// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json;

namespace SimpleIdServer.IdServer.AuthFaker.Stores;

public class CredentialStorage
{
    private readonly string _path;
    private static CredentialStorage _instance;

    private CredentialStorage()
    {
        _path = Path.Combine(Directory.GetCurrentDirectory(), "credentialStore.json");
    }

    public static CredentialStorage New()
    {
        if(_instance == null) _instance = new CredentialStorage();
        return _instance;
    }

    public void Update(CredentialRecord record)
    {
        File.WriteAllText(_path, JsonSerializer.Serialize(record));
    }

    public CredentialRecord Get()
    {
        if (!File.Exists(_path)) return null;
        var content = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<CredentialRecord>(content);
    }
}
