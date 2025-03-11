// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Layout;

public class FormCssStyle
{
    public static string Get()
    {
        return ":root {\r\n    --prompt-width: 600px;\r\n    --outer-padding: 80px;\r\n    --bg-color: #F7F7F6;\r\n    --from-bg-color: white;\r\n    --from-shadow: 0 12px 40px rgba(0,0,0,0.12);\r\n    --from-radius: 5px;\r\n    --divider-bg: gray;\r\n}\r\n\r\n.form {\r\n    padding: var(--outer-padding);\r\n    background-color: var(--bg-color);\r\n    display: flex;\r\n    flex-direction: column;\r\n    align-items: center;\r\n    height: 100%;\r\n}\r\n\r\n.form-content {\r\n    width: var(--prompt-width);\r\n    background-color: var(--from-bg-color);\r\n    box-shadow: var(--from-shadow);\r\n    border-radius: var(--from-radius);\r\n}\r\n\r\n.form-content.view {\r\n    padding: 10px;\r\n}\r\n\r\n/* Divider */\r\n.divider {\r\n    display: flex;\r\n    align-items: center;\r\n}\r\n\r\n.divider::before, .divider::after {\r\n    content: \"\";\r\n    height: 1px;\r\n    background: var(--divider-bg);\r\n    flex: 1;\r\n}\r\n\r\n.divider .text {\r\n    margin-right: 1rem;\r\n    margin-left: 1rem;\r\n    margin-top: 0px;\r\n    margin-bottom: 0px;\r\n    color: var(--divider-color);\r\n}\r\n\r\n.picture-container {\r\n    text-align: center;\r\n}\r\n\r\n.picture-container .picture {\r\n    width: 150px;\r\n}";
    }
}
