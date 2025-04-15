// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace FormBuilder.Controllers;

[Route("fakedata")]
public class FakeDataController
{
    private readonly IEnumerable<IFakerDataService> _fakerDataServices;

    public FakeDataController(IEnumerable<IFakerDataService> fakerDataServices)
    {
        _fakerDataServices = fakerDataServices;
    }

    [HttpGet("{id}/generate")]
    public async Task<IActionResult> GenerateFakeData(string id)
    {
        var fakerDataService = _fakerDataServices.FirstOrDefault(f => f.CorrelationId == id);
        var result = new object();
        if (fakerDataService != null)
        {
            result = fakerDataService.Generate();
        }

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false
        });
        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = (int)HttpStatusCode.OK
        };
    }
}
