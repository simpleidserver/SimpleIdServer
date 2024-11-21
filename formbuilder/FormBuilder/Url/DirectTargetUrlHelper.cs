using FormBuilder.Models.Url;
using FormBuilder.Url.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Url
{
    public class DirectTargetUrlHelper : GenericTargetUrlHelper<DirectTargetUrl>
    {
        public override string Type => DirectTargetUrl.TYPE;

        public override ITargetUrl CreateEmptyInstance() => new DirectTargetUrl();

        protected override string InternalBuild(DirectTargetUrl target) => target.Url;

        protected override void InternalBuildComponent(DirectTargetUrl target, RenderTreeBuilder builder)
        {
            builder.OpenComponent<DirectTargetComponent>(0);
            builder.AddAttribute(1, nameof(DirectTargetComponent.Record), target);
            builder.CloseComponent();
        }
    }
}
