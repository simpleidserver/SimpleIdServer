﻿using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Input;

public partial class FormInputField : IGenericFormElement<FormInputFieldRecord>
{
    [Parameter] public FormInputFieldRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }
}
