using FormBuilder.Link;
using FormBuilder.Models.Layout;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace FormBuilder.Startup.Workflows;

public class PwdAuthWorkflowLayout : IWorkflowLayoutService
{
    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            SourceFormCorrelationId = PwdAuthForms.LoginPwdAuthForm.CorrelationId,
            WorkflowCorrelationId = "pwdAuthWorkflow",
            Name = "pwd",
            Links = new List<WorkflowLinkLayout>
            {
                // Authenticate.
                new WorkflowLinkLayout
                {
                    EltCorrelationId = PwdAuthForms.authPwdFormId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = Link.HttpMethods.POST,
                        IsAntiforgeryEnabled = true,
                        Target = "/{realm}/pwd/Authenticate",
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters()
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = "$.Realm", Target = "realm" }
                                }
                            },
                            new RelativeUrlTransformerParameters()
                        }
                    })
                },
                // Forget my password.
                new WorkflowLinkLayout
                {
                    EltCorrelationId = PwdAuthForms.forgetMyPasswordId,
                    // TargetFormCorrelationId = PwdAuthForms.ResetPwdForm.CorrelationId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = Link.HttpMethods.GET,
                        Target = "/{realm}/pwd/Reset",
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters()
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" },
                                    new MappingRule { Source = "$.Realm", Target = "realm" }
                                }
                            },
                            new RelativeUrlTransformerParameters()
                        },
                        IsCustomParametersEnabled = true,
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = "$.AuthUrl", Target = "returnUrl" }
                        },
                    })
                }
            }
        };
    }
}