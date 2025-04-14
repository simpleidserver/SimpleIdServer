// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Models;

namespace FormBuilder.DefaultTemplate;

internal class RadzenTemplate
{
    public const string Name = "Radzen";

    private static string _cssContent = ":root {\r\n    --prompt-width: 600px;\r\n    --outer-padding: 80px;\r\n    --bg-color: #F7F7F6;\r\n    --from-bg-color: white;\r\n    --from-shadow: 0 12px 40px rgba(0,0,0,0.12);\r\n    --from-radius: 5px;\r\n    --divider-bg: #eee;\r\n    --divider-color: #6c757d;\r\n}\r\n\r\n.form {\r\n    padding: var(--outer-padding);\r\n    background-color: var(--bg-color);\r\n    display: flex;\r\n    flex-direction: column;\r\n    align-items: center;\r\n    height: 100%;\r\n}\r\n\r\n.form-content {\r\n    width: var(--prompt-width);\r\n    background-color: var(--from-bg-color);\r\n    box-shadow: var(--from-shadow);\r\n    border-radius: var(--from-radius);\r\n}\r\n\r\n.form-content.view {\r\n    padding: 10px;\r\n}\r\n\r\n.divider {\r\n    width: 100%;\r\n    white-space: nowrap;\r\n    align-self: stretch;\r\n    align-items: center;\r\n    display: flex;\r\n    color: #cccccc;\r\n}\r\n\r\n.divider:before {\r\n    content: \"\";\r\n    border-inline-end-width: 0;\r\n    border-style: solid;\r\n    border-top-width: 1px;\r\n    flex-grow: 1;\r\n    width: 100%;\r\n    height: 1px;\r\n    border:1px solid #cccccc;\r\n}\r\n\r\n.divider:after {\r\n    content: \"\";\r\n    border-inline-end-width: 0;\r\n    border-style: solid;\r\n    border-top-width: 1px;\r\n    flex-grow: 1;\r\n    width: 100%;\r\n    height: 1px;\r\n    border:1px solid #cccccc;\r\n}";

    public static Template DefaultTemplate
    {
        get
        {
            return TemplateBuilder.New(Name, isActive: true)
                .AddCssLibrary("/_content/Radzen.Blazor/css/default.css")
                .AddCustomCss(_cssContent)
                .SetAuthModalClasses("form", "form-content", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
                .SetAnchorClass(string.Empty, "fullWidth")
                .SetBackClass("fullWidth")
                .SetButtonClass("fullWidth")
                .SetImageClasses("picture-container", "picture")
                .SetInputTextFieldClasses("rz-form-field rz-variant-outlined rz-floating-label", string.Empty, string.Empty)
                .SetPasswordFieldClasses("rz-form-field rz-variant-outlined rz-floating-label", string.Empty, string.Empty)
                .SetDividerClasses("divider", string.Empty)
                .Build();
        }
    }
}
