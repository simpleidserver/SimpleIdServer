// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Conditions;
using FormBuilder.Models;

namespace FormBuilder.Builders;

public class FormCheckboxRecordBuilder
{
    private readonly FormCheckboxRecord _record;

    internal FormCheckboxRecordBuilder(string name)
    {
        _record = new FormCheckboxRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            CorrelationId = Guid.NewGuid().ToString()
        };
    }

    public FormCheckboxRecordBuilder AddTranslation(string language, string value, IConditionParameter conditionParameter = null)
    {
        _record.Labels.Add(new LabelTranslation(language, value, conditionParameter));
        return this;
    }

    internal FormCheckboxRecord Build()
    {
        return _record;
    }
}
