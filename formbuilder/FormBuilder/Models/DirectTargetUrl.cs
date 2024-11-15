namespace FormBuilder.Models;

public class DirectTargetUrl : ITargetUrl
{
    public const string TYPE = "DIRECT";
    public string Type => Type;
    public string Url { get; set; }
}
