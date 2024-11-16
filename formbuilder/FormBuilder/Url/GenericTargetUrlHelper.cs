using FormBuilder.Models.Url;

namespace FormBuilder.Url
{

    public abstract class GenericTargetUrlHelper<T> : ITargetUrlHelper where T : ITargetUrl
    {
        public abstract string Type { get; }

        public string Build(ITargetUrl target) => InternalBuild((T)target);

        protected abstract string InternalBuild(T target);
    }
}
