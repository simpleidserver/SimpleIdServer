using FormBuilder.Link;
using FormBuilder.Models.Layout;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace FormBuilder.Startup.Workflows;

public class MobileAuthWorkflowLayout : IWorkflowLayoutService
{
    public string Category => "auth";

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
           SourceFormCorrelationId = MobileAuthForms.MobileAuthForm.CorrelationId,
           WorkflowCorrelationId = "mobileAuthWorkflow",
           Name = "mobile",
           Links = new List<WorkflowLinkLayout>
           {               
                // Authenticate.
                new WorkflowLinkLayout
                {
                    Description = "Authenticate",
                    EltCorrelationId = MobileAuthForms.mobileAuthFormCorrelationId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = Link.HttpMethods.POST,
                        IsAntiforgeryEnabled = true,
                        Target = "/{realm}/mobile/Authenticate",
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
                }
           }
        };
    }
}
