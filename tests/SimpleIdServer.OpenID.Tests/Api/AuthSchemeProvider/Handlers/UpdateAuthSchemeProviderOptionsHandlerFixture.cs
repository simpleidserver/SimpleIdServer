// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OpenID.Api.AuthSchemeProvider.Handlers;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Persistence;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SimpleIdServer.OpenID.Tests.Api.AuthSchemeProvider.Handlers
{
    public class UpdateAuthSchemeProviderOptionsHandlerFixture
    {
        [Fact]
        public async Task When_Update_Request_Doesnt_Contain_Valid_Properties_Then_Exception_Is_Thrown()
        {
            // ARRANGE
            var repository = new Mock<IAuthenticationSchemeProviderRepository>();
            var logger = new Mock<ILogger<UpdateAuthSchemeProviderOptionsHandler>>();
            var handler = new UpdateAuthSchemeProviderOptionsHandler(repository.Object, logger.Object);
            var authenticationSchemeProvider = new AuthenticationSchemeProvider
            {
                OptionsFullQualifiedName = typeof(OptionsLite).AssemblyQualifiedName
            };
            repository.Setup(r => r.Get(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(authenticationSchemeProvider));
            var jObj = new JObject();
            jObj.Add("Property", "value");
            jObj.Add("InvalidProperty1", "value");
            jObj.Add("InvalidProperty2", "value");

            // ACT
            var exception = await Assert.ThrowsAsync<OAuthException>(() => handler.Handle("id", jObj, CancellationToken.None));

            // ASSERT
            Assert.NotNull(exception);
        }

        private class OptionsLite
        {
            public string Property { get; set; }
        }
    }
}
