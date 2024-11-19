using FormBuilder.Helpers;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.ListData
{
    public partial class ListDataElt : IGenericFormElement<ListDataRecord>
    {
        private RenderFragment? CustomRender { get; set; }
        [Inject] private IRenderFormElementsHelper renderFormsElementsHelper { get; set; }
        [Parameter] public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
        [Parameter] public ListDataRecord Value { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Value != null)
            {
                CustomRender = CreateComponent();
            }
        }

        private RenderFragment CreateComponent() => builder =>
        {
            renderFormsElementsHelper.Render(builder, Value.Elements, AntiforgeryToken);
        };
    }
}
