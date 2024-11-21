using FormBuilder.Models.Url;
using FormBuilder.Url;

namespace FormBuilder.Factories;

public interface ITargetUrlHelperFactory
{
    string Build(ITargetUrl targetUrl);
    ITargetUrlHelper Build(string type);
    List<string> GetAllTypes();
    ITargetUrl CreateEmptyInstance(string type);
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
        return targetUrlHelper.Build(targetUrl);
    }

    public ITargetUrlHelper Build(string type)
        => _targets.Single(t => t.Type == type);

    public ITargetUrl CreateEmptyInstance(string type)
    {
        var targetUrlHelper = Build(type);
        return targetUrlHelper.CreateEmptyInstance();
    }

    public List<string> GetAllTypes()
        => _targets.Select(t => t.Type).ToList();
}
