namespace SimpleIdServer.Authzen.Dtos;

public class AccessEvaluationRequest
{
    public Subject Subject { get; set; }
    public Action Action { get; set; }
    public Resource Resource { get; set; }
    public Context Context { get; set; }
}
