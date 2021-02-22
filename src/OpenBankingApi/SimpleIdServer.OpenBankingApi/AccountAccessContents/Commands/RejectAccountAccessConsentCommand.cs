using MediatR;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands
{
    public class RejectAccountAccessConsentCommand : IRequest<bool>
    {
        public string ConsentId { get; set; }
    }
}
