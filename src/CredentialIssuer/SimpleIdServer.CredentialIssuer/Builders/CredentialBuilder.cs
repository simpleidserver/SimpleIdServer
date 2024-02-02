// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;
using System;

namespace SimpleIdServer.CredentialIssuer.Builders;

public class CredentialBuilder
{
    private readonly Credential _credential;

    private CredentialBuilder(Credential credential)
    {
        _credential = credential;
    }

    public static CredentialBuilder New(string credentialId, CredentialConfiguration configuration, string subject, DateTime issueDateTime, DateTime? expirationDateTime = null)
        => new CredentialBuilder(new Credential
        {
            Id = Guid.NewGuid().ToString(),
            CredentialId = credentialId,
            Subject = subject,
            CredentialConfigurationId = configuration.Id,
            IssueDateTime = DateTime.UtcNow,
            ExpirationDateTime = expirationDateTime
        });

    public Credential Build() => _credential;
}
