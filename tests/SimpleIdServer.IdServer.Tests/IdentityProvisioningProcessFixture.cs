// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Linq;

namespace SimpleIdServer.IdServer.Tests;

public class IdentityProvisioningProcessFixture
{
    [Test]
    public void When_Launch_Process_Then_Process_Is_Created()
    {
        // Arrange
        var provisioning = new IdentityProvisioning
        {
            Id = "test",
            Definition = new IdentityProvisioningDefinition { Name = "test" }
        };

        // Act
        var processId = provisioning.Launch();

        // Assert
        Assert.That(provisioning.Processes.Count, Is.EqualTo(1));
        var process = provisioning.Processes.First();
        Assert.That(process.Id, Is.EqualTo(processId));
        Assert.That(process.CreateDateTime, Is.Not.EqualTo(default(DateTime)));
        Assert.That(provisioning.Histories.Count, Is.EqualTo(1));
    }

    [Test]
    public void When_Start_Extract_Then_Process_Is_Updated()
    {
        // Arrange
        var provisioning = new IdentityProvisioning
        {
            Id = "test",
            Definition = new IdentityProvisioningDefinition { Name = "test" }
        };
        var processId = provisioning.Launch();

        // Act
        provisioning.Start(processId, 10);

        // Assert
        var process = provisioning.Processes.First();
        Assert.That(process.StartExportDateTime, Is.Not.Null);
        Assert.That(process.TotalPageToExtract, Is.EqualTo(10));
        Assert.That(provisioning.Histories.Count, Is.EqualTo(2));
    }

    [Test]
    public void When_Extract_Pages_Then_Process_Accumulates_Stats()
    {
        // Arrange
        var provisioning = new IdentityProvisioning
        {
            Id = "test",
            Definition = new IdentityProvisioningDefinition { Name = "test" }
        };
        var processId = provisioning.Launch();
        provisioning.Start(processId, 3);

        // Act
        provisioning.Extract(processId, 1, 5, 2, 0);
        provisioning.Extract(processId, 2, 3, 1, 1);
        provisioning.Extract(processId, 3, 4, 0, 2);

        // Assert
        var process = provisioning.Processes.First();
        Assert.That(process.NbExtractedPages, Is.EqualTo(3));
        Assert.That(process.NbExtractedUsers, Is.EqualTo(12)); // 5 + 3 + 4
        Assert.That(process.NbExtractedGroups, Is.EqualTo(3)); // 2 + 1 + 0
        Assert.That(process.NbFilteredRepresentations, Is.EqualTo(3)); // 0 + 1 + 2
        Assert.That(provisioning.Histories.Count, Is.EqualTo(5)); // Launch + Start + 3 extracts
    }

    [Test]
    public void When_FinishExtract_Then_Process_Has_EndDate()
    {
        // Arrange
        var provisioning = new IdentityProvisioning
        {
            Id = "test",
            Definition = new IdentityProvisioningDefinition { Name = "test" }
        };
        var processId = provisioning.Launch();
        provisioning.Start(processId, 1);
        provisioning.Extract(processId, 1, 5, 2, 0);

        // Act
        provisioning.FinishExtract(processId);

        // Assert
        var process = provisioning.Processes.First();
        Assert.That(process.IsExported, Is.True);
        Assert.That(process.EndExportDateTime, Is.Not.Null);
    }

    [Test]
    public void When_Import_Pages_Then_Process_Accumulates_Import_Stats()
    {
        // Arrange
        var provisioning = new IdentityProvisioning
        {
            Id = "test",
            Definition = new IdentityProvisioningDefinition { Name = "test" }
        };
        var processId = provisioning.Launch();
        provisioning.Start(processId, 1);
        provisioning.Extract(processId, 1, 5, 2, 0);
        provisioning.FinishExtract(processId);

        // Act
        provisioning.Import(processId, 2);
        provisioning.Import(processId, 3, 1, 1);
        provisioning.Import(processId, 2, 0, 2);

        // Assert
        var process = provisioning.Processes.First();
        Assert.That(process.StartImportDateTime, Is.Not.Null);
        Assert.That(process.TotalPageToImport, Is.EqualTo(2));
        Assert.That(process.NbImportedPages, Is.EqualTo(2));
        Assert.That(process.NbImportedUsers, Is.EqualTo(5)); // 3 + 2
        Assert.That(process.NbImportedGroups, Is.EqualTo(1)); // 1 + 0
    }

    [Test]
    public void When_RefreshProjections_Then_Process_Is_Rebuilt_From_Histories()
    {
        // Arrange
        var provisioning = new IdentityProvisioning
        {
            Id = "test",
            Definition = new IdentityProvisioningDefinition { Name = "test" }
        };
        var processId = provisioning.Launch();
        provisioning.Start(processId, 2);
        provisioning.Extract(processId, 1, 5, 2, 0);
        provisioning.Extract(processId, 2, 3, 1, 1);
        provisioning.FinishExtract(processId);

        // Clear the processes to simulate loading from database without processes
        provisioning.Processes.Clear();

        // Act
        provisioning.RefreshProjections();

        // Assert
        Assert.That(provisioning.Processes.Count, Is.EqualTo(1));
        var process = provisioning.Processes.First();
        Assert.That(process.Id, Is.EqualTo(processId));
        Assert.That(process.NbExtractedPages, Is.EqualTo(2));
        Assert.That(process.NbExtractedUsers, Is.EqualTo(8)); // 5 + 3
        Assert.That(process.NbExtractedGroups, Is.EqualTo(3)); // 2 + 1
        Assert.That(process.NbFilteredRepresentations, Is.EqualTo(1)); // 0 + 1
        Assert.That(process.IsExported, Is.True);
    }

    [Test]
    public void When_Multiple_Processes_Then_RefreshProjections_Handles_All()
    {
        // Arrange
        var provisioning = new IdentityProvisioning
        {
            Id = "test",
            Definition = new IdentityProvisioningDefinition { Name = "test" }
        };
        var processId1 = provisioning.Launch();
        provisioning.Start(processId1, 1);
        provisioning.Extract(processId1, 1, 5, 2, 0);
        provisioning.FinishExtract(processId1);

        var processId2 = provisioning.Launch();
        provisioning.Start(processId2, 1);
        provisioning.Extract(processId2, 1, 3, 1, 0);

        // Clear the processes to simulate refresh
        provisioning.Processes.Clear();

        // Act
        provisioning.RefreshProjections();

        // Assert
        Assert.That(provisioning.Processes.Count, Is.EqualTo(2));
        
        var process1 = provisioning.Processes.First(p => p.Id == processId1);
        Assert.That(process1.IsExported, Is.True);
        Assert.That(process1.NbExtractedUsers, Is.EqualTo(5));

        var process2 = provisioning.Processes.First(p => p.Id == processId2);
        Assert.That(process2.IsExported, Is.False);
        Assert.That(process2.NbExtractedUsers, Is.EqualTo(3));
    }
}
