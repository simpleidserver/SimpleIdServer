﻿using FormBuilder.Models;
namespace FormBuilder.Components.FormElements;

public interface IGenericFormElement<T> where T : IFormElementRecord
{
    T Value { get; set; }
    FormViewerContext Context { get; set; }
    bool IsEditModeEnabled { get; set; }
}
