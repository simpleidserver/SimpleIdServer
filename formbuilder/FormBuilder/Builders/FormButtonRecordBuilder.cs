// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Conditions;
using FormBuilder.Models;

namespace FormBuilder.Builders;

public class FormButtonRecordBuilder
{
    private readonly FormButtonRecord _record;

    internal FormButtonRecordBuilder(string name)
    {
        _record = new FormButtonRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            CorrelationId = Guid.NewGuid().ToString()
        };
    }

    public FormButtonRecordBuilder AddTranslation(string language, string value, IConditionParameter conditionParameter = null)
    {
        _record.Labels.Add(new LabelTranslation(language, value, conditionParameter));
        return this;
    }

    internal FormButtonRecord Build()
    {
        return _record;
    }
}
