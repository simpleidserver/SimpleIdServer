using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IGetOTPQRCodeHandler
    {
        Task<Bitmap> Handle(string id, string claimName, CancellationToken cancellationToken);
    }

    public class GetOTPQRCodeHandler: IGetOTPQRCodeHandler
    {
        private readonly OAuthHostOptions _options;
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly ILogger<GetOTPQRCodeHandler> _logger;

        public GetOTPQRCodeHandler(
            IOptions<OAuthHostOptions> options,
            IOAuthUserRepository oauthUserRepository,
            ILogger<GetOTPQRCodeHandler> logger)
        {
            _options = options.Value;
            _logger = logger;
            _oauthUserRepository = oauthUserRepository;
        }

        public async Task<Bitmap> Handle(string id, string claimName, CancellationToken cancellationToken)
        {
            OAuthUser user;
            if (!string.IsNullOrWhiteSpace(claimName))
            {
                user = await _oauthUserRepository.FindOAuthUserByClaim(claimName, id, cancellationToken);
            }
            else
            {
                user = await _oauthUserRepository.FindOAuthUserByLogin(id, cancellationToken);
            }

            if (user == null)
            {
                _logger.LogError($"the user '{id}' doesn't exist");
                throw new OAuthUserNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_USER, id));
            }

            var alg = Enum.GetName(typeof(OTPAlgs), _options.OTPAlg).ToLowerInvariant();
            var url = $"otpauth://{alg}/{_options.OTPIssuer}:{user.Id}?secret={user.OTPKey}&issuer={_options.OTPIssuer}";
            if (_options.OTPAlg == OTPAlgs.HOTP)
            {
                url = $"{url}&counter={user.OTPCounter}";
            }

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}
