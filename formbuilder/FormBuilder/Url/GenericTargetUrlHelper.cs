using FormBuilder.Models.Url;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Url;


public abstract class GenericTargetUrlHelper<T> : ITargetUrlHelper where T : ITargetUrl
{
    public abstract string Type { get; }

    public string Build(ITargetUrl target) => InternalBuild((T)target);

    public void BuildComponent(ITargetUrl target, RenderTreeBuilder builder) => InternalBuildComponent((T)target, builder);

    public abstract ITargetUrl CreateEmptyInstance();

    protected abstract string InternalBuild(T target);

    protected abstract void InternalBuildComponent(T target, RenderTreeBuilder builder);
}
