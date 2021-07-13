// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.UI;
using Xunit;

namespace SimpleIdServer.OpenID.Tests.UI
{
    public class HOTPAuthenticatorFixture
    {
        private static readonly byte[] secret = new byte[] {
            0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38,
            0x39, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36,
            0x37, 0x38, 0x39, 0x30
            };

        [Theory]
        [InlineData(0, 755224)]
        [InlineData(1, 287082)]
        [InlineData(2, 359152)]
        [InlineData(3, 969429)]
        [InlineData(4, 338314)]
        public void When_Generate_HOTP(int counter, long expectedOtp)
        {
            // ACT
            var user = new OAuthUser();
            user.ResetOtp(secret, counter);
            var options = new OAuth.Options.OAuthHostOptions();
            var opts = Microsoft.Extensions.Options.Options.Create(options);
            var authenticator = new HOTPAuthenticator(opts);

            // ARRANGE
            var otp = authenticator.GenerateOtp(user);
            
            // ASSERT
            Assert.Equal(expectedOtp, otp);
        }
    }
}
