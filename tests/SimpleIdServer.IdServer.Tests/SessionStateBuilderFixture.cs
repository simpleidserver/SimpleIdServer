// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.IdServer.Api.Authorization;

namespace SimpleIdServer.IdServer.Tests
{
    public class SessionStateBuilderFixture
    {
        [Test]
        public void CheckSessionIsCorrect()
        {
            var sessionState = AuthorizationRequestHandler.BuildSessionState("http://localhost:4200", "spa", "00b367a0-350b-4e40-852c-cdeaa3f38303", "f863daeb-61db-4afa-8955-52aaf95df38a");
            Assert.That("8c_w5lKq7W_6KrbYd7ZcBxQTzIxGKAJxWKSVF7e2-Zo" == sessionState.Split('.').First());
        }
    }
}
