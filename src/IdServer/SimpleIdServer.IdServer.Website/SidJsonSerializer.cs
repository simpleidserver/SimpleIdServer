// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Website;

public class SidJsonSerializer
{
    public static T Deserialize<T>(string json) where T : class
    {
        var serializeOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new UtcDateTimeConverter()
            }
        };
        var searchResult = JsonSerializer.Deserialize<T>(json, serializeOptions);
        return searchResult;
    }

    public static SearchResult<Client> DeserializeSearchClients(string json)
    {
        var result = Deserialize<SearchResult<Client>>(json);
        foreach(var client in result.Content)
        {
            client.CreateDateTime = client.CreateDateTime.ToLocalTime();
            client.UpdateDateTime = client.UpdateDateTime.ToLocalTime();
        }

        return result;
    }

    public static IEnumerable<Client> DeserializeClients(string json)
    {
        var result = Deserialize<IEnumerable<Client>>(json);
        foreach (var client in result)
        {
            client.CreateDateTime = client.CreateDateTime.ToLocalTime();
            client.UpdateDateTime = client.UpdateDateTime.ToLocalTime();
        }

        return result;
    }
}