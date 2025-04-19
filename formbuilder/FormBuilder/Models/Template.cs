// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace FormBuilder.Models;

public class Template : ICloneable
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

    public List<TemplateStyle> CssStyles
    {
        get
        {
            return Styles.Where(s => s.Language == TemplateStyleLanguages.Css).ToList();
        }
    }

    public List<TemplateStyle> JsStyles
    {
        get
        {
            return Styles.Where(s => s.Language == TemplateStyleLanguages.Javascript).ToList();
        }
    }

    public void SetWindowClass(string name, string className, TemplateWindowDisplayTypes displayType)
    {
        Windows.Add(new TemplateWindow
        {
            Name = name,
            Value = className,
            Element = nameof(FormRecord),
            DisplayType = displayType
        });
    }

    public object Clone()
    {
        return new Template
        {
            Id = Id,
            Name = Name,
            Realm = Realm,
            IsActive = IsActive,
            Elements = Elements.Select(e => new TemplateElement
            {
                Element = e.Element,
                Name = e.Name,
                Value = e.Value
            }).ToList(),
            Windows = Windows.Select(w => new TemplateWindow
            {
                Element = w.Element,
                Name = w.Name,
                Value = w.Value,
                DisplayType = w.DisplayType
            }).ToList(),
            Styles = Styles.Select(s => new TemplateStyle
            {
                Id = s.Id,
                Value = s.Value,
                Category = s.Category,
                Language = s.Language
            }).ToList()
        };
    }
}