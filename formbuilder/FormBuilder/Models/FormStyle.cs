namespace FormBuilder.Models;

public class FormStyle : ICloneable
{
    public string Id 
    { 
        get; set; 
    }
    
    public string FormId 
    { 
        get; set; 
    }
    
    public string Value 
    {
        get; set; 
    }
    public bool IsActive 
    { 
        get; set; 
    }

    public string TemplateName
    {
        get; set;
    }

    public FormStyleCategories Category
    {
        get; set;
    }

    public FormStyleLanguages Language
    {
        get; set;
    } = FormStyleLanguages.Css;

    public object Clone()
    {
        return new FormStyle
        {
            Id = Id,
            Value = Value,
            IsActive = IsActive,
            Category = Category,
            Language = Language
        };
    }
}