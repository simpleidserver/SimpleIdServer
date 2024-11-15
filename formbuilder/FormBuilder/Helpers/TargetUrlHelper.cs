using FormBuilder.Models;
using Microsoft.AspNetCore.Routing;

namespace FormBuilder.Helpers
{
    public interface ITargetUrlHelper
    {
        string Type { get; }
        string Build(ITargetUrl target);
    }

    public abstract class GenericTargetUrlHelper<T> : ITargetUrlHelper where T : ITargetUrl
    {
        public abstract string Type { get; }

        public string Build(ITargetUrl target) => InternalBuild((T)target);

        protected abstract string InternalBuild(T target);
    }

    public class DirectTargetUrlHelper : GenericTargetUrlHelper<DirectTargetUrl>
    {
        public override string Type => DirectTargetUrl.TYPE;

        protected override string InternalBuild(DirectTargetUrl target) => target.Url;
    }

    public class ControllerActionTargetUrlHelper : GenericTargetUrlHelper<ControllerActionTargetUrl>
    {
        private readonly LinkGenerator _linkGenerator;

        public ControllerActionTargetUrlHelper(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public override string Type => ControllerActionTargetUrl.TYPE;

        protected override string InternalBuild(ControllerActionTargetUrl target)
            => _linkGenerator.GetPathByAction(target.Action, target.Controller);
    }
}
