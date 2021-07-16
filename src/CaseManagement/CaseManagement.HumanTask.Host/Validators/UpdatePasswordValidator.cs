using CaseManagement.HumanTask.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CaseManagement.HumanTask.Host.Validators
{
    public class UpdatePasswordValidator : IHumanTaskInstanceValidator
    {
        public Task Validate(HumanTaskInstanceAggregate humanTaskInstance, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
