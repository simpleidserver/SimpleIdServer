using FormBuilder.Models.Url;
using FormBuilder.Url.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Routing;

namespace FormBuilder.Url;

public class ControllerActionTargetUrlHelper : GenericTargetUrlHelper<ControllerActionTargetUrl>
{
    private readonly LinkGenerator _linkGenerator;

    public ControllerActionTargetUrlHelper(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public override string Type => ControllerActionTargetUrl.TYPE;

    public override ITargetUrl CreateEmptyInstance() => new ControllerActionTargetUrl();

    protected override string InternalBuild(ControllerActionTargetUrl target)
        => target == null ? null : _linkGenerator.GetPathByAction(target.Action, target.Controller);

    protected override void InternalBuildComponent(ControllerActionTargetUrl target, RenderTreeBuilder builder)
    {
        builder.OpenComponent<ControllerActionTargetComponent>(0);
        builder.AddAttribute(1, nameof(ControllerActionTargetComponent.Record), target);
        builder.CloseComponent();
    }
}
