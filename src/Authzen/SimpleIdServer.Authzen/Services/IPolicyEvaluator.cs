using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Authzen.Services;

public interface IAuthzenPolicyEvaluator
{
    Task<bool> Evaluate(Dictionary<string, object> input, CancellationToken cancellationToken);
}