// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Conditions;

namespace FormBuilder.Models;

public class LabelTranslation
{
    public LabelTranslation()
    {
        
    }

    public LabelTranslation(string language, string translation, IConditionParameter conditionParameter)
    {
        Language = language;
        Translation = translation;
        ConditionParameter = conditionParameter;
    }

    public string Translation { get; set; }
    public string Language { get; set; }
    public IConditionParameter ConditionParameter { get; set; }
}
