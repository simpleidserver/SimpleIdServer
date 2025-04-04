using Microsoft.IdentityModel.Tokens;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Serializers
{
    internal class IgnoreEmptyOthConverter : JsonConverter<JsonWebKey>
    {
        public override JsonWebKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<JsonWebKey>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, JsonWebKey value, JsonSerializerOptions options)
        {
            if (value.Oth != null && value.Oth.Count == 0)
            {
                var json = JsonSerializer.Serialize(value, options);
                var jsonObject = JsonNode.Parse(json).AsObject();
                jsonObject.Remove("oth");
                jsonObject.WriteTo(writer);
            }
            else
            {
                JsonSerializer.Serialize(writer, value, options);
            }
        }
    }
}
