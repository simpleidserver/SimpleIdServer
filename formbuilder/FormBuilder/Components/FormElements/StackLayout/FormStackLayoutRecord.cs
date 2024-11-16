using FormBuilder.Models;
using FormBuilder.Models.Url;

namespace FormBuilder.Components.FormElements.StackLayout;

public class FormStackLayoutRecord : BaseFormLayoutRecord
{
    public bool IsFormEnabled { get; set; } = false;
    public bool IsAntiforgeryEnabled { get; set; } = false;
    public ITargetUrl Url { get; set; }
}