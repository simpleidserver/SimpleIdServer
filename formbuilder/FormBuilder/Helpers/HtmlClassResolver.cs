// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components;
using FormBuilder.Models;

namespace FormBuilder.Helpers;

public interface IHtmlClassResolver
{
    string Resolve<T>(T record, string eltName, WorkflowContext context) where T : class;
    string Resolve(string eltName, WorkflowContext context, TemplateWindowDisplayTypes type);
}

public class HtmlClassResolver : IHtmlClassResolver
{
    public string Resolve<T>(T record, string eltName, WorkflowContext context) where T : class
    {
        var elt = context.Template.Elements.SingleOrDefault(e => e.Element == typeof(T).Name && e.Name == eltName);
        return elt?.Value ?? string.Empty;
    }

    public string Resolve(string eltName, WorkflowContext context, TemplateWindowDisplayTypes type)
    {
        var elt = context.Template.Windows.SingleOrDefault(e => e.Name == eltName);
        return elt?.Value ?? string.Empty;
    }
}
