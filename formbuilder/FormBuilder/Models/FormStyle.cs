namespace FormBuilder.Models;

public class FormStyle : ICloneable
{
    public string Id { get; set; }
    public string FormId { get; set; }
    public string Content { get; set; }
    public bool IsActive { get; set; }

    public object Clone()
    {
        return new FormStyle
        {
            Id = Id,
            Content = Content,
            IsActive = IsActive
        };
    }
}