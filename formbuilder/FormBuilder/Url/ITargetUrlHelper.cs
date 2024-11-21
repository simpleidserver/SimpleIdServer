using FormBuilder.Models.Url;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Url
{
    public interface ITargetUrlHelper
    {
        string Type { get; }
        string Build(ITargetUrl target);
        void BuildComponent(ITargetUrl target, RenderTreeBuilder builder);
        ITargetUrl CreateEmptyInstance();
    }
}
