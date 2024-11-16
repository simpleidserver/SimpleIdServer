using FormBuilder.Models.Url;

namespace FormBuilder.Url
{
    public class DirectTargetUrlHelper : GenericTargetUrlHelper<DirectTargetUrl>
    {
        public override string Type => DirectTargetUrl.TYPE;

        protected override string InternalBuild(DirectTargetUrl target) => target.Url;
    }
}
