namespace FormBuilder.Models;

public class BaseFormFieldRecord : IFormElementRecord
{
    public string Name { get; set; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();
}
