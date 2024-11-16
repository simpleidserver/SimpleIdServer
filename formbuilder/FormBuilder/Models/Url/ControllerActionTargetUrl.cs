namespace FormBuilder.Models.Url;

public class ControllerActionTargetUrl : ITargetUrl
{
    public static string TYPE = "CONTROLLERACTION";
    public string Type => TYPE;
    public string Controller { get; set; }
    public string Action { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}
