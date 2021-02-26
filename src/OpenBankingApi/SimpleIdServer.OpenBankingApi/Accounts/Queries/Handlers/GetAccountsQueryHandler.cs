using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenBankingApi.Accounts.Results;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Accounts.Queries.Handlers
{
    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, GetAccountsResult>
    {
        private readonly ITokenQueryRepository _tokenQueryRepository;
        private readonly IJwtParser _jwtParser;
        private readonly IAccountAccessConsentRepository _accountAccessConsentRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly OpenBankingApiOptions _options;
        private readonly ILogger<GetAccountsQueryHandler> _logger;

        public GetAccountsQueryHandler(
            ITokenQueryRepository tokenQueryRepository,
            IJwtParser jwtParser,
            IAccountAccessConsentRepository accountAccessConsentRepository,
            IAccountRepository accountRepository,
            IOptions<OpenBankingApiOptions> options,
            ILogger<GetAccountsQueryHandler> logger)
        {
            _tokenQueryRepository = tokenQueryRepository;
            _jwtParser = jwtParser;
            _accountAccessConsentRepository = accountAccessConsentRepository;
            _accountRepository = accountRepository;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<GetAccountsResult> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            var jwsPayload = await Extract(request.AccessToken);
            if (jwsPayload == null)
            {
                _logger.LogError("access token is invalid");
                throw new OAuthException(ErrorCodes.INVALID_TOKEN, OAuth.ErrorMessages.BAD_TOKEN);
            }

            var token = await _tokenQueryRepository.Get(request.AccessToken, cancellationToken);
            if (token == null)
            {
                _logger.LogError("cannot get accounts because access token has been rejected");
                throw new OAuthException(ErrorCodes.INVALID_TOKEN, ErrorMessages.ACCESS_TOKEN_REJECTED);
            }

            var consentId = jwsPayload[_options.OpenBankingApiConsentClaimName].ToString();
            var consent = await _accountAccessConsentRepository.Get(consentId, cancellationToken);
            if (!consent.Permissions.Contains(AccountAccessConsentPermission.ReadAccountsBasic) &&
                !consent.Permissions.Contains(AccountAccessConsentPermission.ReadAccountsDetail))
            {
                _logger.LogError("no permissions to read accounts");
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.NoPermissionToReadAccounts);
            }

            var result = await _accountRepository.Get(consent.AccountIds, cancellationToken);
            return GetAccountsResult.ToResult(result, consent.Permissions, request.Issuer, 1);
        }

        private async Task<JwsPayload> Extract(string accessToken)
        {
            var isJwe = _jwtParser.IsJweToken(accessToken);
            var isJws = _jwtParser.IsJwsToken(accessToken);
            if (!isJwe && !isJws)
            {
                return null;
            }

            var jws = accessToken;
            if (isJwe)
            {
                jws = await _jwtParser.Decrypt(accessToken);
            }

            return await _jwtParser.Unsign(jws);
        }
    }
}
