// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using System;

namespace SimpleIdServer.IdServer.UI
{
    public interface IOTPQRCodeGenerator
    {
        byte[] GenerateQRCode(User user);
        byte[] GenerateQRCode(User user, UserCredential credential);
    }

    public class OTPQRCodeGenerator : IOTPQRCodeGenerator
    {
        private readonly IdServerHostOptions _options;

        public OTPQRCodeGenerator(IOptions<IdServerHostOptions> options)
        {
            _options = options.Value;
        }

        public byte[] GenerateQRCode(User user) => GenerateQRCode(user, user.ActiveOTP);

        public byte[] GenerateQRCode(User user, UserCredential credential)
        {
            var alg = Enum.GetName(typeof(OTPAlgs), credential.OTPAlg).ToLowerInvariant();
            var url = $"otpauth://{alg}/{_options.OTPIssuer}:{user.Name}?secret={credential.Value}&issuer={_options.OTPIssuer}&algorithm=SHA1";
            if (_options.OTPAlg == OTPAlgs.HOTP)
                url = $"{url}&counter={credential.OTPCounter}";
            if (_options.OTPAlg == OTPAlgs.TOTP)
                url = $"{url}&period={_options.TOTPStep}";
            var result = GetQRCode();
            return result;

            byte[] GetQRCode()
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                return qrCode.GetGraphic(20);
            }
        }
    }
}
