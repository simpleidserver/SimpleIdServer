// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Builders;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Store.EF;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations.AfterDeployment;

public class FormAndWorkflowDataSeeder : EfdataSeeder.DataSeeder<StoreDbContext, StoreDbContext>
{
    private static Dictionary<string, string> mappingManualProvisioningToRegistrationWorkflow = new Dictionary<string, string>
    {
        { "email", StandardEmailRegisterWorkflows.workflowId },
        { "sms", StandardSmsRegisterWorkflows.workflowId },
        { "pwd", StandardPwdRegistrationWorkflows.workflowId },
        { "webauthn", StandardFidoRegistrationWorkflows.webauthWorkflowId },
        { "mobile", StandardFidoRegistrationWorkflows.mobileWorkflowId },
        { "vp", StandardVpRegistrationWorkflows.workflowId },
        { "complex", DataSeeder.BuildComplexRegistrationWorkflow().Id }
    };

    public FormAndWorkflowDataSeeder(StoreDbContext dbContext) : base(dbContext)
    {
        
    }

    protected override string Name => "FormAndWorkflowDataSeeder";
    public override bool IsBeforeDeployment => false;

    protected override async Task Up(CancellationToken cancellationToken)
    {
        await UpdateAcrs(cancellationToken);
        await UpdateManualProvisioning(cancellationToken);
    }

    private async Task UpdateAcrs(CancellationToken cancellationToken)
    {
        // Fetch standard ACRS from the master realm and update their authentication workflow.
        var acrs = await DbContext.Acrs
            .Include(r => r.Realms)
            .Where(a => Constants.StandardAcrNames.Contains(a.Name) && a.Realms.Any(r => r.Name == Constants.DefaultRealm))
            .ToListAsync(cancellationToken);
        acrs.ForEach(a => a.AuthenticationWorkflow = DataSeeder.completePwdAuthWorkflowId);
    }

    private async Task UpdateManualProvisioning(CancellationToken cancellationToken)
    {
        // Fetch manual provisioning from the master realm and update their workflow.
        var registrationMethods = await DbContext.RegistrationWorkflows
            .Where(a => Constants.DefaultRealm == a.RealmName)
            .ToListAsync(cancellationToken);
        registrationMethods.ForEach(r =>
        {
            if(mappingManualProvisioningToRegistrationWorkflow.ContainsKey(r.Name))
            {
                r.WorkflowId = mappingManualProvisioningToRegistrationWorkflow[r.Name];
            }
        });
    }
}
