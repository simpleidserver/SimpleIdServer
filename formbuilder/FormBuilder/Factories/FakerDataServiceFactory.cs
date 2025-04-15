// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;

namespace FormBuilder.Factories;

public interface IFakerDataServiceFactory
{
    Task<string> Generate(string correlationId);
}

public class FakerDataServiceFactory : IFakerDataServiceFactory
{
    private readonly FormBuilderOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;

    public FakerDataServiceFactory(
        IOptions<FormBuilderOptions> options, 
        IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> Generate(string correlationId)
    {
        using (var httpClient = _httpClientFactory.CreateClient())
        {
            var result = await httpClient.GetStringAsync($"{_options.Issuer}/fakedata/{correlationId}/generate");
            return result;
        }
    }
}