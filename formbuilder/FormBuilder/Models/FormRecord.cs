// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FormBuilder.Models;

public class FormRecord : BaseVersionRecord, ICloneable, IFormRecordCollection
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Category { get; set; }
    public string? Realm { get; set; }
    public bool ActAsStep { get; set; }
    public List<FormMessageTranslation> SuccessMessageTranslations
    {
        get; set;
    } = new List<FormMessageTranslation>();
    public List<FormMessageTranslation> ErrorMessageTranslations
    {
        get; set;
    } = new List<FormMessageTranslation>();
    public ObservableCollection<IFormElementRecord> Elements { get; set; } = new ObservableCollection<IFormElementRecord>();

    public object Clone()
    {
        var elements = Elements == null ? 
            new ObservableCollection<IFormElementRecord>() : 
            new ObservableCollection<IFormElementRecord>(Elements.Select(e => JsonSerializer.Deserialize<IFormElementRecord>(JsonSerializer.Serialize(e))).ToList());
        return new FormRecord
        {
            Id = Id,
            ActAsStep = ActAsStep,
            Category = Category,
            CorrelationId = CorrelationId,
            Name = Name,
            Realm = Realm,
            Status = Status,
            UpdateDateTime = UpdateDateTime,
            VersionNumber = VersionNumber,
            Elements = elements,
            SuccessMessageTranslations = SuccessMessageTranslations.Select(s => new FormMessageTranslation
            {
                Code = s.Code,
                Language = s.Language,
                Value = s.Value
            }).ToList(),
            ErrorMessageTranslations = ErrorMessageTranslations.Select(s => new FormMessageTranslation
            {
                Code = s.Code,
                Language = s.Language,
                Value = s.Value
            }).ToList()
        };
    }

    public void Update(
        List<IFormElementRecord> elements, 
        List<FormMessageTranslation> successMessageTranslations, 
        List<FormMessageTranslation> errorMessageTranslations,
        DateTime dateTime)
    {
        UpdateDateTime = dateTime;
        SuccessMessageTranslations = successMessageTranslations;
        ErrorMessageTranslations = errorMessageTranslations;
        Elements = new ObservableCollection<IFormElementRecord>(elements);
    }

    public IFormElementRecord GetChild(string id)
    {
        var result = Elements.SingleOrDefault(e => e.Id == id);
        if (result != null) return result;
        foreach (var elt in Elements)
        {
            var child = elt.GetChild(id);
            if (child != null) return child;
        }

        return null;
    }

    public IFormElementRecord GetChildByCorrelationId(string correlationId)
    {
        var result = Elements.SingleOrDefault(e => e.CorrelationId == correlationId);
        if (result != null) return result;
        foreach(var elt in Elements)
        {
            var child = elt.GetChildByCorrelationId(correlationId);
            if (child != null) return child;
        }

        return null;
    }

    public override BaseVersionRecord NewDraft(DateTime currentDateTime)
    {
        var clonedElements = JsonSerializer.Deserialize<List<IFormElementRecord>>(JsonSerializer.Serialize(Elements));
        clonedElements.ForEach(c => c.Id = Guid.NewGuid().ToString());
        return new FormRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = CorrelationId,
            Name = Name,
            Category = Category,
            ActAsStep = ActAsStep,
            UpdateDateTime = currentDateTime,
            Status = RecordVersionStatus.Draft,
            Realm = Realm,
            VersionNumber = VersionNumber + 1,
            Elements = new ObservableCollection<IFormElementRecord>(clonedElements),
            SuccessMessageTranslations = SuccessMessageTranslations?.Select(s => new FormMessageTranslation
            {
                Code = s.Code,
                Language = s.Language,
                Value = s.Value
            }).ToList() ?? new List<FormMessageTranslation>(),
            ErrorMessageTranslations = ErrorMessageTranslations?.Select(s => new FormMessageTranslation
            {
                Code = s.Code,
                Language = s.Language,
                Value = s.Value
            }).ToList() ?? new List<FormMessageTranslation>()
        };
    }

    public bool HasChild(string id)
        => GetChild(id) != null;
}