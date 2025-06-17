namespace FormBuilder.Components.FormElements.ReCaptcha
{
    public class ReCaptchaDefinition : GenericFormElementDefinition<ReCaptchaRecord>
    {
        public override Type UiElt => typeof(ReCaptchaComponent);


        public static string TYPE = "ReCaptcha";

        public override string Type => TYPE;

        public override string Icon => "frame_person";

        public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;

        protected override void ProtectedInit(ReCaptchaRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
        {
        }
    }
}
