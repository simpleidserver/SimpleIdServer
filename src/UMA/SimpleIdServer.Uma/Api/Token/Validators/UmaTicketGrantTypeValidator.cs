using SimpleIdServer.OAuth.Api;

namespace SimpleIdServer.Uma.Api.Token.Validators
{
    public interface IUmaTicketGrantTypeValidator
    {
        void Validate(HandlerContext handlerContext);
    }

    public class UmaTicketGrantTypeValidator : IUmaTicketGrantTypeValidator
    {
        public void Validate(HandlerContext handlerContext)
        {
            // Vérifier le paramètre "ticket" existe.
            throw new System.NotImplementedException();
        }
    }
}
