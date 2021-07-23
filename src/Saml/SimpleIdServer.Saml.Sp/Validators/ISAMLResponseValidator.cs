using SimpleIdServer.Saml.DTOs;
using SimpleIdServer.Saml.Xsd;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Sp.Validators
{
    public interface ISAMLResponseValidator
    {
        Task<AssertionType> Validate(SAMLResponseDto samlResponse, CancellationToken cancellationToken);
    }
}
