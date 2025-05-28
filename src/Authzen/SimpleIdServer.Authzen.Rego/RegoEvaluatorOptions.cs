namespace SimpleIdServer.Authzen.Rego;

public class RegoEvaluatorOptions
{
    public required string PoliciesFolderBasePath
    {
        get; set;
    } = "policies";

    public required string PolicyPath
    {
        get; set;
    } = "authzen.evaluate.allow";
}