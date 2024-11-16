using FormBuilder.Models.Url;

namespace FormBuilder.Url
{
    public interface ITargetUrlHelper
    {
        string Type { get; }
        string Build(ITargetUrl target);
    }
}
