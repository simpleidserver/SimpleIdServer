namespace FormBuilder.Conditions;

public class NotPresentParameter : IConditionParameter
{
    public static string TYPE = "NotPresent";
    public string Source { get; set; }
    public string Type => TYPE;
}