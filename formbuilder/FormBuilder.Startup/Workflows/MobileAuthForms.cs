using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using System.Collections.ObjectModel;

namespace FormBuilder.Startup.Workflows;

public class MobileAuthForms
{
    public static string mobileAuthFormId = "mobileAuthFormId";

    public static FormRecord MobileAuthForm = new FormRecord
    {
        Id = "mobileId",
        Name = "mobile",
        CorrelationId = "mobileId",
        ActAsStep = true,
        VersionNumber = 0,
        Status = RecordVersionStatus.Published,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = Guid.NewGuid().ToString(),
                CorrelationId = mobileAuthFormId,
                IsFormEnabled = true,
                FormType = FormTypes.HTML,
                HtmlAttributes = new Dictionary<string, object>
                {
                    { "id", "mobileAuthForm" }
                },
                Elements = new ObservableCollection<IFormElementRecord>
                {
                     new FormButtonRecord
                     {
                         Id = Guid.NewGuid().ToString(),
                         CorrelationId = "mobileAuthenticate",
                         Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").Build()
                     }
                }
            }
        }
    };
}