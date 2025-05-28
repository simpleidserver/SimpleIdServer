using System.Collections.Generic;

namespace SimpleIdServer.Authzen.Services;

public interface IPolicyEvaluator
{
    bool Evaluate(Dictionary<string, object> input);
}