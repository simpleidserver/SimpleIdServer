// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.UI;

namespace SimpleIdServer.IdServer.Tests
{
    public class HOTPAuthenticatorFixture
    {
        private static readonly byte[] secret = new byte[] {
            0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38,
            0x39, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36,
            0x37, 0x38, 0x39, 0x30
            };

        [Test]
        [TestCase(0, 755224)]
        [TestCase(1, 287082)]
        [TestCase(2, 359152)]
        [TestCase(3, 969429)]
        [TestCase(4, 338314)]
        public void CheckHOTP(int counter, long expectedOtp)
        {
            // ACT
            var user = UserBuilder.Create("login", "pwd").Build();
            user.Credentials.Add(new Domains.UserCredential
            {
                Value = secret.ConvertFromBase32(),
                OTPCounter= counter
            });
            var options = new IdServerHostOptions();
            var opts = Microsoft.Extensions.Options.Options.Create(options);
            var authenticator = new HOTPAuthenticator(opts);

            // ARRANGE
            var otp = authenticator.GenerateOtp(user.Credentials.Last());

            // ASSERT
            Assert.That(expectedOtp == otp);
        }
    }
}
