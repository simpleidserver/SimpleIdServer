using SimpleIdServer.Saml.Xsd;

namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public interface IResponseBuilder
    {
        string Binding { get; }
        SingleSignOnResult Build(string location, ResponseType response, string relayState);
    }
}
