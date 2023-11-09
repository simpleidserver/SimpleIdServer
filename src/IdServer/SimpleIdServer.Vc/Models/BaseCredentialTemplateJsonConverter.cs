// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc.Models
{
    public class BaseCredentialTemplateJsonConverter : JsonConverter<BaseCredentialTemplate>
    {
        public override BaseCredentialTemplate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, BaseCredentialTemplate value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (!string.IsNullOrWhiteSpace(value.Id)) writer.WriteString(CredentialTemplateNames.Id, value.Id);
            if (!string.IsNullOrWhiteSpace(value.Format)) writer.WriteString(CredentialTemplateNames.Format, value.Format);
            if (value.DisplayLst != null && value.DisplayLst.Any())
            {
                writer.WriteStartArray(CredentialTemplateNames.Display);
                foreach (var display in value.DisplayLst)
                {
                    writer.WriteStartObject();
                    if (!string.IsNullOrWhiteSpace(display.Name)) writer.WriteString(CredentialTemplateDisplayNames.Name, display.Name);
                    if (!string.IsNullOrWhiteSpace(display.Locale)) writer.WriteString(CredentialTemplateDisplayNames.Locale, display.Locale);
                    if (!string.IsNullOrWhiteSpace(display.LogoUrl) && !string.IsNullOrWhiteSpace(display.LogoAltText))
                    {
                        writer.WriteStartObject(CredentialTemplateDisplayNames.Logo);
                        writer.WriteString(CredentialTemplateDisplayNames.Url, display.LogoUrl);
                        writer.WriteString(CredentialTemplateDisplayNames.AltText, display.LogoAltText);
                        writer.WriteEndObject();
                    }

                    if (!string.IsNullOrWhiteSpace(display.Description)) writer.WriteString(CredentialTemplateDisplayNames.Description, display.Description);
                    if (!string.IsNullOrWhiteSpace(display.BackgroundColor)) writer.WriteString(CredentialTemplateDisplayNames.BackgroundColor, display.BackgroundColor);
                    if (!string.IsNullOrWhiteSpace(display.TextColor)) writer.WriteString(CredentialTemplateDisplayNames.TextColor, display.TextColor);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }

            if (value.Parameters.Any() && value.Parameters != null)
            {
                foreach (var grp in value.Parameters.GroupBy(kvp => kvp.Name)) Write(grp.Select(kvp => kvp));
            }

            writer.WriteEndObject();

            void Write(IEnumerable<CredentialTemplateParameter> parameters)
            {
                var firstParameter = parameters.First();
                writer.WritePropertyName(firstParameter.Name);
                if(firstParameter.IsArray) writer.WriteStartArray();
                foreach(var parameter in parameters)
                {
                    switch(firstParameter.ParameterType)
                    {
                        case CredentialTemplateParameterTypes.STRING:
                            writer.WriteStringValue(parameter.Value); 
                            break;
                        case CredentialTemplateParameterTypes.JSON:
                            writer.WriteRawValue(parameter.Value);
                            break;
                    }
                }
                if (firstParameter.IsArray) writer.WriteEndArray();
            }
        }
    }
}
