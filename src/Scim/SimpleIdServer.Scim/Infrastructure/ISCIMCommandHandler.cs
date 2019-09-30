using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Infrastructure
{
    public interface ISCIMCommandHandler<TCommand, TResult> where TCommand : ISCIMCommand<TResult>
    {
        Task<TResult> Handle(TCommand request);
    }
}
