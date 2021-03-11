using MediatR;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenBankingApi.AccountAccessContents.Results;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Exceptions;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands.Handlers
{
    public class AddAccountAccessContentCommandHandler : IRequestHandler<AddAccountAccessContentCommand, AccountAccessContentResult>
    {
        private readonly ICommandRepository _commandRepository;
        private readonly ITokenQueryRepository _tokenQueryRepository;
        private readonly ILogger<AddAccountAccessContentCommandHandler> _logger;

        public AddAccountAccessContentCommandHandler(
            ICommandRepository commandRepository, 
            ITokenQueryRepository tokenQueryRepository,
            ILogger<AddAccountAccessContentCommandHandler> logger)
        {
            _commandRepository = commandRepository;
            _tokenQueryRepository = tokenQueryRepository;
            _logger = logger;
        }

        public async Task<AccountAccessContentResult> Handle(AddAccountAccessContentCommand request, CancellationToken cancellationToken)
        {
            var risk = request.Risk == null ? string.Empty : request.Risk.ToString();
            var token = await _tokenQueryRepository.Get(request.Token, cancellationToken);
            if (token == null)
            {
                _logger.LogError($"Access token '{request.Token}' is invalid or has been revoked");
                throw new UnauthorizedException(string.Format(Global.AccessTokenInvalid, token));
            }

            var accountAccessConsent = AccountAccessConsentAggregate.Create(token.ClientId, request.Data.Permissions, request.Data.ExpirationDateTime, request.Data.TransactionFromDateTime, request.Data.TransactionToDateTime, risk);
            await _commandRepository.Commit(accountAccessConsent, cancellationToken);
            var url = $"{request.Issuer}/{Constants.RouteNames.AccountAccessContents}/{accountAccessConsent.AggregateId}";
            return AccountAccessContentResult.ToDto(accountAccessConsent, url, 1);
        }
    }
}