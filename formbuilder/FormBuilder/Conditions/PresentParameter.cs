namespace FormBuilder.Conditions;

public class PresentParameter : IConditionParameter
{
    public static string TYPE = "Present";
    public string Source { get; set; }
    public string Type => TYPE;
}
