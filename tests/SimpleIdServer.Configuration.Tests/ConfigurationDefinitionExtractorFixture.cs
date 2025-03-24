// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Configuration.Models;
using System.Globalization;

namespace SimpleIdServer.Configuration.Tests;

public class ConfigurationDefinitionExtractorFixture
{
    [Test]
    public void When_Extract_UserOptions_To_ConfigurationDefinition_Then_ExtractionIsCorrect()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en");
        // ACT
        var configurationDefinition = ConfigurationDefinitionExtractor.Extract(typeof(UserOptions));

        // ASSERT
        var firstNameDefinition = configurationDefinition.Records.Single(r => r.Name == nameof(UserOptions.FirstName));
        var genderDefinition = configurationDefinition.Records.Single(r => r.Name == nameof(UserOptions.Gender));
        var statusDefinition = configurationDefinition.Records.Single(r => r.Name == nameof(UserOptions.Status));
        var isActiveDefinition = configurationDefinition.Records.Single(r => r.Name == nameof(UserOptions.IsActive));
        var ageDefinition = configurationDefinition.Records.Single(r => r.Name == nameof(UserOptions.Age));
        var passwordDefinition = configurationDefinition.Records.Single(r => r.Name == nameof(UserOptions.Password));
        var birthDateDefinition = configurationDefinition.Records.Single(r => r.Name == nameof(UserOptions.BirthDateTime));
        var genderValues = genderDefinition.Values;
        var statusValues = statusDefinition.Values;

        Assert.That(firstNameDefinition.Name, Is.EqualTo(nameof(UserOptions.FirstName)));
        Assert.That(genderDefinition.Name, Is.EqualTo(nameof(UserOptions.Gender)));
        Assert.That(statusDefinition.Name, Is.EqualTo(nameof(UserOptions.Status)));
        Assert.That(isActiveDefinition.Name, Is.EqualTo(nameof(UserOptions.IsActive)));
        Assert.That(ageDefinition.Name, Is.EqualTo(nameof(UserOptions.Age)));
        Assert.That(passwordDefinition.Name, Is.EqualTo(nameof(UserOptions.Password)));
        Assert.That(birthDateDefinition.Name, Is.EqualTo(nameof(UserOptions.BirthDateTime)));

        Assert.That(firstNameDefinition.Type, Is.EqualTo(ConfigurationDefinitionRecordTypes.INPUT));
        Assert.That(genderDefinition.Type, Is.EqualTo(ConfigurationDefinitionRecordTypes.SELECT));
        Assert.That(statusDefinition.Type, Is.EqualTo(ConfigurationDefinitionRecordTypes.MULTISELECT));
        Assert.That(isActiveDefinition.Type, Is.EqualTo(ConfigurationDefinitionRecordTypes.CHECKBOX));
        Assert.That(ageDefinition.Type, Is.EqualTo(ConfigurationDefinitionRecordTypes.NUMBER));
        Assert.That(passwordDefinition.Type, Is.EqualTo(ConfigurationDefinitionRecordTypes.PASSWORD));
        Assert.That(birthDateDefinition.Type, Is.EqualTo(ConfigurationDefinitionRecordTypes.DATETIME));

        Assert.That(firstNameDefinition.DisplayName, Is.EqualTo("FirstName"));
        Assert.That(firstNameDefinition.Description, Is.EqualTo("Description"));
        Assert.That(genderDefinition.DisplayName, Is.EqualTo("Gender"));
        Assert.That(genderDefinition.Description, Is.EqualTo("Gender"));
        Assert.That(statusDefinition.DisplayName, Is.EqualTo("Status"));
        Assert.That(statusDefinition.Description, Is.EqualTo("Status"));
        Assert.That(isActiveDefinition.DisplayName, Is.EqualTo("IsActive"));
        Assert.That(isActiveDefinition.Description, Is.EqualTo("IsActive"));
        Assert.That(ageDefinition.DisplayName, Is.EqualTo("Age"));
        Assert.That(ageDefinition.Description, Is.EqualTo("Age"));

        Assert.That(genderValues.Count(), Is.EqualTo(2));
        Assert.That(genderValues.First().Name, Is.EqualTo("Male"));
        Assert.That(genderValues.Last().Name, Is.EqualTo("Female"));
        Assert.That(genderValues.First().Value, Is.EqualTo("MAL"));
        Assert.That(genderValues.Last().Value, Is.EqualTo("SEX"));

        Assert.That(statusValues.Count(), Is.EqualTo(2));
        Assert.That(statusValues.First().Name, Is.EqualTo("Freelance"));
        Assert.That(statusValues.Last().Name, Is.EqualTo("Employee"));
        Assert.That(statusValues.First().Value, Is.EqualTo("FREELANCE"));
        Assert.That(statusValues.Last().Value, Is.EqualTo("EMPLOYEE"));
    }
}
