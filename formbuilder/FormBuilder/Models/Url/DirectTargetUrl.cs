namespace FormBuilder.Models.Url;

public class DirectTargetUrl : ITargetUrl
{
    public const string TYPE = "DIRECT";
    public string Type => TYPE;
    public string Url { get; set; }
}
