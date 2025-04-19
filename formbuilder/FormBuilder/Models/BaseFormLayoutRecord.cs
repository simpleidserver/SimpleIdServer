// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models.Rules;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace FormBuilder.Models;

public abstract class BaseFormLayoutRecord : IFormElementRecord, IFormRecordCollection
{
    public string Id { get; set; }
    public string CorrelationId { get; set; }
    public string CssStyle { get; set; } = "";
    public abstract string Type { get; }
    public ObservableCollection<IFormElementRecord> Elements { get; set; } = new ObservableCollection<IFormElementRecord>();
    public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();
    public List<ITransformationRule> Transformations { get; set; }

    public void Apply(JsonNode node)
    {

    }

    public void ExtractJson(JsonObject json)
    {
        foreach(var elt in Elements)
            elt.ExtractJson(json);
    }

    public IFormElementRecord GetChildByCorrelationId(string correlationId)
    {
        var result = Elements.SingleOrDefault(e => e.CorrelationId == correlationId);
        if (result != null) return result;
        foreach (var elt in Elements)
        {
            var child = elt.GetChildByCorrelationId(correlationId);
            if (child != null) return child;
        }

        return null;
    }

    public IFormElementRecord GetChild(string id) => Elements.SingleOrDefault(e => e.Id == id);
}
