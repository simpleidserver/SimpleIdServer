// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace FormBuilder.Models;

public class TemplateWindow : TemplateElement
{
    public bool IsDefault
    {
        get; set;
    }

    public TemplateWindowDisplayTypes DisplayType
    {
        get; set;
    }
}
