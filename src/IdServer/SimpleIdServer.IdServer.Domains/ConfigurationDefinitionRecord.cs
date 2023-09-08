// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics;

namespace SimpleIdServer.IdServer.Domains;

[DebuggerDisplay("Name {Name}, Display name {DisplayName}, Description {Description}")]
public class ConfigurationDefinitionRecord : ITranslatable
{
    private const string DISPLAY_NAME_KEY = "display_name";
    private const string DESCRIPTION_KEY = "description";

    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ConfigurationDefinitionRecordTypes Type { get; set; } = ConfigurationDefinitionRecordTypes.INPUT;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string? DisplayName
    {
        get
        {
            return GetTranslation(DisplayNames);
        }
    }
    public string? Description
    {
        get
        {
            return GetTranslation(Descriptions);
        }
    }
    public ICollection<Translation> DisplayNames
    {
        get
        {
            return Translations.Where(t => t.Key == DISPLAY_NAME_KEY).ToList();
        }
    }
    public ICollection<Translation> Descriptions
    {
        get
        {
            return Translations.Where(t => t.Key == DESCRIPTION_KEY).ToList();
        }
    }

    public ICollection<ConfigurationDefinitionRecordValue> Values { get; set; } = new List<ConfigurationDefinitionRecordValue>();

    public void SetDisplayName(string displayName, string language) => SetTranslation(DISPLAY_NAME_KEY, language, displayName);

    public void SetDescription(string description, string language) => SetTranslation(DESCRIPTION_KEY, language, description);

    public void SetValue(string name, string language, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var selectedValue = Values.SingleOrDefault(v => v.Value == value);
        if (selectedValue == null)
        {
            var record = new ConfigurationDefinitionRecordValue { Id = Guid.NewGuid().ToString(), Value = value };
            record.SetName(name, language);
            Values.Add(record);
            return;
        }

        selectedValue.SetName(name, language);
    }
}

public enum ConfigurationDefinitionRecordTypes
{
    INPUT = 0,
    SELECT = 1,
    MULTISELECT = 2,
    CHECKBOX = 3,
    NUMBER = 4,
    PASSWORD = 5,
    DATETIME = 6
}