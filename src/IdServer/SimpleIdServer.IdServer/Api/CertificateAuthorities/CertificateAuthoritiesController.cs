// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.CertificateAuthorities;

public class CertificateAuthoritiesController : BaseController
{
    private readonly ICertificateAuthorityRepository _certificateAuthorityRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IJwtBuilder _jwtBuilder;
    private readonly IBusControl _busControl;
    private readonly ILogger<CertificateAuthoritiesController> _logger;

    public CertificateAuthoritiesController(ICertificateAuthorityRepository certificateAuthorityRepository, IRealmRepository realmRepository, IJwtBuilder jwtBuilder, IBusControl busControl, ILogger<CertificateAuthoritiesController> logger)
    {
        _certificateAuthorityRepository = certificateAuthorityRepository;
        _realmRepository = realmRepository;
        _jwtBuilder = jwtBuilder;
        _busControl = busControl;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            CheckAccessToken(prefix, Constants.StandardScopes.CertificateAuthorities.Name, _jwtBuilder);
            IQueryable<CertificateAuthority> query = _certificateAuthorityRepository.Query()
                .Include(p => p.Realms)
                .Where(p => p.Realms.Any(r => r.Name == prefix))
                .AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.Filter))
                query = query.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                query = query.OrderBy(request.OrderBy);
            else
                query = query.OrderByDescending(c => c.UpdateDateTime);

            var nb = query.Count();
            var cas = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            return new OkObjectResult(new SearchResult<CertificateAuthority>
            {
                Count = nb,
                Content = cas
            });
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public IActionResult Generate([FromRoute] string prefix, [FromBody] GenerateCertificateAuthorityRequest request)
    {
        CheckAccessToken(prefix, Constants.StandardScopes.CertificateAuthorities.Name, _jwtBuilder);
        if (request == null) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
        if (string.IsNullOrWhiteSpace(request.SubjectName)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CertificateAuthorityNames.SubjectName));
        var certificateAuthority = CertificateAuthorityBuilder.Create(request.SubjectName, numberOfDays: request.NumberOfDays).Build();
        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.Created,
            Content = JsonSerializer.Serialize(certificateAuthority).ToString(),
            ContentType = "application/json"
        };
    }

    [HttpPost]
    public IActionResult Import([FromRoute] string prefix, [FromBody] ImportCertificateAuthorityRequest request)
    {
        CheckAccessToken(prefix, Constants.StandardScopes.CertificateAuthorities.Name, _jwtBuilder);
        if (request == null) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
        if (string.IsNullOrWhiteSpace(request.FindValue)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CertificateAuthorityNames.FindValue));
        var store = new X509Store(request.StoreName, request.StoreLocation);
        try
        {
            store.Open(OpenFlags.ReadOnly);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.CANNOT_READ_CERTIFICATE_STORE);
        }

        var certificate = store.Certificates.Find(request.FindType, request.FindValue, true).FirstOrDefault();
        if(certificate == null) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.CERTIFICATE_DOESNT_EXIST);
        try
        {
            if (!certificate.HasPrivateKey || certificate.PrivateKey == null) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.CERTIFICATE_DOESNT_HAVE_PRIVATE_KEY);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.CERTIFICATE_DOESNT_HAVE_PRIVATE_KEY);
        }

        var certificateAuthority = CertificateAuthorityBuilder.Import(certificate, request.StoreLocation, request.StoreName, request.FindType, request.FindValue).Build();
        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.Created,
            Content = JsonSerializer.Serialize(certificateAuthority).ToString(),
            ContentType = "application/json"
        };
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] CertificateAuthority request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add certificate authority"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                CheckAccessToken(prefix, Constants.StandardScopes.CertificateAuthorities.Name, _jwtBuilder);
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
                activity?.SetTag("subjectName", request.SubjectName);
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
                var realm = await _realmRepository.Query().SingleAsync(r => r.Name == prefix);
                record.Realms.Add(realm);
                _certificateAuthorityRepository.Add(record);
                await _certificateAuthorityRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Certificate authority {request.SubjectName} added");
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
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new AddCertificateAuthorityFailureEvent
                {
                    Realm = prefix,
                    SubjectName = request?.SubjectName
                });
                return BuildError(ex);
            }
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Remove([FromRoute] string prefix, string id)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove certificate authority"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                CheckAccessToken(prefix, Constants.StandardScopes.CertificateAuthorities.Name, _jwtBuilder);
                var ca = await _certificateAuthorityRepository.Query().Include(c => c.Realms).SingleOrDefaultAsync(c => c.Id == id && c.Realms.Any(r => r.Name == prefix));
                if (ca == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CA, id));
                _certificateAuthorityRepository.Delete(ca);
                await _certificateAuthorityRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Certificate authority {id} removed");
                await _busControl.Publish(new RemoveCertificateAuthoritySuccessEvent
                {
                    Realm = prefix,
                    Id = id
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new RemoveCertificateAuthorityFailureEvent
                {
                    Realm = prefix,
                    Id = id
                });
                return BuildError(ex);
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id)
    {
        CheckAccessToken(prefix, Constants.StandardScopes.CertificateAuthorities.Name, _jwtBuilder);
        var ca = await _certificateAuthorityRepository.Query().Include(c => c.Realms).Include(c => c.ClientCertificates).AsNoTracking().SingleOrDefaultAsync(c => c.Id == id && c.Realms.Any(r => r.Name == prefix));
        if (ca == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CA, id));
        return new OkObjectResult(ca);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveClientCertificate([FromRoute] string prefix, string id, string clientCertificateId)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove client certificate"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                CheckAccessToken(prefix, Constants.StandardScopes.CertificateAuthorities.Name, _jwtBuilder);
                var ca = await _certificateAuthorityRepository.Query().Include(c => c.Realms).Include(c => c.ClientCertificates).SingleOrDefaultAsync(c => c.Id == id && c.Realms.Any(r => r.Name == prefix));
                if (ca == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CA, id));
                var clientCertificate = ca.ClientCertificates.SingleOrDefault(c => c.Id == clientCertificateId);
                if (clientCertificate == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CLIENT_CA, id));
                ca.ClientCertificates.Remove(clientCertificate);
                await _certificateAuthorityRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Client certificate {clientCertificateId} removed");
                await _busControl.Publish(new RemoveClientCertificateSuccessEvent
                {
                    Realm = prefix,
                    CAId = id,
                    ClientCertificateId = clientCertificateId
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new RemoveClientCertificateFailureEvent
                {
                    Realm = prefix,
                    CAId = id,
                    ClientCertificateId = clientCertificateId
                });
                return BuildError(ex);
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddClientCertificate([FromRoute] string prefix, string id, [FromBody] AddClientCertificateRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add client certificate authority"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                CheckAccessToken(prefix, Constants.StandardScopes.CertificateAuthorities.Name, _jwtBuilder);
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
                if (string.IsNullOrWhiteSpace(request.SubjectName)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, ClientCertificateNames.SubjectName));
                activity?.SetTag("subjectName", request.SubjectName);
                var ca = await _certificateAuthorityRepository.Query().Include(c => c.Realms).Include(c => c.ClientCertificates).SingleAsync(c => c.Id == id && c.Realms.Any(r => r.Name == prefix));
                if (ca == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CA, id));
                var store = new Stores.CertificateAuthorityStore(null);
                var certificate = store.Get(ca);
                var pem = KeyGenerator.GenerateClientCertificate(certificate, request.SubjectName, request.NbDays);
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
                await _certificateAuthorityRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Certificate authority client {request.SubjectName} added");
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
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
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
}