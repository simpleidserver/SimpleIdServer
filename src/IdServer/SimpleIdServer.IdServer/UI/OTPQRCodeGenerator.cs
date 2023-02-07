// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SimpleIdServer.IdServer.UI
{
    public interface IOTPQRCodeGenerator
    {
        byte[] GenerateQRCode(User user);
    }

    public class OTPQRCodeGenerator : IOTPQRCodeGenerator
    {
        private readonly IdServerHostOptions _options;

        public OTPQRCodeGenerator(IOptions<IdServerHostOptions> options)
        {
            _options = options.Value;
        }

        public byte[] GenerateQRCode(User user)
        {
            var alg = Enum.GetName(typeof(OTPAlgs), user.ActiveOTP.OTPAlg).ToLowerInvariant();
            var url = $"otpauth://{alg}/{_options.OTPIssuer}:{user.Name}?secret={user.ActiveOTP.Value}&issuer={_options.OTPIssuer}&algorithm=SHA1";
            if (_options.OTPAlg == OTPAlgs.HOTP)
                url = $"{url}&counter={user.ActiveOTP.OTPCounter}";
            if (_options.OTPAlg == OTPAlgs.TOTP)
                url = $"{url}&period={_options.TOTPStep}";
            byte[] payload = null;
            var result = GetQRCode();
            using (var stream = new MemoryStream())
            {
                result.Save(stream, ImageFormat.Png);
                payload = stream.ToArray();
            }

            return payload;

            Bitmap GetQRCode()
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                return qrCode.GetGraphic(20);
            }
        }
    }
}
