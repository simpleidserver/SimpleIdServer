// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Conditions;
using FormBuilder.Models;

namespace FormBuilder.Builders;

public class FormAnchorRecordBuilder
{
    private readonly FormAnchorRecord _record;

    internal FormAnchorRecordBuilder(string id = null, string correlationId = null)
    {
        _record = new FormAnchorRecord
        {
            Id = id ?? Guid.NewGuid().ToString(),
            CorrelationId = correlationId ?? Guid.NewGuid().ToString()
        };
    }

    public FormAnchorRecordBuilder AddTranslation(string language, string value, IConditionParameter conditionParameter = null)
    {
        _record.Labels.Add(new LabelTranslation(language, value, conditionParameter));
        return this;
    }

    public FormAnchorRecordBuilder ActAsBtn()
    {
        _record.ActAsButton = true;
        return this;
    }

    internal FormAnchorRecord Build()
    {
        return _record;
    }
}
