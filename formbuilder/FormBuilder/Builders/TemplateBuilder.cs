// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.Form;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Back;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Image;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Models;

namespace FormBuilder.Builders;

public class TemplateBuilder
{
    private readonly Template _template;

    public TemplateBuilder(string name, string realm, bool isActive = false)
    {
        _template = new Template
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Realm = realm,
            IsActive = isActive
        };
    }

    public static TemplateBuilder New(string name, string realm = "master")
    {
        return new TemplateBuilder(name, realm);
    }

    internal static TemplateBuilder New(string name, string realm = "master", bool isActive = false)
    {
        return new TemplateBuilder(name, realm, isActive);
    }

    public TemplateBuilder SetAuthModalClasses(
        string containerClass, 
        string contentClass, 
        string formContainerClass, 
        string formContentClass, 
        string stepperListClass, 
        string stepperListItemClass, 
        string stepperListItemActiveClass,
        string stepperListItemContainerClass, 
        string stepperListItemNumberClass,
        string stepperListItemNumberActiveClass,
        string stepperListItemTextClass,
        string stepperListItemTextActiveClass)
    {
        _template.SetWindowClass(FormElementNames.Container, containerClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.Content, contentClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.FormContainer, formContainerClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.FormContent, formContentClass, TemplateWindowDisplayTypes.MODAL);
        // Stepper.
        _template.SetWindowClass(FormElementNames.StepperList, stepperListClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.StepperListItem, stepperListItemClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.StepperListItemActive, stepperListItemActiveClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.StepperListItemContainer, stepperListItemContainerClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.StepperListItemNumber, stepperListItemNumberClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.StepperListItemNumberActive, stepperListItemNumberActiveClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.StepperListItemText, stepperListItemTextClass, TemplateWindowDisplayTypes.MODAL);
        _template.SetWindowClass(FormElementNames.StepperListItemTextActive, stepperListItemTextActiveClass, TemplateWindowDisplayTypes.MODAL);
        return this;
    }

    public TemplateBuilder AddCssLibrary(string url)
    {
        _template.Styles.Add(new TemplateStyle
        {
            Id = Guid.NewGuid().ToString(),
            Category = TemplateStyleCategories.Lib,
            Language = TemplateStyleLanguages.Css,
            Value = url
        });
        return this;
    }

    public TemplateBuilder AddCustomCss(string content)
    {
        _template.Styles.Add(new TemplateStyle
        {
            Id = Guid.NewGuid().ToString(),
            Category = TemplateStyleCategories.Custom,
            Language = TemplateStyleLanguages.Css,
            Value = content
        });
        return this;
    }

    public TemplateBuilder AddJsLibrary(string url)
    {
        _template.Styles.Add(new TemplateStyle
        {
            Id = Guid.NewGuid().ToString(),
            Category = TemplateStyleCategories.Lib,
            Language = TemplateStyleLanguages.Javascript,
            Value = url
        });
        return this;
    }

    public TemplateBuilder AddCustomJs(string content)
    {
        _template.Styles.Add(new TemplateStyle
        {
            Id = Guid.NewGuid().ToString(),
            Category = TemplateStyleCategories.Custom,
            Language = TemplateStyleLanguages.Javascript,
            Value = content
        });
        return this;
    }

    public TemplateBuilder SetInputTextFieldClasses(string containerClass, string labelClass, string textBoxClass)
    {
        SetClass(nameof(FormInputFieldRecord), FormInputElementNames.Container, containerClass);
        SetClass(nameof(FormInputFieldRecord), FormInputElementNames.Label, labelClass);
        SetClass(nameof(FormInputFieldRecord), FormInputElementNames.TextBox, textBoxClass);
        return this;
    }

    public TemplateBuilder SetPasswordFieldClasses(string containerClass, string labelClass, string textBoxClass)
    {
        SetClass(nameof(FormPasswordFieldRecord), FormPasswordElementNames.Container, containerClass);
        SetClass(nameof(FormPasswordFieldRecord), FormPasswordElementNames.Label, labelClass);
        SetClass(nameof(FormPasswordFieldRecord), FormPasswordElementNames.Password, textBoxClass);
        return this;
    }

    public TemplateBuilder SetDividerClasses(string containerClass, string textClass)
    {
        SetClass(nameof(DividerLayoutRecord), DividerElementNames.Container, containerClass);
        SetClass(nameof(DividerLayoutRecord), DividerElementNames.Text, textClass);
        return this;
    }

    public TemplateBuilder SetAnchorClass(string anchorClass, string btnClass)
    {
        SetClass(nameof(FormAnchorRecord), AnchorElementNames.Anchor, anchorClass);
        SetClass(nameof(FormAnchorRecord), AnchorElementNames.Btn, btnClass);
        return this;
    }

    public TemplateBuilder SetBackClass(string btnClass)
    {
        SetClass(nameof(BackButtonRecord), BackButtonElementNames.Btn, btnClass);
        return this;
    }

    public TemplateBuilder SetButtonClass(string btnClass)
    {
        SetClass(nameof(FormButtonRecord), FormButtonElementNames.Btn, btnClass);
        return this;
    }

    public TemplateBuilder SetCheckboxClass(string containerClass, string checkboxClass, string labelClass)
    {
        SetClass(nameof(FormCheckboxRecord), CheckboxElementNames.Container, containerClass);
        SetClass(nameof(FormCheckboxRecord), CheckboxElementNames.Checkbox, checkboxClass);
        SetClass(nameof(FormCheckboxRecord), CheckboxElementNames.Label, labelClass);
        return this;
    }

    public TemplateBuilder SetImageClasses(string containerClass, string imgClass)
    {
        SetClass(nameof(ImageRecord), ImageElementNames.Container, containerClass);
        SetClass(nameof(ImageRecord), ImageElementNames.Img, imgClass);
        return this;
    }


    public Template Build()
    {
        return _template;
    }

    private void SetClass(string elt, string name, string className)
    {
        _template.Elements.Add(new TemplateElement
        {
            Element = elt,
            Name = name,
            Value = className
        });
    }
}
