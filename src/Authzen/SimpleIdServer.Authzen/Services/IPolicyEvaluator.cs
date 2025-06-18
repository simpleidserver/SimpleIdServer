using System.Threading;
using System.Threading.Tasks;
using SimpleIdServer.Authzen.Dtos;

namespace SimpleIdServer.Authzen.Services;

public interface IAuthzenPolicyEvaluator
{
    Task<bool> Evaluate(AccessEvaluationRequest request, CancellationToken cancellationToken);
}