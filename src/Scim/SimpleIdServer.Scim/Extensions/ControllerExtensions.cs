// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Serialization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Scim.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult BuildError(this Controller controller, HttpStatusCode code, string detail, string scimType = null)
        {
            var serializer = new SCIMSerializer();
            var result = serializer.Serialize(new SCIMErrorRepresentation(((int)code).ToString(), detail, scimType));
            return new ContentResult
            {
                StatusCode = (int)code,
                Content = result.ToString(),
                ContentType = "application/json"
            };
        }

        public static IActionResult BuildError<T>(this Controller controller, GenericResult<T> result)
        {
            var serializer = new SCIMSerializer();
            var code = result.StatusCode;
            var res = serializer.Serialize(new SCIMErrorRepresentation(((int)code).ToString(), result.ErrorMessage, result.ScimType));
            return new ContentResult
            {
                StatusCode = (int)code,
                Content = res.ToString(),
                ContentType = "application/json"
            };
        }

        public static JsonObject SerializeQuery(this Controller controller)
        {
            var query = controller.Request.Query;
            var result = new JsonObject();
            foreach(var record in query)
            {
                result.Add(record.Key, JsonObject.Parse(JsonSerializer.Serialize(record)));
            }

            return result;
        }
    }
}