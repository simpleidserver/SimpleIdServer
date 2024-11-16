using FormBuilder.Helpers;
using FormBuilder.Models.Url;

namespace FormBuilder.Factories;

public interface ITargetUrlHelperFactory
{
    string Build(ITargetUrl targetUrl);
    ITargetUrlHelper Build(string type);
}

public class TargetUrlHelperFactory : ITargetUrlHelperFactory
{
    private readonly IEnumerable<ITargetUrlHelper> _targets;

    public TargetUrlHelperFactory(IEnumerable<ITargetUrlHelper> targets)
    {
        _targets = targets;
    }

    public string Build(ITargetUrl targetUrl)
    {
        var targetUrlHelper = Build(targetUrl.Type);
        if (targetUrlHelper == null) return null;
        return targetUrlHelper.Build(targetUrl);
    }

    public ITargetUrlHelper Build(string type)
    {
        return _targets.SingleOrDefault(t => t.Type == type);
    }
}
