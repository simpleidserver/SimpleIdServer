// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Api.Credential.Validators;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.Did;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public interface ICredentialConfigurationSerializer
{
    JsonObject Serialize(CredentialConfiguration template);
}

public class CredentialConfigurationSerializer : ICredentialConfigurationSerializer
{
    private readonly IEnumerable<ICredentialFormatter> _credentialSerializers;
    private readonly IEnumerable<IKeyProofTypeValidator> _keyProofTypeValidators;
    private readonly IEnumerable<IDidResolver> _didResolvers;

    public CredentialConfigurationSerializer(
        IEnumerable<ICredentialFormatter> credentialSerializers,
        IEnumerable<IKeyProofTypeValidator> keyProofTypeValidators,
        IEnumerable<IDidResolver> didResolvers)
    {
        _credentialSerializers = credentialSerializers;
        _keyProofTypeValidators = keyProofTypeValidators;
        _didResolvers = didResolvers;
    }

    public JsonObject Serialize(CredentialConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        var formatter = _credentialSerializers.Single(c => c.Format == configuration.Format);
        var result = formatter.ExtractCredentialIssuerMetadata(configuration);
        var cryptographicBindingMethodsSupported = new JsonArray();
        foreach (var didResolver in _didResolvers)
            cryptographicBindingMethodsSupported.Add($"{Did.Constants.Scheme}:{didResolver.Method}");
        result.Add("format", configuration.Format);
        result.Add("scope", configuration.Scope);
        result.Add("cryptographic_binding_methods_supported", cryptographicBindingMethodsSupported);
        result.Add("cryptographic_suites_supported", ""); // ES256 etc...
        result.Add("proof_types", SerializeProofTypes());
        result.Add("display", SerializeDisplay(configuration));
        return result;
    }

    private JsonArray SerializeProofTypes()
    {
        var result = new JsonArray();
        foreach (var proofType in _keyProofTypeValidators) result.Add(proofType.Type);
        return result;
    }

    private JsonArray SerializeDisplay(CredentialConfiguration configuration)
    {
        var result = new JsonArray();
        foreach(var display in configuration.Displays)
        {
            var record = new JsonObject
            {
                { "name", display.Name }
            };
            var logo = new JsonObject();
            if (!string.IsNullOrWhiteSpace(display.Locale))
                record.Add("locale", display.Locale);
            if(!string.IsNullOrWhiteSpace(display.LogoUrl))
                logo.Add("uri", display.LogoUrl);
            if (!string.IsNullOrWhiteSpace(display.LogoAltText))
                logo.Add("alt_text", display.LogoAltText);
            if(!string.IsNullOrWhiteSpace(display.Description))
                record.Add("description", display.Description);
            if (!string.IsNullOrWhiteSpace(display.BackgroundColor))
                record.Add("background_color", display.BackgroundColor);
            if (!string.IsNullOrWhiteSpace(display.TextColor))
                record.Add("text_color", display.TextColor);
            record.Add("logo", logo);
            result.Add(record);
        }

        return result;
    }
}