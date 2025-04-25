// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Models;

namespace FormBuilder.DefaultTemplate;

internal class RadzenTemplate
{
    public const string Name = "Radzen";

    private static string _cssContent = ":root {\r\n    --prompt-width: 600px;\r\n    --outer-padding: 80px;\r\n    --bg-color: #F7F7F6;\r\n    --from-bg-color: white;\r\n    --from-shadow: 0 12px 40px rgba(0,0,0,0.12);\r\n    --from-radius: 5px;\r\n    --divider-bg: gray;\r\n}\r\n\r\n@media (min-width:375px) and (max-width:768px) {\r\n   :root {\r\n    --prompt-width: 400px;\r\n  }\r\n}\r\n\r\n@media (max-width: 375px) {\r\n   :root {\r\n    --prompt-width: 150px;\r\n  }\r\n}\r\n\r\n.form {\r\n    padding: var(--outer-padding);\r\n    background-color: var(--bg-color);\r\n    display: flex;\r\n    flex-direction: column;\r\n    align-items: center;\r\n    height: 100%;\r\n}\r\n\r\n.form-content {\r\n    width: var(--prompt-width);\r\n    background-color: var(--from-bg-color);\r\n    box-shadow: var(--from-shadow);\r\n    border-radius: var(--from-radius);\r\n}\r\n\r\n.form-content.view {\r\n    padding: 10px;\r\n}\r\n\r\n/* Divider */\r\n.divider {\r\n    display: flex;\r\n    align-items: center;\r\n}\r\n\r\n.divider::before, .divider::after {\r\n    content: \"\";\r\n    height: 1px;\r\n    background: var(--divider-bg);\r\n    flex: 1;\r\n}\r\n\r\n.divider .text {\r\n    margin-right: 1rem;\r\n    margin-left: 1rem;\r\n    margin-top: 0px;\r\n    margin-bottom: 0px;\r\n    color: var(--divider-color);\r\n}\r\n\r\n.picture-container {\r\n    text-align: center;\r\n}\r\n\r\n.picture-container .picture {\r\n    width: 150px;\r\n}\r\n\r\n/*Stepper*/\r\n.steps {\r\n    flex-direction: row !important;\r\n    padding: 0px;\r\n}\r\n\r\n.steps .steps-item:not(:last-child) {\r\n    flex: auto;\r\n    display: flex;\r\n    align-items: center;\r\n}\r\n\r\n.steps .steps-item:not(:last-child):after {\r\n    display: \"block\";\r\n    content: \"\";\r\n    flex: auto;\r\n    height: 1px;\r\n    margin-inline-end: 16px;\r\n    background-color: var(--rz-base-300);\r\n}";

    public static Template DefaultTemplate
    {
        get
        {
            return TemplateBuilder.New(Name, isActive: true)
                .AddCssLibrary("/_content/Radzen.Blazor/css/default.css")
                .AddCustomCss(_cssContent)
                .SetAuthModalClasses("form", "form-content", string.Empty, string.Empty)
                .SetStepperClasses(
                    "rz-steps steps",
                    "rz-steps-item steps-item", 
                    "rz-state-highlight rz-steps-current", 
                    "rz-menuitem-link", 
                    "rz-steps-number", 
                    string.Empty, 
                    "rz-steps-title", 
                    string.Empty
                )
                .SetAnchorClass(string.Empty, "rz-button rz-button-md rz-variant-filled rz-primary rz-shade-default fullWidth")
                .SetBackClass("rz-button rz-button-md rz-variant-filled rz-primary rz-shade-default fullWidth")
                .SetButtonClass("rz-button rz-button-md rz-variant-filled rz-primary rz-shade-default fullWidth")
                .SetImageClasses("picture-container", "picture")
                .SetInputTextFieldClasses("rz-form-field rz-variant-outlined rz-floating-label fullWidth", string.Empty, string.Empty)
                .SetPasswordFieldClasses("rz-form-field rz-variant-outlined rz-floating-label fullWidth", string.Empty, string.Empty)
                .SetDividerClasses("divider", string.Empty)
                .Build();
        }
    }
}