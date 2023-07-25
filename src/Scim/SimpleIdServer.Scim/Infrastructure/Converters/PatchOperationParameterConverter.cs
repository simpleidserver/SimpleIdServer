// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Extensions;
using System;
using System.Reflection;

namespace SimpleIdServer.Scim.Infrastructure.Converters
{
    public class PatchOperationParameterConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetTypeInfo().Equals(typeof(PatchOperationParameter).GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var result = new PatchOperationParameter
            {
                Path = json.GetStringIgnoreCase(SCIMConstants.PathOperationAttributes.Path)
            };
            if (json.TryGetEnumIgnoreCase(SCIMConstants.PathOperationAttributes.Operation, out SCIMPatchOperations op))
                result.Operation = op;
            if (json.TryGetValue(SCIMConstants.PathOperationAttributes.Value, StringComparison.InvariantCultureIgnoreCase, out JToken val))
                result.Value = val.ToCamelCase();
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
