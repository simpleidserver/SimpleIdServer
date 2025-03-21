using DataSeeder;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Migrations;

public abstract class BaseAuthDataSeeder : BaseAfterDeploymentDataSeeder
{
    const string cssStyle = ":root {\r\n    --prompt-width: 600px;\r\n    --outer-padding: 80px;\r\n    --bg-color: #F7F7F6;\r\n    --from-bg-color: white;\r\n    --from-shadow: 0 12px 40px rgba(0,0,0,0.12);\r\n    --from-radius: 5px;\r\n    --divider-bg: gray;\r\n}\r\n\r\n.form {\r\n    padding: var(--outer-padding);\r\n    background-color: var(--bg-color);\r\n    display: flex;\r\n    flex-direction: column;\r\n    align-items: center;\r\n    height: 100%;\r\n}\r\n\r\n.form-content {\r\n    width: var(--prompt-width);\r\n    background-color: var(--from-bg-color);\r\n    box-shadow: var(--from-shadow);\r\n    border-radius: var(--from-radius);\r\n}\r\n\r\n.form-content.view {\r\n    padding: 10px;\r\n}\r\n\r\n/* Divider */\r\n.divider {\r\n    display: flex;\r\n    align-items: center;\r\n}\r\n\r\n.divider::before, .divider::after {\r\n    content: \"\";\r\n    height: 1px;\r\n    background: var(--divider-bg);\r\n    flex: 1;\r\n}\r\n\r\n.divider .text {\r\n    margin-right: 1rem;\r\n    margin-left: 1rem;\r\n    margin-top: 0px;\r\n    margin-bottom: 0px;\r\n    color: var(--divider-color);\r\n}\r\n\r\n.picture-container {\r\n    text-align: center;\r\n}\r\n\r\n.picture-container .picture {\r\n    width: 150px;\r\n}";
    private readonly IAuthenticationContextClassReferenceRepository _acrRepository;
    private readonly IFormStore _formStore;
    private readonly IWorkflowStore _workflowStore;

    protected BaseAuthDataSeeder(
        IAuthenticationContextClassReferenceRepository acrRepository,
        IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository,
        IFormStore formStore,
        IWorkflowStore workflowStore) : base(dataSeederExecutionHistoryRepository)
    {
        _acrRepository = acrRepository;
        _formStore = formStore;
        _workflowStore = workflowStore;
    }

    protected async Task<AuthenticationContextClassReference> TryAddAcr(string realm, AuthenticationContextClassReference acr, CancellationToken cancellationToken)
    {
       var existingAcr = await _acrRepository.GetByName(realm, acr.Name, cancellationToken);
       if (existingAcr != null)
       {
           return existingAcr;
       }
       
       _acrRepository.Add(acr);
        return acr;
    }

    protected async Task<bool> TryAddForm(string realm, FormRecord formRecord, CancellationToken cancellationToken)
    {
        var existingForm = await _formStore.Get(realm, formRecord.Id, cancellationToken);
        if (existingForm != null)
        {
            return false;
        }

        formRecord.AvailableStyles = new List<FormStyle>
        {
            new FormStyle
            {
                Id = Guid.NewGuid().ToString(),
                Content = cssStyle,
                IsActive = true
            }
        };
        _formStore.Add(formRecord);
        return true;
    }

    protected async Task<bool> TryAddWorkflow(string realm, WorkflowRecord workflow, CancellationToken cancellationToken)
    {
        var existingWorkflow = await _workflowStore.Get(realm, workflow.Id, cancellationToken);
        if (existingWorkflow != null)
        {
            return false;
        }
        _workflowStore.Add(workflow);
        return true;
    }

    protected async Task Commit(CancellationToken cancellationToken)
    {
        await _workflowStore.SaveChanges(cancellationToken);
        await _formStore.SaveChanges(cancellationToken);
    }
}