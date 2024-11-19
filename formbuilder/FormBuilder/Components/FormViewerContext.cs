namespace FormBuilder.Components;

public class FormViewerContext
{
    public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
    public IFormElementDefinition SelectedDefinition { get; set; }
}
