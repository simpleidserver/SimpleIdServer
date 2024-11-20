﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Vc;

public class VcBuilder
{
    private readonly W3CVerifiableCredential _credential;

    public VcBuilder(W3CVerifiableCredential credential)
    {
        _credential = credential;
    }

    public static VcBuilder New(string id,
        string jsonLdContext,
        string issuer, 
        string type,
        List<string> additionalTypes,
        DateTime? validFrom = null, 
        DateTime? validUntil = null
        )
    {
        var credential = new W3CVerifiableCredential
        {
            Id = id,
            Issuer = issuer,
            ValidFrom = validFrom,
            ValidUntil = validUntil,
            IssuanceDate = DateTime.UtcNow.Date,
            Issued = DateTime.UtcNow.Date
        };
        credential.Context.Add(VcConstants.VerifiableCredentialJsonLdContext);
        credential.Context.Add(jsonLdContext);
        credential.Type.Add(VcConstants.VerifiableCredentialType);
        if (additionalTypes != null)
            credential.Type.AddRange(additionalTypes);
        credential.Type.Add(type);
        return new VcBuilder(credential);
    }

    public VcBuilder AddCredentialSubject(Action<CredentialSubjectBuilder> action)
        => AddCredentialSubject(null, action);

    public VcBuilder AddCredentialSubject(string id, Action<CredentialSubjectBuilder> action)
    {
        var builder = CredentialSubjectBuilder.New(id);
        action(builder);
        var record = builder.Build();
        if (_credential.CredentialSubject == null)
            _credential.CredentialSubject = record;
        else
        {
            var existingCredentialSubject = _credential.CredentialSubject as JsonObject;
            if (existingCredentialSubject != null) _credential.CredentialSubject = new JsonArray(JsonObject.Parse(existingCredentialSubject.ToJsonString()), record);
            else (_credential.CredentialSubject as JsonArray).Add(record);
        }

        return this;
    }

    public VcBuilder SetSchema(string id, string type)
    {
        _credential.Schema = new W3CVerifiableCredentialSchema
        {
            Id  = id,
            Type = type
        };
        return this;
    }

    public W3CVerifiableCredential Build() => _credential;
}