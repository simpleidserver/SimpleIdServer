// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.UI;
using System;
using Xunit;

namespace SimpleIdServer.OpenID.Tests.UI
{
    public class TOTPAuthenticatorFixture
    {
        private static readonly byte[] secret = new byte[] {
            0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38,
            0x39, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36,
            0x37, 0x38, 0x39, 0x30
            };

        [Fact]
        public void When_Generate_TOTP()
        {
            // ACT
            var user = new OAuthUser();
            user.ResetOtp(secret);
            var options = new OAuth.Options.OAuthHostOptions();
            var opts = Microsoft.Extensions.Options.Options.Create(options);
            var authenticator = new TOTPAuthenticator(opts);
            var date = DateTime.Parse("12-07-21 20:07:56");

            // ARRANGE
            var otp = authenticator.GenerateOtp(user, date);

            // ASSERT
            Assert.Equal(102892, otp);
        }
    }
}
