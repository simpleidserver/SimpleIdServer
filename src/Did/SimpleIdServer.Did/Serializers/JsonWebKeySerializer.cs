using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SimpleIdServer.Did.Serializers;

public class JsonWebKeySerializer
{
    public static string Write(JsonWebKey jsonWebKey)
    {
        using MemoryStream memoryStream = new MemoryStream();
        Utf8JsonWriter writer = null;
        try
        {
            writer = new Utf8JsonWriter(memoryStream, new JsonWriterOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            Write(ref writer, jsonWebKey);
            writer.Flush();
            return System.Text.Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }
        finally
        {
            writer?.Dispose();
        }
    }

    public static void Write(ref Utf8JsonWriter writer, JsonWebKey jsonWebKey)
    {
        if (jsonWebKey == null)
        {
            throw new ArgumentNullException("jsonWebKey");
        }

        if (writer == null)
        {
            throw new ArgumentNullException("writer");
        }

        writer.WriteStartObject();
        if (!string.IsNullOrEmpty(jsonWebKey.Alg))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.Alg, jsonWebKey.Alg);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.Crv))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.Crv, jsonWebKey.Crv);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.D))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.D, jsonWebKey.D);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.DP))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.DP, jsonWebKey.DP);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.DQ))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.DQ, jsonWebKey.DQ);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.E))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.E, jsonWebKey.E);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.K))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.K, jsonWebKey.K);
        }

        if (jsonWebKey.KeyOps.Count > 0)
        {
            WriteStrings(ref writer, JsonWebKeyParameterUtf8Bytes.KeyOps, jsonWebKey.KeyOps);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.Kid))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.Kid, jsonWebKey.Kid);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.Kty))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.Kty, jsonWebKey.Kty);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.N))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.N, jsonWebKey.N);
        }

        if (jsonWebKey.Oth.Count > 0)
        {
            WriteStrings(ref writer, JsonWebKeyParameterUtf8Bytes.Oth, jsonWebKey.Oth);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.P))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.P, jsonWebKey.P);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.Q))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.Q, jsonWebKey.Q);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.QI))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.QI, jsonWebKey.QI);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.Use))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.Use, jsonWebKey.Use);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.X))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.X, jsonWebKey.X);
        }

        if (jsonWebKey.X5c.Count > 0)
        {
            WriteStrings(ref writer, JsonWebKeyParameterUtf8Bytes.X5c, jsonWebKey.X5c);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.X5t))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.X5t, jsonWebKey.X5t);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.X5tS256))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.X5tS256, jsonWebKey.X5tS256);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.X5u))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.X5u, jsonWebKey.X5u);
        }

        if (!string.IsNullOrEmpty(jsonWebKey.Y))
        {
            writer.WriteString(JsonWebKeyParameterUtf8Bytes.Y, jsonWebKey.Y);
        }

        WriteObjects(ref writer, jsonWebKey.AdditionalData);
        writer.WriteEndObject();
    }

    public static void WriteStrings(ref Utf8JsonWriter writer, ReadOnlySpan<byte> propertyName, IList<string> strings)
    {
        writer.WriteStartArray(propertyName);
        foreach (string @string in strings)
        {
            writer.WriteStringValue(@string);
        }

        writer.WriteEndArray();
    }

    public static void WriteObjects(ref Utf8JsonWriter writer, IDictionary<string, object> dictionary)
    {
        if (dictionary == null || dictionary.Count <= 0)
        {
            return;
        }

        foreach (KeyValuePair<string, object> item in dictionary)
        {
            WriteObject(ref writer, item.Key, item.Value);
        }
    }
    public static void WriteObject(ref Utf8JsonWriter writer, string key, object obj)
    {
        if (writer.CurrentDepth >= 64)
        {
            throw new InvalidOperationException(LogHelper.FormatInvariant("IDX10815: Depth of JSON: '{0}' exceeds max depth of '{1}'.", LogHelper.MarkAsNonPII(writer.CurrentDepth), LogHelper.MarkAsNonPII(64)));
        }

        if (obj == null)
        {
            writer.WriteNull(key);
            return;
        }

        Type type = obj.GetType();
        if (obj is string value)
        {
            writer.WriteString(key, value);
        }
        else if (obj is long value2)
        {
            writer.WriteNumber(key, value2);
        }
        else if (obj is int value3)
        {
            writer.WriteNumber(key, value3);
        }
        else if (obj is bool value4)
        {
            writer.WriteBoolean(key, value4);
        }
        else if (obj is DateTime dateTime)
        {
            writer.WriteString(key, dateTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
        }
        else if (typeof(IDictionary).IsAssignableFrom(type))
        {
            IDictionary dictionary = (IDictionary)obj;
            writer.WritePropertyName(key);
            writer.WriteStartObject();
            foreach (object key2 in dictionary.Keys)
            {
                WriteObject(ref writer, key2.ToString(), dictionary[key2]);
            }

            writer.WriteEndObject();
        }
        else if (typeof(IList).IsAssignableFrom(type))
        {
            IList obj2 = (IList)obj;
            writer.WriteStartArray(key);
            foreach (object item in obj2)
            {
                WriteObjectValue(ref writer, item);
            }

            writer.WriteEndArray();
        }
        else if (obj is JsonElement jsonElement)
        {
            writer.WritePropertyName(key);
            jsonElement.WriteTo(writer);
        }
        else if (obj is double value5)
        {
            writer.WriteNumber(key, value5);
        }
        else if (obj is decimal value6)
        {
            writer.WriteNumber(key, value6);
        }
        else if (obj is float value7)
        {
            writer.WriteNumber(key, value7);
        }
        else
        {
            if (!(obj is Guid value8))
            {
                throw LogHelper.LogExceptionMessage(new ArgumentException(LogHelper.FormatInvariant("IDX11025: Cannot serialize object of type: '{0}' into property: '{1}'.", LogHelper.MarkAsNonPII(type.ToString()), LogHelper.MarkAsNonPII(key))));
            }

            writer.WriteString(key, value8);
        }
    }
    public static void WriteObjectValue(ref Utf8JsonWriter writer, object obj)
    {
        if (writer.CurrentDepth >= 64)
        {
            throw new InvalidOperationException(LogHelper.FormatInvariant("IDX10815: Depth of JSON: '{0}' exceeds max depth of '{1}'.", LogHelper.MarkAsNonPII(writer.CurrentDepth), LogHelper.MarkAsNonPII(64)));
        }

        if (obj == null)
        {
            writer.WriteNullValue();
            return;
        }

        Type type = obj.GetType();
        if (obj is string value)
        {
            writer.WriteStringValue(value);
        }
        else if (obj is DateTime dateTime)
        {
            writer.WriteStringValue(dateTime.ToUniversalTime());
        }
        else if (obj is int value2)
        {
            writer.WriteNumberValue(value2);
        }
        else if (obj is bool value3)
        {
            writer.WriteBooleanValue(value3);
        }
        else if (obj is long value4)
        {
            writer.WriteNumberValue(value4);
        }
        else if (obj == null)
        {
            writer.WriteNullValue();
        }
        else if (obj is double value5)
        {
            writer.WriteNumberValue(value5);
        }
        else if (obj is JsonElement jsonElement)
        {
            jsonElement.WriteTo(writer);
        }
        else if (typeof(IDictionary).IsAssignableFrom(type))
        {
            IDictionary dictionary = (IDictionary)obj;
            writer.WriteStartObject();
            foreach (object key in dictionary.Keys)
            {
                WriteObject(ref writer, key.ToString(), dictionary[key]);
            }

            writer.WriteEndObject();
        }
        else if (typeof(IList).IsAssignableFrom(type))
        {
            IList obj2 = (IList)obj;
            writer.WriteStartArray();
            foreach (object item in obj2)
            {
                WriteObjectValue(ref writer, item);
            }

            writer.WriteEndArray();
        }
        else if (obj is decimal value6)
        {
            writer.WriteNumberValue(value6);
        }
        else if (obj is float value7)
        {
            writer.WriteNumberValue(value7);
        }
        else
        {
            writer.WriteStringValue(obj.ToString());
        }
    }
}
