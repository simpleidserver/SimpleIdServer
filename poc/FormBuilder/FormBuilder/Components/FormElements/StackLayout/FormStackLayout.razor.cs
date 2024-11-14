using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.StackLayout;

public partial class FormStackLayout : IGenericFormElement<FormStackLayoutRecord>
{
    private RenderFragment? CustomRender { get; set; }
    [Parameter] public FormStackLayoutRecord Value { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Value != null)
        {
            CustomRender = CreateComponent();
        }
    }

    void Submit()
    {
        var json = new JsonObject();
        Value.ExtractJson(json);
        string ss = "";
    }

    private RenderFragment CreateComponent() => builder =>
    {
        renderFormsElementsHelper.Render(builder, Value.Elements);
    };
}
