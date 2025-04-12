﻿using FormBuilder.Models.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Models;

public abstract class BaseFormFieldRecord : IFormElementRecord
{
    public string Id { get; set; }
    public string CorrelationId { get; set; }
    public string Name { get; set; }
    public string CssStyle { get; set; }
    public abstract string Type { get; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();
    public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();
    public List<HtmlClassRecord> Classes { get; set; } = new List<HtmlClassRecord>();
    public List<ITransformationRule> Transformations { get; set; }
    public abstract void Apply(JsonNode node);
    public abstract void ExtractJson(JsonObject json);

    public IFormElementRecord GetChildByCorrelationId(string correlationId) => null;

    public IFormElementRecord GetChild(string id) => null;
}