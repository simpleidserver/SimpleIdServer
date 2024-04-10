using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Collections;
using System.Globalization;

namespace SimpleIdServer.DPoP;

public class JsonSerializerPrimitives
{
    const int MaxDepth = 64;

    public static void WriteStrings(ref Utf8JsonWriter writer, ReadOnlySpan<byte> propertyName, IList<string> strings)
    {
        writer.WriteStartArray(propertyName);
        foreach (string str in strings)
            writer.WriteStringValue(str);

        writer.WriteEndArray();
    }

    public static void WriteObjects(ref Utf8JsonWriter writer, IDictionary<string, object> dictionary)
    {
        if (dictionary?.Count > 0)
            foreach (KeyValuePair<string, object> kvp in dictionary)
                WriteObject(ref writer, kvp.Key, kvp.Value);
    }
    public static void WriteObject(ref Utf8JsonWriter writer, string key, object obj)
    {
        if (writer.CurrentDepth >= MaxDepth)
            throw new InvalidOperationException("");

        if (obj is null)
        {
            writer.WriteNull(key);
            return;
        }

        Type objType = obj.GetType();

        if (obj is string str)
            writer.WriteString(key, str);
        else if (obj is long l)
            writer.WriteNumber(key, l);
        else if (obj is int i)
            writer.WriteNumber(key, i);
        else if (obj is bool b)
            writer.WriteBoolean(key, b);
        else if (obj is DateTime dt)
            writer.WriteString(key, dt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
        else if (typeof(IDictionary).IsAssignableFrom(objType))
        {
            IDictionary dictionary = (IDictionary)obj;
            writer.WritePropertyName(key);

            writer.WriteStartObject();
            foreach (var k in dictionary.Keys)
                WriteObject(ref writer, k.ToString(), dictionary[k]);

            writer.WriteEndObject();
        }
        else if (typeof(IList).IsAssignableFrom(objType))
        {
            IList list = (IList)obj;
            writer.WriteStartArray(key);
            foreach (var k in list)
                WriteObjectValue(ref writer, k);

            writer.WriteEndArray();
        }
        else if (obj is JsonElement j)
        {
            writer.WritePropertyName(key);
            j.WriteTo(writer);
        }
        else if (obj is double dub)
            // Below net6.0, we have to convert the double to a decimal otherwise values like 1.11 will be serailized as 1.1100000000000001
            // large and small values such as double.MaxValue and double.MinValue cannot be converted to decimal.
            // In these cases, we will write the double as is.
#if NET6_0_OR_GREATER
            writer.WriteNumber(key, dub);
#else
#pragma warning disable CA1031 // Do not catch general exception types, we have seen TryParse fault.
                try
                {
                    if (decimal.TryParse(dub.ToString(CultureInfo.InvariantCulture), out decimal dec))
                        writer.WriteNumber(key, dec);
                    else
                        writer.WriteNumber(key, dub);
                }
                catch (Exception)
                {
                    writer.WriteNumber(key, dub);
                }
#pragma warning restore CA1031
#endif
        else if (obj is decimal d)
            writer.WriteNumber(key, d);
        else if (obj is float f)
            // Below net6.0, we have to convert the float to a decimal otherwise values like 1.11 will be serailized as 1.11000001
            // In failure cases, we will write the float as is.
#if NET6_0_OR_GREATER
            writer.WriteNumber(key, f);
#else
#pragma warning disable CA1031 // Do not catch general exception types, we have seen TryParse fault.
                try
                {
                    if (decimal.TryParse(f.ToString(CultureInfo.InvariantCulture), out decimal dec))
                        writer.WriteNumber(key, dec);
                    else
                        writer.WriteNumber(key, f);
                }
                catch (Exception)
                {
                    writer.WriteNumber(key, f);
                }
#pragma warning restore CA1031
#endif
        else if (obj is Guid g)
            writer.WriteString(key, g);
    }
    public static void WriteObjectValue(ref Utf8JsonWriter writer, object obj)
    {
        if (writer.CurrentDepth >= MaxDepth)
            throw new InvalidOperationException("");

        if (obj is null)
        {
            writer.WriteNullValue();
            return;
        }

        Type objType = obj.GetType();

        if (obj is string str)
            writer.WriteStringValue(str);
        else if (obj is DateTime dt)
            writer.WriteStringValue(dt.ToUniversalTime());
        else if (obj is int i)
            writer.WriteNumberValue(i);
        else if (obj is bool b)
            writer.WriteBooleanValue(b);
        else if (obj is long l)
            writer.WriteNumberValue(l);
        else if (obj is null)
            writer.WriteNullValue();
        else if (obj is double dub)
            // Below net6.0, we have to convert the double to a decimal otherwise values like 1.11 will be serailized as 1.1100000000000001
            // large and small values such as double.MaxValue and double.MinValue cannot be converted to decimal.
            // In these cases, we will write the double as is.
#if NET6_0_OR_GREATER
            writer.WriteNumberValue(dub);
#else
#pragma warning disable CA1031 // Do not catch general exception types, we have seen TryParse fault.
                try
                {
                    if (decimal.TryParse(dub.ToString(CultureInfo.InvariantCulture), out decimal dec))
                        writer.WriteNumberValue(dec);
                    else
                        writer.WriteNumberValue(dub);
                }
                catch (Exception)
                {
                    writer.WriteNumberValue(dub);
                }
#pragma warning restore CA1031
#endif
        else if (obj is JsonElement j)
            j.WriteTo(writer);
        else if (typeof(IDictionary).IsAssignableFrom(objType))
        {
            IDictionary dictionary = (IDictionary)obj;
            writer.WriteStartObject();
            foreach (var k in dictionary.Keys)
                WriteObject(ref writer, k.ToString(), dictionary[k]);

            writer.WriteEndObject();
        }
        else if (typeof(IList).IsAssignableFrom(objType))
        {
            IList list = (IList)obj;
            writer.WriteStartArray();
            foreach (var k in list)
                WriteObjectValue(ref writer, k);

            writer.WriteEndArray();
        }
        else if (obj is decimal d)
            writer.WriteNumberValue(d);
        else if (obj is float f)
            // Below net6.0, we have to convert the float to a decimal otherwise values like 1.11 will be serailized as 1.11000001
            // In failure cases, we will write the float as is.
#if NET6_0_OR_GREATER
            writer.WriteNumberValue(f);
#else
#pragma warning disable CA1031 // Do not catch general exception types, we have seen TryParse fault.
            try
            {
                if (decimal.TryParse(f.ToString(CultureInfo.InvariantCulture), out decimal dec))
                    writer.WriteNumberValue(dec);
                else
                    writer.WriteNumberValue(f);
            }
            catch (Exception)
            {
                writer.WriteNumberValue(f);
            }
#pragma warning restore CA1031
#endif

        else
            writer.WriteStringValue(obj.ToString());
    }
}
