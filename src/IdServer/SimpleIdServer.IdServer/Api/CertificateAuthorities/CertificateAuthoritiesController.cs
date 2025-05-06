// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.CertificateAuthorities;

public class CertificateAuthoritiesController : BaseController
{
    private readonly ICertificateAuthorityRepository _certificateAuthorityRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IBusControl _busControl;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly ILogger<CertificateAuthoritiesController> _logger;

    public CertificateAuthoritiesController(
        ICertificateAuthorityRepository certificateAuthorityRepository, 
        IRealmRepository realmRepository, 
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        ITransactionBuilder transactionBuilder,
        IBusControl busControl, ILogger<CertificateAuthoritiesController> logger) : base(tokenRepository, jwtBuilder)
    {
        _certificateAuthorityRepository = certificateAuthorityRepository;
        _realmRepository = realmRepository;
        _busControl = busControl;
        _transactionBuilder = transactionBuilder;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.CertificateAuthorities.Name);
            var result = await _certificateAuthorityRepository.Search(prefix, request, cancellationToken);
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromRoute] string prefix, [FromBody] GenerateCertificateAuthorityRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.CertificateAuthorities.Name);
            if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
            if (string.IsNullOrWhiteSpace(request.SubjectName)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, CertificateAuthorityNames.SubjectName));
            CertificateAuthority certificateAuthority = null;
            try
            {
                certificateAuthority = CertificateAuthorityBuilder.Create(request.SubjectName, numberOfDays: request.NumberOfDays).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.CertificateCannotBeGenerated);
            }
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(certificateAuthority).ToString(),
                ContentType = "application/json"
            };
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Import([FromRoute] string prefix, [FromBody] ImportCertificateAuthorityRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.CertificateAuthorities.Name);
            if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
            if (string.IsNullOrWhiteSpace(request.FindValue)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, CertificateAuthorityNames.FindValue));
            var store = new X509Store(request.StoreName, request.StoreLocation);
            try
            {
                store.Open(OpenFlags.ReadOnly);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.CannotReadCertificateStore);
            }

            var certificate = store.Certificates.Find(request.FindType, request.FindValue, true).FirstOrDefault();
            if (certificate == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.CertificateDoesntExistInvalid);
            try
            {
                if (!certificate.HasPrivateKey || certificate.PrivateKey == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.CertificateDoesntHavePrivateKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.CertificateDoesntHavePrivateKey);
            }

            var certificateAuthority = CertificateAuthorityBuilder.Import(certificate, request.StoreLocation, request.StoreName, request.FindType, request.FindValue).Build();
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(certificateAuthority).ToString(),
                ContentType = "application/json"
            };
        }
        catch(CryptographicException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] CertificateAuthority request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.CertificateAuthorities.Name);
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                var id = Guid.NewGuid().ToString();
                var record = new CertificateAuthority
                {
                    EndDateTime = request.EndDateTime,
                    FindType = request.FindType,
                    FindValue = request.FindValue,
                    Id = id,
                    PrivateKey = request.PrivateKey,
                    PublicKey = request.PublicKey,
                    Source = request.Source,
                    StartDateTime = request.StartDateTime,
                    StoreLocation = request.StoreLocation,
                    SubjectName = request.SubjectName,
                    StoreName = request.StoreName,
                    UpdateDateTime = DateTime.UtcNow
                };
                var realm = await _realmRepository.Get(prefix, cancellationToken);
                record.Realms.Add(realm);
                _certificateAuthorityRepository.Add(record);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new AddCertificateAuthoritySuccessEvent
                {
                    Realm = prefix,
                    SubjectName = request.SubjectName
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(record).ToString(),
                    ContentType = "application/json"
                };
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new AddCertificateAuthorityFailureEvent
            {
                Realm = prefix,
                SubjectName = request?.SubjectName
            });
            return BuildError(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Remove([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.CertificateAuthorities.Name);
                var ca = await _certificateAuthorityRepository.Get(prefix, id, cancellationToken);
                if (ca == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownCa, id));
                _certificateAuthorityRepository.Delete(ca);
                await transaction.Commit(cancellationToken);
                await _busControl.Publish(new RemoveCertificateAuthoritySuccessEvent
                {
                    Realm = prefix,
                    Id = id
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new RemoveCertificateAuthorityFailureEvent
            {
                Realm = prefix,
                Id = id
            });
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.CertificateAuthorities.Name);
            var ca = await _certificateAuthorityRepository.Get(prefix, id, cancellationToken);
            if (ca == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownCa, id));
            return new OkObjectResult(ca);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveClientCertificate([FromRoute] string prefix, string id, string clientCertificateId, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.CertificateAuthorities.Name);
                var ca = await _certificateAuthorityRepository.Get(prefix, id, cancellationToken);
                if (ca == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownCa, id));
                var clientCertificate = ca.ClientCertificates.SingleOrDefault(c => c.Id == clientCertificateId);
                if (clientCertificate == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownClientCa, id));
                ca.ClientCertificates.Remove(clientCertificate);
                await transaction.Commit(CancellationToken.None);
                await _busControl.Publish(new RemoveClientCertificateSuccessEvent
                {
                    Realm = prefix,
                    CAId = id,
                    ClientCertificateId = clientCertificateId
                });
                return new NoContentResult();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new RemoveClientCertificateFailureEvent
            {
                Realm = prefix,
                CAId = id,
                ClientCertificateId = clientCertificateId
            });
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddClientCertificate([FromRoute] string prefix, string id, [FromBody] AddClientCertificateRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, Config.DefaultScopes.CertificateAuthorities.Name);
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                if (string.IsNullOrWhiteSpace(request.SubjectName)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, ClientCertificateNames.SubjectName));
                var ca = await _certificateAuthorityRepository.Get(prefix, id, cancellationToken);
                if (ca == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownCa, id));
                var store = new Stores.CertificateAuthorityStore(null);
                var certificate = store.Get(ca);
                PemResult pem = null;
                try
                {
                    pem = KeyGenerator.GenerateClientCertificate(certificate, request.SubjectName, request.NbDays);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.CertificateClientCannotBeGenerated);
                }

                var record = new ClientCertificate
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = request.SubjectName,
                    PublicKey = pem.PublicKey,
                    PrivateKey = pem.PrivateKey,
                    StartDateTime = DateTime.UtcNow,
                    EndDateTime = DateTime.UtcNow.AddDays(request.NbDays)
                };
                ca.UpdateDateTime = DateTime.UtcNow;
                ca.ClientCertificates.Add(record);
                _certificateAuthorityRepository.Update(ca);
                await transaction.Commit(CancellationToken.None);
                await _busControl.Publish(new AddClientCertificateAuthoritySuccessEvent
                {
                    Realm = prefix,
                    SubjectName = request.SubjectName,
                    CAId = id,
                    NbDays = request.NbDays
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(record).ToString(),
                    ContentType = "application/json"
                };
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            await _busControl.Publish(new AddClientCertificateAuthorityFailureEvent
            {
                Realm = prefix,
                SubjectName = request?.SubjectName,
                CAId = id,
                NbDays = request?.NbDays
            });
            return BuildError(ex);
        }
    }
}