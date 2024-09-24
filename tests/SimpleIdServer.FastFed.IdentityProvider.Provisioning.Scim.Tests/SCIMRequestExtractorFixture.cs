// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim.Tests;
using SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;
using SimpleIdServer.FastFed.Provisioning.Scim;
using System.Text.Json.Nodes;

public class SCIMRequestExtractorFixture
{
    [Test]
    public void When_Extract_Attributes_Then_RepresentationIsReturned()
    {
        // ARRANGE
        var userJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "User.json"));
        var userObject = JsonObject.Parse(userJson).AsObject();
        var desiredAttributes = new SchemaGrammarDesiredAttributes
        {
            RequiredUserAttributes = new List<string> { "externalId", "name.formatted", "emails[primary eq true]", "name.givenName" },
            OptionalUserAttributes = new List<string>()
        };

        // ACT
        var result = SCIMRequestExtractor.ExtractAdd(userObject, desiredAttributes);

        // ASSERT
        Assert.IsFalse(result.HasError);
        Assert.AreEqual("98d78581-dd0d-4361-ab61-9511c6e5f035", result.Result["externalId"].ToString());
        Assert.AreEqual("Ms. Barbara J Jensen III", result.Result["name"]["formatted"].ToString());
        Assert.AreEqual("true", result.Result["emails"][0]["primary"].ToString());
        Assert.AreEqual("work", result.Result["emails"][0]["type"].ToString());
        Assert.AreEqual("dschrute@example.com", result.Result["emails"][0]["value"].ToString());
    }
}
