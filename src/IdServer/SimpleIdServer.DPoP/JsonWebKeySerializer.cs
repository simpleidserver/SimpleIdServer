using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SimpleIdServer.DPoP
{
    public class JsonWebKeySerializer
    {
        public static string Write(JsonWebKey jsonWebKey)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Utf8JsonWriter writer = null;
                try
                {
                    writer = new Utf8JsonWriter(memoryStream, new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                    Write(ref writer, jsonWebKey);
                    writer.Flush();

                    return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                }
                finally
                {
                    writer?.Dispose();
                }
            }
        }

        public static void Write(ref Utf8JsonWriter writer, JsonWebKey jsonWebKey)
        {
            _ = jsonWebKey ?? throw new ArgumentNullException(nameof(jsonWebKey));
            _ = writer ?? throw new ArgumentNullException(nameof(writer));

            writer.WriteStartObject();

            if (!string.IsNullOrEmpty(jsonWebKey.Alg))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.Alg, jsonWebKey.Alg);

            if (!string.IsNullOrEmpty(jsonWebKey.Crv))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.Crv, jsonWebKey.Crv);

            if (!string.IsNullOrEmpty(jsonWebKey.D))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.D, jsonWebKey.D);

            if (!string.IsNullOrEmpty(jsonWebKey.DP))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.DP, jsonWebKey.DP);

            if (!string.IsNullOrEmpty(jsonWebKey.DQ))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.DQ, jsonWebKey.DQ);

            if (!string.IsNullOrEmpty(jsonWebKey.E))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.E, jsonWebKey.E);

            if (!string.IsNullOrEmpty(jsonWebKey.K))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.K, jsonWebKey.K);

            if (jsonWebKey.KeyOps.Count > 0)
                JsonSerializerPrimitives.WriteStrings(ref writer, JsonWebKeyParameterUtf8Bytes.KeyOps, jsonWebKey.KeyOps);

            if (!string.IsNullOrEmpty(jsonWebKey.Kid))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.Kid, jsonWebKey.Kid);

            if (!string.IsNullOrEmpty(jsonWebKey.Kty))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.Kty, jsonWebKey.Kty);

            if (!string.IsNullOrEmpty(jsonWebKey.N))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.N, jsonWebKey.N);

            if (jsonWebKey.Oth.Count > 0)
                JsonSerializerPrimitives.WriteStrings(ref writer, JsonWebKeyParameterUtf8Bytes.Oth, jsonWebKey.Oth);

            if (!string.IsNullOrEmpty(jsonWebKey.P))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.P, jsonWebKey.P);

            if (!string.IsNullOrEmpty(jsonWebKey.Q))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.Q, jsonWebKey.Q);

            if (!string.IsNullOrEmpty(jsonWebKey.QI))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.QI, jsonWebKey.QI);

            if (!string.IsNullOrEmpty(jsonWebKey.Use))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.Use, jsonWebKey.Use);

            if (!string.IsNullOrEmpty(jsonWebKey.X))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.X, jsonWebKey.X);

            if (jsonWebKey.X5c.Count > 0)
                JsonSerializerPrimitives.WriteStrings(ref writer, JsonWebKeyParameterUtf8Bytes.X5c, jsonWebKey.X5c);

            if (!string.IsNullOrEmpty(jsonWebKey.X5t))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.X5t, jsonWebKey.X5t);

            if (!string.IsNullOrEmpty(jsonWebKey.X5tS256))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.X5tS256, jsonWebKey.X5tS256);

            if (!string.IsNullOrEmpty(jsonWebKey.X5u))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.X5u, jsonWebKey.X5u);

            if (!string.IsNullOrEmpty(jsonWebKey.Y))
                writer.WriteString(JsonWebKeyParameterUtf8Bytes.Y, jsonWebKey.Y);

            JsonSerializerPrimitives.WriteObjects(ref writer, jsonWebKey.AdditionalData);

            writer.WriteEndObject();
        }
    }
}
