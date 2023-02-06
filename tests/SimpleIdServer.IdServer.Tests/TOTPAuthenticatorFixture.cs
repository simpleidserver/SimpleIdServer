// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.UI;

namespace SimpleIdServer.IdServer.Tests
{
    public class TOTPAuthenticatorFixture
    {
        private static readonly byte[] secret = new byte[] {
            0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38,
            0x39, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36,
            0x37, 0x38, 0x39, 0x30
            };

        [Test]
        public void CheckTOTP()
        {
            // ACT
            var user = UserBuilder.Create("login", "pwd").Build();
            user.Credentials.Add(new Domains.UserCredential
            {
                Value = secret.ConvertFromBase32()
            });
            var options = new IdServerHostOptions();
            var opts = Microsoft.Extensions.Options.Options.Create(options);
            var authenticator = new TOTPAuthenticator(opts);
            var date = DateTime.Parse("12-07-21 20:07:56");

            // ARRANGE
            var otp = authenticator.GenerateOtp(user.Credentials.Last(), date);

            // ASSERT
            Assert.That(102892 == otp);
        }
    }
}
