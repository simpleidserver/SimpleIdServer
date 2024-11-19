using FormBuilder.Models.Url;
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

    protected override string InternalBuild(ControllerActionTargetUrl target)
        => target == null ? null : _linkGenerator.GetPathByAction(target.Action, target.Controller);
}
