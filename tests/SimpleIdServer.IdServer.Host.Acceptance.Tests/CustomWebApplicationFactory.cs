// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Host.Acceptance.Tests.Middlewares;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Builders;
using SimpleIdServer.OpenidFederation.Domains;
using SimpleIdServer.OpenidFederation.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SimpleIdServer.IdServer.Host.Acceptance.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly ScenarioContext _scenarioContext;

        public CustomWebApplicationFactory(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.ConfigureTestServices(s =>
            {
                s.AddAuthentication(CustomAuthenticationHandler.AuthenticationScheme)
                    .AddScheme<CustomAuthenticationHandlerOptions, CustomAuthenticationHandler>(CustomAuthenticationHandler.AuthenticationScheme, options => {
                        options.ScenarioContext = _scenarioContext;
                    });
                s.AddSingleton<IStartupFilter>(new CertificateStartupFilter(_scenarioContext));
                s.RemoveAll<Helpers.IHttpClientFactory>();
                var mo = new Mock<Helpers.IHttpClientFactory>();
                mo.Setup(m => m.GetHttpClient())
                    .Returns(() =>
                    {
                        var fakeHttpMessageHandler = new FakeHttpMessageHandler(_scenarioContext);
                        return new HttpClient(fakeHttpMessageHandler);
                    });
                mo.Setup(m => m.GetHttpClient(It.IsAny<HttpClientHandler>()))
                    .Returns(() =>
                    {
                        var fakeHttpMessageHandler = new FakeHttpMessageHandler(_scenarioContext);
                        return new HttpClient(fakeHttpMessageHandler);
                    });
                s.AddSingleton<Helpers.IHttpClientFactory>(mo.Object);
            });
        }
    }

    public class CertificateStartupFilter : IStartupFilter
    {
        private readonly ScenarioContext _scenarioContext;

        public CertificateStartupFilter(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.Use((ctx, nxt) =>
                {
                    if (ctx.Request.Headers.TryGetValue("X-Testing-ClientCert", out var key))
                    {
                        var certificate = _scenarioContext.Get<X509Certificate2>(key);
                        ctx.Connection.ClientCertificate = certificate;
                    }
                    return nxt();
                });
                next(builder);
            };
        }
    }

    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private RpFederationEntityBuilder _rpFederationEntityBuilder;
        private AuthorityFederationEntityBuilder _authorityFederationEntityBuilder;
        private RpFederationOptions _rpOpts;
        private RpFederationOptions _taOpts;
        private readonly ScenarioContext _scenarioContext;

        public FakeHttpMessageHandler(
            ScenarioContext scenarioContext)
        {
            var firstJsonWebKey = OAuth.Host.Acceptance.Tests.IdServerConfiguration.RpSigningCredential.SerializePublicJWK();
            firstJsonWebKey.Alg = SecurityAlgorithms.RsaSha256;
            firstJsonWebKey.Use = "sig";
            var secondJsonWebKey = OAuth.Host.Acceptance.Tests.IdServerConfiguration.RpJwtSigningCredential.SerializePublicJWK();
            secondJsonWebKey.Alg = SecurityAlgorithms.RsaSha256;
            secondJsonWebKey.Use = "sig";
            var client = new Domains.Client
            {
                ClientId = "http://rp.com",
                ClientRegistrationTypesSupported = new List<string>
                {
                    ClientRegistrationMethods.Automatic
                },
                ApplicationType = "web",
                RedirectionUrls = new List<string>
                {
                    "https://openid.sunet.se/rp/callback"
                },
                RequestObjectSigningAlg = SecurityAlgorithms.RsaSha256,
                Scopes = new List<Scope>
                {
                    new Scope
                    {
                        Name = "openid"
                    },
                    new Scope
                    {
                        Name = "profile"
                    }
                },
                ResponseTypes = new List<string>
                {
                    "code"
                },
                GrantTypes = new List<string>
                {
                    "authorization_code"
                },
                IsConsentDisabled = true,
                TokenEndPointAuthMethod = "private_key_jwt"
            };
            client.Add(OAuth.Host.Acceptance.Tests.IdServerConfiguration.RpSigningCredential.Kid, firstJsonWebKey, "sig", SecurityKeyTypes.RSA);
            client.Add(OAuth.Host.Acceptance.Tests.IdServerConfiguration.RpJwtSigningCredential.Kid, secondJsonWebKey, "sig", SecurityKeyTypes.RSA);
            _scenarioContext = scenarioContext;
            _rpOpts = new RpFederationOptions
            {
                Client = client,
                IsFederationEnabled = false,
                OrganizationName = null,
                SigningCredentials = OAuth.Host.Acceptance.Tests.IdServerConfiguration.RpSigningCredential
            };
            _taOpts = new RpFederationOptions
            {
                OrganizationName = "ta",
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "taKeyId" }, SecurityAlgorithms.RsaSha256)
            };
            _rpFederationEntityBuilder = new RpFederationEntityBuilder(Microsoft.Extensions.Options.Options.Create(_rpOpts), new FakeRPFederationEntityStore());
            _authorityFederationEntityBuilder = new AuthorityFederationEntityBuilder(Microsoft.Extensions.Options.Options.Create(_taOpts), new FakeTAFederationEntityStore());
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri.AbsoluteUri;
            if(uri == "http://localhost/notificationedp")
            {
                var j = await request.Content.ReadAsStringAsync();
                _scenarioContext.Set(j, "notificationResponse");
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
            }

            if(uri == "http://rp.com/.well-known/openid-federation")
            {
                return await GetRPOpenidFederation();
            }            

            if(uri == "http://ta.com/.well-known/openid-federation")
            {
                return await GetTAOpenidFederation();
            }

            if(uri == "http://ta.com/federation_fetch?iss=http://ta.com&sub=http://rp.com")
            {
                return await FetchRPFromTA();
            }

            var redirectUrls = new List<string>
            {
                "http://a.domain.com",
                "http://b.domain.com"
            };
            var json = JsonSerializer.Serialize(redirectUrls);
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private async Task<HttpResponseMessage> GetRPOpenidFederation()
        {
            var result = await _rpFederationEntityBuilder.BuildSelfIssued(new OpenidFederation.Builders.BuildFederationEntityRequest
            {
                Credential = _rpOpts.SigningCredentials,
                Issuer = "http://rp.com",
                Realm = null
            }, CancellationToken.None);
            var handler = new JsonWebTokenHandler();
            var jws = handler.CreateToken(JsonSerializer.Serialize(result), new SigningCredentials(_rpOpts.SigningCredentials.Key, _rpOpts.SigningCredentials.Algorithm));
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jws, System.Text.Encoding.UTF8, OpenidFederationConstants.EntityStatementContentType)
            };
        }

        private async Task<HttpResponseMessage> GetTAOpenidFederation()
        {
            var result = await _authorityFederationEntityBuilder.BuildSelfIssued(new OpenidFederation.Builders.BuildFederationEntityRequest
            {
                Credential = _taOpts.SigningCredentials,
                Issuer = "http://ta.com",
                Realm = null
            }, CancellationToken.None);
            var handler = new JsonWebTokenHandler();
            var jws = handler.CreateToken(JsonSerializer.Serialize(result), new SigningCredentials(_taOpts.SigningCredentials.Key, _taOpts.SigningCredentials.Algorithm));
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jws, System.Text.Encoding.UTF8, OpenidFederationConstants.EntityStatementContentType)
            };
        }

        private async Task<HttpResponseMessage> FetchRPFromTA()
        {
            var result = await _rpFederationEntityBuilder.BuildSelfIssued(new OpenidFederation.Builders.BuildFederationEntityRequest
            {
                Credential = _rpOpts.SigningCredentials,
                Issuer = "http://rp.com",
                Realm = null
            }, CancellationToken.None); 
            var openidFederation = new OpenidFederationResult
            {
                Iat = result.Iat,
                Exp = result.Exp,
                Iss = "http://ta.com",
                Sub = result.Sub,
                Jwks = result.Jwks
            };
            var handler = new JsonWebTokenHandler();
            var jws = handler.CreateToken(JsonSerializer.Serialize(openidFederation), new SigningCredentials(_taOpts.SigningCredentials.Key, _taOpts.SigningCredentials.Algorithm));
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jws, System.Text.Encoding.UTF8, OpenidFederationConstants.EntityStatementContentType)
            };
        }
    }

    public class RpFederationOptions
    {
        /// <summary>
        /// Enable or disable federation.
        /// </summary>
        public bool IsFederationEnabled { get; set; }
        /// <summary>
        /// Organization name.
        /// </summary>
        public string OrganizationName { get; set; }
        /// <summary>
        /// Key used to sign the response.
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
        /// <summary>
        /// OPENID client of the relying party.
        /// </summary>
        public Client Client { get; set; }
    }

    public class RpFederationEntityBuilder : BaseFederationEntityBuilder
    {
        private readonly RpFederationOptions _options;

        public RpFederationEntityBuilder(IOptions<RpFederationOptions> options, IFederationEntityStore federationEntityStore) : base(federationEntityStore)
        {
            _options = options.Value;
        }

        protected override bool IsFederationEnabled => _options.IsFederationEnabled;

        protected override string OrganizationName => _options.OrganizationName;

        protected override Task EnrichSelfIssued(OpenidFederationResult federationEntity, BuildFederationEntityRequest request, CancellationToken cancellationToken)
        {
            if (federationEntity.Metadata.OtherParameters == null)
                federationEntity.Metadata.OtherParameters = new Dictionary<string, JsonObject>();
            var client = JsonObject.Parse(JsonSerializer.Serialize(_options.Client)).AsObject();
            federationEntity.Metadata.OtherParameters.Add("openid_relying_party", client);
            return Task.CompletedTask;
        }
    }

    public class AuthorityFederationEntityBuilder : BaseFederationEntityBuilder
    {
        private readonly RpFederationOptions _options;

        public AuthorityFederationEntityBuilder(
            IOptions<RpFederationOptions> options,
            IFederationEntityStore federationEntityStore) : base(federationEntityStore)
        {
            _options = options.Value;
        }

        protected override bool IsFederationEnabled => true;

        protected override string OrganizationName => _options.OrganizationName;

        protected override Task EnrichSelfIssued(OpenidFederationResult federationEntity, BuildFederationEntityRequest request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class FakeRPFederationEntityStore : IFederationEntityStore
    {
        public Task<List<FederationEntity>> GetAllAuthorities(string realm, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<FederationEntity>
            {
                new FederationEntity
                {
                    Sub = "http://ta.com",
                    IsSubordinate = false
                }
            });
        }

        public Task<List<FederationEntity>> GetAllSubordinates(string realm, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<FederationEntity>
            {
            });
        }

        public Task<FederationEntity> GetSubordinate(string sub, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeTAFederationEntityStore : IFederationEntityStore
    {
        public Task<List<FederationEntity>> GetAllAuthorities(string realm, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<FederationEntity>
            {
            });
        }

        public Task<List<FederationEntity>> GetAllSubordinates(string realm, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<FederationEntity>
            {
                new FederationEntity
                {
                    Sub = "http://rp.com",
                    IsSubordinate = true
                }
            });
        }

        public Task<FederationEntity> GetSubordinate(string sub, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
