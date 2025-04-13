// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace FormBuilder.Models;

public class Template
{
    public string Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = string.Empty;

    public string Realm
    {
        get; set;
    }

    public bool IsActive
    {
        get; set;
    }

    public List<TemplateElement> Elements
    {
        get; set;
    } = new List<TemplateElement>();

    public List<TemplateWindow> Windows
    {
        get; set;
    } = new List<TemplateWindow>();

    public List<TemplateStyle> Styles
    {
        get; set;
    } = new List<TemplateStyle>();

    public void SetWindowClass(string name, string className, TemplateWindowDisplayTypes displayType)
    {
        Windows.Add(new TemplateWindow
        {
            Name = name,
            Value = className,
            Element = nameof(TemplateWindow),
            DisplayType = displayType
        });
    }
}